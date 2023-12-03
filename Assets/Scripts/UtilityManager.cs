using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilityManager : MonoBehaviour
{
    [SerializeField] private DiceController player;
    [SerializeField] private GameObject victoryObject;
    [SerializeField] private GameObject defeatObject;
    private EnemyAIController[] enemies;
    public bool isPlayerTurn = true;

    private void Start()
    {
        AssignGameObjects();
        StartCoroutine(TurnLoop());
        victoryObject.SetActive(false);
        defeatObject.SetActive(false);
    }

    private void Update()
    {
        LevelEndHandler();
    }

    private void AssignGameObjects()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<DiceController>();
        }
        else
        {
            Debug.LogError("Player (Dice) not found or does not have DiceController component!");
        }

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new EnemyAIController[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemies[i] = enemyObjects[i].GetComponent<EnemyAIController>();
        }
    }

    private IEnumerator TurnLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => AllDicesStationary());

            if (isPlayerTurn)
            {
                Debug.Log("Player's turn");
                yield return new WaitUntil(() => PlayerTurnConditions());
                isPlayerTurn = false;
                player.isLaunched = false;

                Debug.Log("Player Dice Value: " + player.diceValue);
            }
            else
            {
                yield return new WaitUntil(() => EnemyTurnConditions());
                Debug.Log("Enemy's turn");
                isPlayerTurn = true;
                foreach (EnemyAIController enemy in enemies)
                {
                    enemy.isLaunched = false;

                    Debug.Log(enemy.gameObject.name + " Value: " + enemy.diceValue);
                }
            }
        }
    }

    private bool AllDicesStationary()
    {
        bool allStationary = player != null && player.isStationary;

        foreach (EnemyAIController enemy in enemies)
        {
            allStationary &= enemy != null && enemy.isStationary;
        }

        return allStationary;
    }

    private bool PlayerTurnConditions()
    {
        return player != null && player.isStationary && player.isLaunched;
    }

    private bool EnemyTurnConditions()
    {
        bool allEnemyLaunched = true;

        foreach (EnemyAIController enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                if (enemy == null || enemy.isStationary || !enemy.isLaunched)
                {
                    allEnemyLaunched = false;
                    break;
                }
            }
        }

        return allEnemyLaunched;
    }

    private void LevelEndHandler()
    {
        if (victoryObject == null || defeatObject == null)
        {
            Debug.LogError("Victory or Defeat objects not found under Canvas!");
            return;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            victoryObject.SetActive(true);
        }
        else if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            defeatObject.SetActive(true);
        }
    }

    public void RestartHandler()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevelHandler()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitHandler()
    {
        SceneManager.LoadScene(0);
    }
}
