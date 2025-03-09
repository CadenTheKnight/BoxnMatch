using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DamageableObject : MonoBehaviour
{

    public float damageModifier = 1f;
    public float currentDamage = 0f;

    private Rigidbody2D rb;
    private TMP_Text damageText;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageText = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        damageText.text = currentDamage + "%";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        handleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        handleCollision(collision.gameObject);
    }

    private void handleCollision(GameObject go)
    {
        if (go.CompareTag("DamageObject"))
        {
            // Try to access the damage object (extra check in case something got miss labeled)
            DamageObject d;
            try
            {
                d = go.GetComponent<DamageObject>();
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
            Vector2 collisionDirection = transform.position - go.transform.position;
            HandleKnockback(knockback, collisionDirection);
        }
    }

    private void HandleKnockback(float knockback, Vector2 dir)
    {
        Vector2 knockbackVelocity = dir * (knockback * currentDamage / 100);
        rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);
    }
}
