using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    public GameController turnController;
    public PowerUps playerAbility;
    public PowerUps opponentAbility;
    [ReadOnly] public bool isAimed;
    [ReadOnly] public int diceValue;
    [ReadOnly] public Vector3 startPosition;
    [ReadOnly] public Vector3 endPosition;
    [ReadOnly] public Vector3 launchDirection;
    [ReadOnly] public float timer;

    private void Start()
    {
        StartValueRandomizer();
    }

    private void StartValueRandomizer()
    {
        Vector3 torque = Vector3.Cross(new Vector3(Random.value, 0f, Random.value), Vector3.up);
        gameObject.GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Player")
        {
            if (collision.gameObject.tag == "Opponent")
            {
                ContactPoint[] contactPoints = collision.contacts;
                foreach (ContactPoint contact in contactPoints)
                {
                    //Debug.Log("Contact Normal: " + contact.normal);
                    if (transform.position.y > contact.point.y && contact.normal.y >= 0.8f && contact.normal.y <= 1.0f) // Player Eliminating Enemy
                    {
                        Debug.Log("Player eliminated Opponent");
                        collision.gameObject.SetActive(false);
                        turnController.Setup();
                    }
                }
            }
            else if (collision.gameObject.tag == "PowerUps")
            {
                playerAbility.ObtainPowerups();
            }
        }
        else if (gameObject.tag == "Opponent")
        {
            if (collision.gameObject.tag == "Player")
            {
                ContactPoint[] contactPoints = collision.contacts;
                foreach (ContactPoint contact in contactPoints)
                {
                    //Debug.Log("Contact Normal: " + contact.normal);
                    if (transform.position.y > contact.point.y && contact.normal.y >= 0.8f && contact.normal.y <= 1.0f) // Enemy Eliminating Player
                    {
                        Debug.Log("Opponent eliminated Player");
                        collision.gameObject.SetActive(false);
                        turnController.Setup();
                    }
                }
            }
            else if (collision.gameObject.tag == "PowerUps")
            {
                opponentAbility.ObtainPowerups();
            }
        }
    }
}
