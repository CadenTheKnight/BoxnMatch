using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{

    public float damageModifier = 1f;

    private float currentDamage = 0f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.tag == "DamageObject")
        {


            // Try to access the damage object (extra check in case something got miss labeled)
            DamageObject d;
            try
            {
                d = collision.gameObject.GetComponent<DamageObject>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("----------------------------\nError: No DamageObject Script is attatched\n----------------------------");
                return;
            }

            // get damage and knockback from script on object
            Vector2 damageKnockback = d.getDamageKnockback();
            float damage = damageKnockback[0];
            float knockback = damageKnockback[1];

            currentDamage += damage;
            Vector2 collisionDirection = transform.position - collision.transform.position;
            handleKnockback(knockback, collisionDirection);

        }
    }

    private void handleKnockback(float knockback, Vector2 dir)
    {
        Vector2 knockbackVelocity = dir * (knockback * currentDamage / 100);
        rb.velocity += knockbackVelocity;
        Debug.Log("Knockback added " + knockbackVelocity + " to velocity");
    }
}
