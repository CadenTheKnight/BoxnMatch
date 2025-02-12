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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UseAbility(AbilityDirection.NORTH);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UseAbility(AbilityDirection.EAST);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UseAbility(AbilityDirection.SOUTH);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UseAbility(AbilityDirection.WEST);
        }
    }

    private void UseAbility(AbilityDirection ab)
    {
        /*access the socket that is at that direction, 
         * by using its index in the socket array.
         * relies on the ordering in the inspector to be
         * N, E, S, W 
         */

        //so if my head is facing the West, if I use UP,
        //2nd socket should fire (the East socket, currently north
        //my dir: 3. Button pressed: 0. Should fire: 1

        int socketToFire = ((int)ab - (int)currDirection);
        if (socketToFire < 0) socketToFire += 4;
        //Debug.Log(socketToFire);

        sockets[socketToFire % 4].FireAbility(this);
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
