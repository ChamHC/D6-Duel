using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    [ReadOnly] public bool isAimed;
    [ReadOnly] public int diceValue;
    [ReadOnly] public Vector3 startPosition;
    [ReadOnly] public Vector3 endPosition;
    [ReadOnly] public Vector3 launchDirection;
    [ReadOnly] public float timer;
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Player")
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
                        Debug.Log("Enemy eliminated Player");
                        collision.gameObject.SetActive(false);
                    }
                }
            }
        }
    }*/
}
