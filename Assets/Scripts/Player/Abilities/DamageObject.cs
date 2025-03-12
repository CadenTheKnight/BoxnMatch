using UnityEngine;

public class DamageObject : MonoBehaviour
{

    [SerializeField] public float damage = 0f;
    [SerializeField] public float knockback = 0f;

    public Vector2 GetDamageKnockback()
    {
        return new Vector2(damage, knockback);
    }

}
