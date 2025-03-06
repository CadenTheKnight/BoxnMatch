using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteExplosive : MonoBehaviour
{
    [SerializeField] Explosion explosion;
    [SerializeField] float explosionDuration;
    [SerializeField] int explosionDamage;
    [SerializeField] int explosionKnockback;

    public void explode()
    {
        Explosion e = Instantiate(explosion, transform.position, transform.rotation);
        DamageObject d = e.GetComponent<DamageObject>();
        e.explosionDuration = explosionDuration;
        d.damage = explosionDamage;
        d.knockback = explosionKnockback;
    }


}
