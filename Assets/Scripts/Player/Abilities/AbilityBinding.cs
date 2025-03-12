using UnityEngine;

public abstract class AbilityBinding : MonoBehaviour
{
    public abstract void Fire(AbilityDirection dir, PlayerRotator pr);
}
