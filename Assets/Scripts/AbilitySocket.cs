using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySocket : MonoBehaviour
{
    [SerializeField] private AbilityDirection socketDirection;
    [SerializeField] public AbilityBinding ability;

    public void UpdateRotation(int quarterCircles)
    {
        socketDirection.Rotate(quarterCircles);
    }

    public void FireAbility(PlayerRotator pr)
    {
        GetComponent<SpriteRenderer>().color = Color.red;

        if(ability != null)
        {
            ability.Fire(socketDirection, pr);
        }
    }
}
