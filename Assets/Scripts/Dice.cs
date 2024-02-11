using D6Dice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public enum Tag
    {
        PlayerA,
        PlayerB,
        Powerups
    }

    [Header("Game Object")]
    public TurnBasedSystem TurnBasedSystem;
    public LineRenderer LineRenderer;
    public Rigidbody Rigidbody;
    public Outline Outline;
    public GameObject Explosion;
    public GameObject Boost;
    public CameraShake camShake;

    [Header("Audio")]
    public AudioSource CollisionSound;
    public AudioSource ExplosionSound;
    public AudioSource BoostSound;

    private List<GameObject> ExplosionInstances = new List<GameObject>();
    private List<GameObject> BoostInstances = new List<GameObject>();

    [Header("Variable")]
    public Tag DiceTag;
    private Vector3[] diceFaces = { Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up };

    [Header("Indicator Variable")]
    [ReadOnly] public float DiceValue;
    [ReadOnly] public Vector3 StartPosition;
    [ReadOnly] public Vector3 EndPosition;
    [ReadOnly] public Vector3 LaunchDirection;
    [ReadOnly] public float LaunchDistance;
    private bool onDice;
    public bool isPrep;
    public bool isLaunched;
    public bool isEliminated;
    private bool isCoroutineRunning;

    private void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        Rigidbody = GetComponent<Rigidbody>();

        ValueRandomizer();
    }

    private void Update()
    {
        #region Boolean
        OnDice();
        ResetState();
        #endregion
        ValueRecognizer();

        DrawProjectileIndicator();
        OutlineHandler();

        ExplosionHandler();
        BoostHandler();

        if (isEliminated && !isCoroutineRunning)
        {
            StartCoroutine(Eliminate());

            IEnumerator Eliminate()
            {
                isCoroutineRunning = true;
                yield return new WaitForSeconds(0.1f);
                this.gameObject.SetActive(false);
                TurnBasedSystem.Setup();
                isCoroutineRunning = false;
            }
        }

        VolumeUpdate();
    }


    private void OnCollisionEnter(Collision collision)
    {
        CollisionSound.Stop();
        CollisionSound.pitch = Random.Range(0f, 1f);
        CollisionSound.Play();
        if (TurnBasedSystem.GameState == D6Dice.GameState.Start) return;

        switch (collision.gameObject.tag)
        {
            case ("PlayerA"):
            case ("PlayerB"):
                if (gameObject.tag != collision.gameObject.tag && IsStack(collision))
                {
                    ExplosionSound.Play();
                    camShake.Shake(1f, 0.7f);
                    CreateExplosion(collision);
                    Rigidbody.velocity += (collision.contacts[0].normal * 15f);
                    collision.gameObject.GetComponent<Dice>().isEliminated = true;
                }
                else if (gameObject.tag == collision.gameObject.tag && IsStack(collision))
                {
                    BoostSound.Play();
                    CreateBoost(collision);
                    Rigidbody.velocity += (collision.contacts[0].normal * 15f);
                    collision.rigidbody.AddForceAtPosition(Vector3.one * 5f, collision.contacts[0].point, ForceMode.Impulse);
                }
                break;
            case ("PowerUps"):
                BoostSound.Play();
                CreateBoost(collision);
                Rigidbody.velocity += (collision.contacts[0].normal * 15f);
                collision.rigidbody.AddForceAtPosition(Vector3.one * 5f ,collision.contacts[0].point, ForceMode.Impulse);
                break;
            default:
                break;
        }
    }

    #region Dice Value
    private void ValueRandomizer()
    {
        Vector3 torque = Vector3.Cross(new Vector3(Random.value, 0f, Random.value), Vector3.up);
        GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void ValueRecognizer()
    {
        float minAngle = float.MaxValue;
        int closestFace = -1;

        for (int i = 0; i < diceFaces.Length; i++)
        {
            float angle = Vector3.Angle(transform.TransformDirection(diceFaces[i]), Vector3.up);
            if (angle < minAngle)
            {
                minAngle = angle;
                closestFace = i + 1;
            }
        }
        DiceValue = closestFace;
    }
    #endregion

    #region Draw
    private void DrawProjectileIndicator()
    {
        if (!onDice || isPrep || DiceTag.ToString() != TurnBasedSystem.GameState.ToString()) return;
        if (Input.GetMouseButtonDown(0))
        {
            StartPosition.x = Input.mousePosition.x;
            StartPosition.z = Input.mousePosition.y;
            
        }
        else if (Input.GetMouseButton(0))
        {
            EndPosition.x = Input.mousePosition.x;
            EndPosition.z = Input.mousePosition.y;
            LaunchDirection = -Vector3.Normalize(EndPosition - StartPosition);

            LineRenderer.enabled = true;
            LineRenderer.SetPosition(0, transform.position);

            float distance = Vector3.Distance(StartPosition, EndPosition);
            float normalizedDistance = Mathf.Clamp01(distance / 100.0f);
            LaunchDistance = normalizedDistance * 6f;

            if (LaunchDistance > DiceValue) LaunchDistance = DiceValue;
            LineRenderer.SetPosition(1, transform.position + LaunchDirection * LaunchDistance);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            LineRenderer.enabled = false;
            onDice = false;
            isPrep = true;
            Launch();
        }
    }
    private void OutlineHandler()
    {
        if (DiceTag.ToString() == TurnBasedSystem.GameState.ToString())
        {
            if (!isPrep)
                Outline.enabled = true;
            else
                Outline.enabled = false;
        }
        else
            Outline.enabled = false;
    }
    #endregion

    #region Launch
    public void Launch()
    {
        if (isLaunched) return;
        CollisionSound.Stop();
        CollisionSound.pitch = Random.Range(1f, 1.5f);
        CollisionSound.Play();

        Rigidbody.AddForce(LaunchDirection * LaunchDistance * 0.5f, ForceMode.Impulse);
        Rigidbody.AddForce(Vector3.up * 25f, ForceMode.Impulse);

        Vector3 torque = Vector3.Cross(LaunchDirection, Vector3.up);
        Rigidbody.AddTorque(-torque * 10f, ForceMode.Impulse);
        Rigidbody.angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

        isLaunched = true;
    }
    #endregion

    #region Boolean
    private void OnDice()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject == this.gameObject &&
            Rigidbody.IsSleeping() && TurnBasedSystem.GameState.ToString() == DiceTag.ToString() && 
            Input.GetMouseButtonDown(0))
        {
            onDice = true;
        }
    }
    private void ResetState()
    {
        if (TurnBasedSystem.GameState.ToString() != DiceTag.ToString())
        {
            isPrep = false;
            isLaunched = false;
        }
    }
    private bool IsStack(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        foreach (ContactPoint contact in contactPoints)
        {
            if (transform.position.y > contact.point.y && contact.normal.y >= 0.8f && contact.normal.y <= 1.2f)
                return true;
        }
        return false;
    }
    #endregion

    #region Visual Effects
    private void CreateExplosion(Collision collision)
    {
        GameObject temp = Instantiate(Explosion, collision.contacts[0].point, Quaternion.identity);
        temp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        ExplosionInstances.Add(temp);
    }
    private void ExplosionHandler()
    {
        if (ExplosionInstances == null) return;
        List<GameObject> temp = new List<GameObject>(ExplosionInstances.Count);
        foreach (var explosion in ExplosionInstances)
        {
            ParticleSystem _particleSystem = explosion.GetComponent<ParticleSystem>();
            if (!_particleSystem.IsAlive())
            {
                temp.Add(explosion);
            }
        }
        foreach (var explosionToRemove in temp)
        {
            ExplosionInstances.Remove(explosionToRemove);
            Destroy(explosionToRemove);
        }
    }
    private void CreateBoost(Collision collision)
    {
        GameObject temp = Instantiate(Boost, collision.contacts[0].point, Quaternion.identity);
        temp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        BoostInstances.Add(temp);
    }
    private void BoostHandler()
    {
        if (BoostInstances == null) return;
        List<GameObject> temp = new List<GameObject>(BoostInstances.Count);
        foreach (var boost in BoostInstances)
        {
            ParticleSystem _particleSystem = boost.GetComponent<ParticleSystem>();
            if (!_particleSystem.IsAlive())
            {
                temp.Add(boost);
            }
        }
        foreach (var boostToRemove in temp)
        {
            BoostInstances.Remove(boostToRemove);
            Destroy(boostToRemove);
        }
    }
    #endregion

    #region Volume
    public void VolumeUpdate()
    {
        ExplosionSound.volume = MenuController.soundVolume;
        BoostSound.volume = MenuController.soundVolume;
        CollisionSound.volume = MenuController.soundVolume;
    }
    #endregion

}
