using TMPro;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public float damageModifier = 1f;
    public float currentDamage = 0f;

    private Rigidbody2D rb;
    [SerializeField] private TMP_Text damageText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        damageText.text = currentDamage + "%";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject go)
    {
        if (go.CompareTag("DamageObject"))
        {
            DamageObject d = go.GetComponent<DamageObject>();

            Vector2 damageKnockback = d.GetDamageKnockback();
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
