using System.Text;
using UnityEditor;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    private Rigidbody rb;
    private LineRenderer lr;
    private UtilityManager turn;

    [Header("Launching Parameters")]
    [SerializeField] public float timerThreshold = 2f;
    [SerializeField, ReadOnly] private Vector3 dragStartPosition;
    [SerializeField, ReadOnly] private Vector3 dragEndPosition;
    [SerializeField, ReadOnly] private Vector3 launchDirection;
    [SerializeField, ReadOnly] public float timer;
    [SerializeField, ReadOnly] private bool isOnDice;
    [SerializeField, ReadOnly] private bool dragStart;
    [SerializeField, ReadOnly] private bool dragging;
    [SerializeField, ReadOnly] public int diceValue;
    [SerializeField, ReadOnly] public bool isStationary;
    [SerializeField, ReadOnly] public bool isLaunched;
    private Vector3[] diceFaces = { Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up };

    private bool isDiceStationary => rb.velocity.magnitude< 0.01f && rb.angularVelocity.magnitude< 0.01f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        turn = GameObject.FindWithTag("GameController").GetComponent<UtilityManager>();
        lr.enabled = false;
        InitialiseRandomDiceValue();
    }

    private void Update()
    {
        isStationary = isDiceStationary;
        if (turn.isPlayerTurn && isDiceStationary)
        {
            DiceValueRecognizer();
            OnDiceHandler();
            InputHandler();
        }
    }

    private void InputHandler()
    {
        if(isDiceStationary)
        {
            timer += Time.deltaTime;
            if(timer > timerThreshold && isOnDice)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DragStartHandler();
                    dragStart = true;
                }
                else if (Input.GetMouseButton(0) && dragStart)
                {
                    DraggingHandler();
                    dragging = true;
                }
                else if (Input.GetMouseButtonUp(0) && dragStart && dragging)
                {
                    DragEndHandler();
                    timer = 0f;
                    dragStart = false;
                    dragging = false;
                    isOnDice = false;
                    isLaunched = true;
                }
            }
        }
    }

    private void DragStartHandler()
    {
        dragStartPosition.x = Input.mousePosition.x;
        dragStartPosition.z = Input.mousePosition.y;
    }

    private void DraggingHandler()
    {
        dragEndPosition.x = Input.mousePosition.x;
        dragEndPosition.z = Input.mousePosition.y;
        launchDirection = - Vector3.Normalize(dragEndPosition - dragStartPosition);
        TrajectoryLineHandler();
    }

    private void DragEndHandler()
    {
        rb.AddForce(launchDirection * (float)diceValue * 0.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 25f, ForceMode.Impulse);

        Vector3 torque = Vector3.Cross(launchDirection, Vector3.up);
        rb.AddTorque(torque * 10f, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

        lr.enabled = false;
    }

    private void TrajectoryLineHandler()
    {
        lr.enabled = true;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + launchDirection * (float)diceValue);
    }

    private void OnDiceHandler()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f) && 
            hit.collider.gameObject == this.gameObject &&
            timer > timerThreshold &&
            isDiceStationary &&
            Input.GetMouseButtonDown(0))
        {
            isOnDice = true;
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            ContactPoint[] contactPoints = collision.contacts;
            foreach (ContactPoint contact in contactPoints)
            {
                //Debug.Log("Contact Normal: " + contact.normal);
                if (transform.position.y > contact.point.y && contact.normal.y >= 0.8f && contact.normal.y <= 1.0f) // Player Eliminating Enemy
                {
                    Debug.Log("Player eliminated Enemy");
                    collision.gameObject.SetActive(false);
                }
            }
        }
    }

}
