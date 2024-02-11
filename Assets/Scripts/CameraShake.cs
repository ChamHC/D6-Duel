using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeDuration;
    public float ShakeAmount;
    public float DecreaseFactor = 1.0f;
    [ReadOnly] public Vector3 OriginalPos;

    void Start()
    {
        OriginalPos = gameObject.transform.position;
    }

    void Update()
    {
        if (ShakeDuration > 0)
        {
            gameObject.transform.localPosition = OriginalPos + Random.insideUnitSphere * ShakeAmount;
            ShakeDuration -= Time.deltaTime * DecreaseFactor;
            ShakeAmount -= Time.deltaTime * DecreaseFactor;

            if (ShakeAmount <= 0) 
                ShakeAmount = 0;
        }
        else
        {
            ShakeDuration = 0f;
            gameObject.transform.localPosition = OriginalPos;
        }
    }

    public void Shake(float shakeDuration, float shakeAmount)   //Duration = 1f, Amount = 0.7f
    {
        ShakeDuration = shakeDuration;
        ShakeAmount = shakeAmount;
    }
}
