using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySocket : MonoBehaviour
{
    [SerializeField] public AbilityDirection socketDirection;
    [SerializeField] public AbilityBinding ability;

    public void UpdateRotation(int quarterCircles)
    {
        socketDirection.Rotate(quarterCircles);
    }

    public void FireAbility(PlayerRotator pr)
    {
        //GetComponent<SpriteRenderer>().color = Color.red;

        if(ability != null)
        {
            ability.Fire(socketDirection, pr);
        }
    }

    public void TrashAbilityBinding()
    {
        //nope this caused bugs with the rocket never despawning.
        //could fix, but there are more pressing issues 4-14-25

        /*
        if(ability != null)
        {
            Destroy(ability.gameObject);
            ability = null;
            GetComponent<SpriteRenderer>().sprite = null;
        }
        */
    }

}
