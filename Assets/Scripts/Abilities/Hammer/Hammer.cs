using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{

    private DamageObject d;
    private Vector2 baseDamageKnockback;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        d = gameObject.GetComponent<DamageObject>();
        baseDamageKnockback = d.getDamageKnockback();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates damage and knockback values based on the velocity of the hammer
        d.setDamageKnockback(baseDamageKnockback * rb.velocity.magnitude);
    }
}
