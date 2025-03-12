using UnityEngine;

public class RemoteExplosive : MonoBehaviour
{
    [SerializeField] Explosion explosion;
    [SerializeField] float explosionDuration;
    [SerializeField] int explosionDamage;
    [SerializeField] int explosionKnockback;

    public void Explode()
    {
        Explosion e = Instantiate(explosion, transform.position, transform.rotation);
        DamageObject d = e.GetComponent<DamageObject>();
        e.explosionDuration = explosionDuration;
        d.damage = explosionDamage;
        d.knockback = explosionKnockback;
        Destroy(gameObject);
    }


}
