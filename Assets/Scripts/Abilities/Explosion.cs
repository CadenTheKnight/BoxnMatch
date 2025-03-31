using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Explosion : MonoBehaviour
{
    [SerializeField] public float explosionDuration;
    [SerializeField] private float visibleDuration;

    private float time = 0;

    private void Update()
    { 
        time += Time.deltaTime;

        if(time > visibleDuration) // removes the whole object when we dont want to see or hear it anymore
        {
            Destroy(gameObject);
        }

        if(time > explosionDuration) // removes collider after explosion duration ends
        {
            Destroy(gameObject.GetComponent<Collider2D>());
        }
    }
}
