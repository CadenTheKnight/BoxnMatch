using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{

    [SerializeField] float damage = 0f;
    [SerializeField] float knockback = 0f;

    public Vector2 getDamageKnockback()
    {
        return new Vector2(damage, knockback);
    }

}
