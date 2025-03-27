using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Hammer : MonoBehaviour
{

    private DamageObject d;
    private Vector2 baseDamageKnockback;
    private Rigidbody2D rb;
    private Vector2 previousPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        d = gameObject.GetComponent<DamageObject>();
        baseDamageKnockback = d.getDamageKnockback();
        previousPos = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceTraveled = (previousPos - rb.position).magnitude;
        float speed = (int)(distanceTraveled / Time.deltaTime);

        // Updates damage and knockback values based on the speed of the hammer
        d.setDamageKnockback(baseDamageKnockback * speed);
        Debug.Log(d.getDamageKnockback());

        previousPos = rb.position;
    }
}
