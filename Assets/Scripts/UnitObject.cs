using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    public GameController gameController;
    public PowerUps playerAbility;
    public PowerUps opponentAbility;
    [ReadOnly] public bool isAimed;
    [ReadOnly] public float diceValue;
    [ReadOnly] public Vector3 startPosition;
    [ReadOnly] public Vector3 endPosition;
    [ReadOnly] public Vector3 launchDirection;
    [ReadOnly] public float timer;
    [ReadOnly] public bool sunAbility;
    [ReadOnly] public bool crossAbility;
    [ReadOnly] public bool crownAbility;
    [ReadOnly] public bool anchorAbility;
    [ReadOnly] public bool eagleAbility;
    [ReadOnly] public bool gobletAbility;
    [ReadOnly] public bool isSnowflake;

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
        //Sun Ability
        if (sunAbility && GetComponent<Rigidbody>().velocity.y < -0.1f)
        {
            float explosionRadius = transform.localScale.x / 50.0f * 2.0f;
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider collider in colliders)
            {
                Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                if (rigidbody && !rigidbody.isKinematic)
                {
                    Vector3 direction = rigidbody.position - transform.position;
                    float distance = direction.magnitude;

                    float forceMultiplier = Mathf.Clamp(distance / explosionRadius, 0f, 1f);
                    rigidbody.AddForce(direction.normalized * forceMultiplier * 10f, ForceMode.Impulse);
                }
            }
        }

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
                        if (collision.gameObject.GetComponent<UnitObject>().crownAbility) //Crown Ability
                        {
                            Vector3 direction = gameObject.transform.position - collision.transform.position;
                            gameObject.GetComponent<Rigidbody>().AddForce(direction.normalized * 10.0f, ForceMode.Impulse);
                            collision.gameObject.GetComponent<UnitObject>().crownAbility = false;
                            return;
                        }
                        else
                        {
                            Debug.Log("Player eliminated Opponent");
                            collision.gameObject.SetActive(false);
                            gameController.Setup();
                        }
                    }
                }
            }
            else if (collision.gameObject.tag == "PowerUps")
            {
                playerAbility.ObtainPowerups(gameObject);
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
                        if (collision.gameObject.GetComponent<UnitObject>().crownAbility) //Crown Ability
                        {
                            Vector3 direction = gameObject.transform.position - collision.transform.position;
                            gameObject.GetComponent<Rigidbody>().AddForce(direction.normalized * 10.0f, ForceMode.Impulse);
                            collision.gameObject.GetComponent<UnitObject>().crownAbility = false;
                            return;
                        }
                        else
                        {
                            Debug.Log("Opponent eliminated Player");
                            collision.gameObject.SetActive(false);
                            gameController.Setup();
                        }
                    }
                }
            }
            else if (collision.gameObject.tag == "PowerUps")
            {
                opponentAbility.ObtainPowerups(gameObject);
            }
        }
    }
}
