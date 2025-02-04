using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerRotator : MonoBehaviour
{
    //serializing makes it editable in inspector but not public to other classes
    [SerializeField] private float rotationTime;

    [Header("Sockets")]
    [SerializeField] private AbilitySocket[] sockets;

    private AbilityDirection currDirection;

    private void Start()
    {
        currDirection = AbilityDirection.NORTH;
    }

    //tmp input-gathering. to be replaced later on
    private void Update()
    {
        //rotate CW
        if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }
        //rotate CCW
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }


        //ability inputs. once again, temporary.
        //will do this the good way later with unity's New input system
    }

    private void Rotate(int quarterCirclesCW)
    {
        //tween rotation
        float rotation = -90f * quarterCirclesCW;
        transform.DORotate(new Vector3(0f, 0f, rotation), 
            rotationTime, RotateMode.WorldAxisAdd).SetEase(Ease.OutCubic);

        //update direction
        currDirection.Rotate(quarterCirclesCW);

        //update sockets
        foreach(AbilitySocket socket in sockets)
        {
            socket.UpdateRotation(quarterCirclesCW);
        }
    }

}
