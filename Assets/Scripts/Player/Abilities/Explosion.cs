using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Explosion : MonoBehaviour
{
    [SerializeField] public float explosionDuration;

    private float time = 0;

    private void Update()
    { 
        time += Time.deltaTime;

        if(time > explosionDuration)
        {
            Destroy(gameObject);
        }
    }
}
