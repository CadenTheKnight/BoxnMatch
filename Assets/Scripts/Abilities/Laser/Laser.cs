using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public AbilityDirection dir;
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        // Laser moves in straight line in fired direction
        transform.position += dir.GetUnitDirection() * Time.deltaTime * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
