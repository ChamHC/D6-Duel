using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum GameState
{
    Start,
    PlayerTurn,
    OpponentTurn,
    Victory,
    Defeat
}

public class TurnController : MonoBehaviour
{
    [ReadOnly] public GameState state;
    [Header("Assigned Objects")]
    public List<GameObject> players;
    public List<GameObject> opponents;
    public TextMeshProUGUI turnText;
    public Material playerMaterial;
    public Material altPlayerMaterial;
    private bool isCoroutineRunning;

    [Header("Dice Parameters")]
    [ReadOnly] public GameObject selectedDice;
    private Vector3[] diceFaces = { Vector3.down, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up };

    [Header("Drag Parameters")]
    [ReadOnly] public bool isOnDice;

    [Header("AI Parameters")]
    [SerializeField, Range(0, 1)] public float attackChance;



    void Start()
    {
        state = GameState.Start;

        Setup();
        StartCoroutine(TurnHandler());
    }

    void Update()
    {
        if (!isCoroutineRunning)
        {
            PlayerAction();
            OpponentAction();
        }
    }

    public void Setup()
    {
        players = new List<GameObject>();
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerGameObjects)
        {
            players.Add(player);
        }

        opponents = new List<GameObject>();
        GameObject[] opponentGameObjects = GameObject.FindGameObjectsWithTag("Opponent");
        foreach (var opponent in opponentGameObjects)
        {
            opponents.Add(opponent);
        }

