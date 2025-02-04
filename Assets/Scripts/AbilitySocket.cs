using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySocket : MonoBehaviour
{
    [SerializeField] private AbilityDirection socketDirection;

    public void UpdateRotation(int quarterCircles)
    {
        socketDirection.Rotate(quarterCircles);
    }
}
