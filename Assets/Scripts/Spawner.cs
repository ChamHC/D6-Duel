using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField, Range(1, 20)] public int playerCount;
    [SerializeField, Range(1, 20)] public int opponentCount;
    [SerializeField, Range(1, 20)] public int powerupCount;
    public GameObject playerSource;
    public GameObject opponentSource;
    public GameObject powerupSource;
    public GameController gameController;

    public void Start()
    {
        for (int i = 1; i < playerCount + 1; i++)
        {
            Vector3 offset = new Vector3(0f, 2f, 0f) * i;
            GameObject playerClone = Instantiate(playerSource, playerSource.transform.position + offset, Quaternion.identity);
            playerClone.SetActive(true);
        }

        for (int i = 1; i < opponentCount + 1; i++)
        {
            Vector3 offset = new Vector3(0f, 2f, 0f) * i;
            GameObject opponentClone = Instantiate(opponentSource, opponentSource.transform.position + offset, Quaternion.identity);
            opponentClone.SetActive(true);
        }

        for (int i = 1; i < powerupCount + 1; i++)
        {
            Vector3 offset = new Vector3(0f, 2f, 0f) * i;
            GameObject powerupClone = Instantiate(powerupSource, powerupSource.transform.position + offset, Quaternion.identity);
            powerupClone.SetActive(true);
        }

        gameController.Setup();
    }
}