        altPlayerMaterial.SetFloat("_Glossiness", 1f);
    }

    IEnumerator TurnHandler()
    {
        //Debug.Log("Coroutine Started");
        isCoroutineRunning = true;
        yield return new WaitUntil(() => AllStationary());
        if (state == GameState.OpponentTurn || state == GameState.Start)
        {
            turnText.text = "Player's\nTurn";
        }
        else if (state == GameState.PlayerTurn)
        {
            yield return new WaitForSeconds(1f);
            turnText.text = "Opponent's\nTurn";
        }

        var color = turnText.color;
        while (turnText.color.a < 1f)
        {
            //Debug.Log("Increase Opacity: " + turnText.color.a);
            color = turnText.color;
            color.a = Mathf.Lerp(color.a, 2f, 2f * Time.deltaTime);
            turnText.color = color;
            yield return null;
        }
        yield return new WaitUntil(() => turnText.color.a >= 1f);
        yield return new WaitForSeconds(2.0f);
        while (turnText.color.a >= 0f)
        {
            //Debug.Log("Reduce Opacity: " + turnText.color.a);
            color = turnText.color;
            color.a = Mathf.Lerp(color.a, -1f, 2f * Time.deltaTime);
            turnText.color = color;
            yield return null;
        }

        if (state == GameState.OpponentTurn || state == GameState.Start)
        {
            state = GameState.PlayerTurn;
        }
        else if (state == GameState.PlayerTurn)
        {
            state = GameState.OpponentTurn;
        }

        isCoroutineRunning = false;
        //Debug.Log("Coroutine Ended");
    }
    /*
        public void PlayerAction()
        {
            if (state != GameState.PlayerTurn)
                return;

            bool allPlayersLaunched = false;
            foreach (var player in players)
            {
                if (!player.GetComponent<UnitObject>().isAimed)
                {
                    OnDiceHandler(player.gameObject);
                    DiceValueRecognizer(player.gameObject);
                    if (Input.GetMouseButtonDown(0))
                    {
                        player.GetComponent<UnitObject>().startPosition.x = Input.mousePosition.x;
                        player.GetComponent<UnitObject>().startPosition.z = Input.mousePosition.y;
                        //Debug.Log("Clicked");
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        player.GetComponent<UnitObject>().endPosition.x = Input.mousePosition.x;
                        player.GetComponent<UnitObject>().endPosition.z = Input.mousePosition.y;
                        player.GetComponent<UnitObject>().launchDirection = -Vector3.Normalize(player.GetComponent<UnitObject>().endPosition - player.GetComponent<UnitObject>().startPosition);

                        player.GetComponent<LineRenderer>().enabled = true;
                        player.GetComponent<LineRenderer>().SetPosition(0, player.transform.position);
                        player.GetComponent<LineRenderer>().SetPosition(1, player.transform.position + player.GetComponent<UnitObject>().launchDirection * (float)diceValue);
                        //Debug.Log("Dragged");
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        player.GetComponent<LineRenderer>().enabled = false;
                        player.GetComponent<UnitObject>().isAimed = true;
                        isOnDice = false;
                        //Debug.Log("Released");
                    }
                    allPlayersLaunched = players.All(player => player.GetComponent<UnitObject>().isAimed);
                }
            }

            if (allPlayersLaunched)
            {
                foreach (var player in players)
                {
                    //Debug.Log("Launched");
                    player.GetComponent<Rigidbody>().AddForce(player.GetComponent<UnitObject>().launchDirection * (float)diceValue * 0.5f, ForceMode.Impulse);
                    player.GetComponent<Rigidbody>().AddForce(Vector3.up * 25f, ForceMode.Impulse);

                    Vector3 torque = Vector3.Cross(player.GetComponent<UnitObject>().launchDirection, Vector3.up);
                    player.GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
                    player.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                    player.GetComponent<UnitObject>().isAimed = false;
                    Debug.Log(player.gameObject.name + " Value: " + diceValue);
                }

                if(!isCoroutineRunning)
                    StartCoroutine(TurnHandler());
            }
        }
    */

    public void PlayerAction()
    {
        if (state != GameState.PlayerTurn)
            return;

        bool allPlayersLaunched = false;
        foreach (var player in players)
        {
            if (!player.GetComponent<UnitObject>().isAimed)
            {
                DicePulser(player);
                OnDiceHandler(player.gameObject);
                if (isOnDice)
                {
                    DiceValueRecognizer(selectedDice.gameObject);
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedDice.GetComponent<UnitObject>().startPosition.x = Input.mousePosition.x;
                        selectedDice.GetComponent<UnitObject>().startPosition.z = Input.mousePosition.y;
                        //Debug.Log("Clicked");
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        selectedDice.GetComponent<UnitObject>().endPosition.x = Input.mousePosition.x;
                        selectedDice.GetComponent<UnitObject>().endPosition.z = Input.mousePosition.y;
                        selectedDice.GetComponent<UnitObject>().launchDirection = -Vector3.Normalize(selectedDice.GetComponent<UnitObject>().endPosition - selectedDice.GetComponent<UnitObject>().startPosition);

                        selectedDice.GetComponent<LineRenderer>().enabled = true;
                        selectedDice.GetComponent<LineRenderer>().SetPosition(0, selectedDice.transform.position);
                        selectedDice.GetComponent<LineRenderer>().SetPosition(1, selectedDice.transform.position + selectedDice.GetComponent<UnitObject>().launchDirection * (float)selectedDice.GetComponent<UnitObject>().diceValue);
                        //Debug.Log("Dragged");
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        selectedDice.GetComponent<MeshRenderer>().material = playerMaterial;
                        selectedDice.GetComponent<LineRenderer>().enabled = false;
                        selectedDice.GetComponent<UnitObject>().isAimed = true;
                        isOnDice = false;
                        //Debug.Log("Released");
                    }
                }
                allPlayersLaunched = players.All(player => player.GetComponent<UnitObject>().isAimed);
            }
        }

        if (allPlayersLaunched)
        {
            foreach (var player in players)
            {
                //Debug.Log("Launched");
                player.GetComponent<Rigidbody>().AddForce(player.GetComponent<UnitObject>().launchDirection * (float)player.GetComponent<UnitObject>().diceValue * 0.5f, ForceMode.Impulse);
                player.GetComponent<Rigidbody>().AddForce(Vector3.up * 25f, ForceMode.Impulse);

                Vector3 torque = Vector3.Cross(player.GetComponent<UnitObject>().launchDirection, Vector3.up);
                player.GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
                player.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                player.GetComponent<UnitObject>().isAimed = false;
                Debug.Log(player.gameObject.name + " Value: " + player.GetComponent<UnitObject>().diceValue);
            }
            altPlayerMaterial.SetFloat("_Glossiness", 1.0f);

            if (!isCoroutineRunning)
                StartCoroutine(TurnHandler());
        }
    }
    public void OpponentAction()
    {
        if (state != GameState.OpponentTurn)
            return;

        bool allOpponentsLaunched = false;
        foreach (var opponent in opponents)
        {
            if (!opponent.GetComponent<UnitObject>().isAimed)
            {
                DiceValueRecognizer(opponent);
                if (Random.value < attackChance)
                {
                    Vector3 playerDirection = players[Random.Range(0, players.Count)].transform.position - opponent.transform.position;
                    opponent.GetComponent<UnitObject>().launchDirection = Vector3.Normalize(playerDirection);
                }
                else
                {
                    Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                    opponent.GetComponent<UnitObject>().launchDirection = randomDirection;
                }
                opponent.GetComponent<UnitObject>().isAimed = true;

                allOpponentsLaunched = opponents.All(opponent => opponent.GetComponent<UnitObject>().isAimed);
            }
        }

        if (allOpponentsLaunched)
        {
            foreach (var opponent in opponents)
            {
                //Debug.Log("Launched");
                opponent.GetComponent<Rigidbody>().AddForce(opponent.GetComponent<UnitObject>().launchDirection * (float)opponent.GetComponent<UnitObject>().diceValue * 0.5f, ForceMode.Impulse);
                opponent.GetComponent<Rigidbody>().AddForce(Vector3.up * 25f, ForceMode.Impulse);

                Vector3 torque = Vector3.Cross(opponent.GetComponent<UnitObject>().launchDirection, Vector3.up);
                opponent.GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
                opponent.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                opponent.GetComponent<UnitObject>().isAimed = false;
                Debug.Log(opponent.gameObject.name + " Value: " + opponent.GetComponent<UnitObject>().diceValue);
            }

            if (!isCoroutineRunning)
                StartCoroutine(TurnHandler());
        }
    }

    public void DicePulser(GameObject player)
    {
        player.GetComponent<MeshRenderer>().material = altPlayerMaterial;
        if (player.GetComponent<UnitObject>().timer <= 2f)
        {
            altPlayerMaterial.SetFloat("_Glossiness", Mathf.Lerp(1f, 0.5f, player.GetComponent<UnitObject>().timer / 2f));
        }
        else if (player.GetComponent<UnitObject>().timer <= 4f)
        {
            altPlayerMaterial.SetFloat("_Glossiness", Mathf.Lerp(0.5f, 1f, (player.GetComponent<UnitObject>().timer - 2f) / 2f));
        }
        else
        {
            altPlayerMaterial.SetFloat("_Glossiness", 1f);
            player.GetComponent<UnitObject>().timer = 0f; // Reset timer
        }
        player.GetComponent<UnitObject>().timer += Time.deltaTime;
    }

    //Utility
    private bool AllStationary()
    {
        bool isStationary = true;
        if (players != null)
        {
            foreach (var player in players)
            {
                if (!player.GetComponent<Rigidbody>().IsSleeping())
                {
                    isStationary = false;
                }
            }
        }
        if (opponents != null)
        {
            foreach (var opponent in opponents)
            {
                if (!opponent.GetComponent<Rigidbody>().IsSleeping())
                {
                    isStationary = false;
                }
            }
        }
        return isStationary;
    }

    private void OnDiceHandler(GameObject player)
    {
        if (state != GameState.PlayerTurn)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f) &&
            hit.collider.gameObject == player &&
            AllStationary() &&
            Input.GetMouseButtonDown(0))
        {
            isOnDice = true;
            selectedDice = player;
        }
    }

    private void DiceValueRecognizer(GameObject dice)
    {
        float minAngle = float.MaxValue;
        int closestFace = -1;

        for (int i = 0; i < diceFaces.Length; i++)
        {
            float angle = Vector3.Angle(dice.transform.TransformDirection(diceFaces[i]), Vector3.up);
            if (angle < minAngle)
            {
                minAngle = angle;
                closestFace = i + 1;
            }
        }
        dice.GetComponent<UnitObject>().diceValue = closestFace;
        dice.GetComponent<UnitObject>().diceValue = closestFace;
    }
}
