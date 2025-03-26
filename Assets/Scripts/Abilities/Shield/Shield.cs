using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public AbilityDirection dir;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>())
        {

            Destroy(gameObject);
        }
    }
}
