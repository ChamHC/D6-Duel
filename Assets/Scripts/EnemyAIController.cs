using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    private Rigidbody rb;
    private DiceController player;
    private UtilityManager turn;

    [Header("Launching Parameters")]
    [SerializeField] private float timerThreshold = 1f;
    [SerializeField, ReadOnly] private Vector3 launchDirection;
    [SerializeField, ReadOnly] private float timer;
    [SerializeField, ReadOnly] public int diceValue;
    [SerializeField, ReadOnly] public bool isStationary;
    [SerializeField, ReadOnly] public bool isLaunched;
    private Vector3[] diceFaces = { Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up };

    private bool isDiceStationary => rb.velocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.01f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<DiceController>();
        turn = GameObject.FindWithTag("GameController").GetComponent<UtilityManager>();
        InitialiseRandomDiceValue();
    }

    private void Update()
    {
        isStationary = isDiceStationary;
        if (isDiceStationary && !turn.isPlayerTurn && player.isStationary)
        {
            DiceValueRecognizer();
            AIAction();
        }
    }

    private void AIAction()
    {
        timer += Time.deltaTime;

        if (timer > timerThreshold)
        {
            // Simulate AI decision (60% chance towards player, 40% random direction)
            if (Random.value < 0.6f)
            {
                LaunchTowardsPlayer();
            }
            else
            {
                LaunchInRandomDirection();
            }
            isLaunched = true;
            timer = 0f;
        }
    }

    private void LaunchTowardsPlayer()
    {
        Vector3 playerDirection = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        launchDirection = Vector3.Normalize(playerDirection);
        rb.AddForce(launchDirection * (float)diceValue * 0.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 25f, ForceMode.Impulse);
        UpdateDiceRotation();
    }

    private void LaunchInRandomDirection()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        launchDirection = randomDirection;
        rb.AddForce(launchDirection * (float)diceValue * 0.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 25f, ForceMode.Impulse);
        UpdateDiceRotation();
    }

    private void DiceValueRecognizer()
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
        diceValue = closestFace;
    }

    private void InitialiseRandomDiceValue()
    {
        Vector3 torque = Vector3.Cross(new Vector3(Random.value, 0f, Random.value), Vector3.up);
        rb.AddTorque(torque * 10f, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void UpdateDiceRotation()
    {
        Vector3 torque = Vector3.Cross(launchDirection, Vector3.up);
        rb.AddTorque(torque * 10f, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ContactPoint[] contactPoints = collision.contacts;
            foreach (ContactPoint contact in contactPoints)
            {
                //Debug.Log("Contact Normal: " + contact.normal);
                if (transform.position.y > contact.point.y && contact.normal.y >= 0.8f && contact.normal.y <= 1.0f) // Enemy Eliminating Player
                {
                    Debug.Log("Enemy eliminated Player");
                    collision.gameObject.SetActive(false);
                }
            }
        }
    }
}
