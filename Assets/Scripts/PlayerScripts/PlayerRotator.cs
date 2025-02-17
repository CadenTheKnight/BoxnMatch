using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager input;

    [Header("Timing")]
    //serializing makes it editable in inspector but not public to other classes
    [SerializeField] private float rotationTime;

    [Header("Sockets")]
    [SerializeField] private AbilitySocket[] sockets;

    private AbilityDirection currDirection;

    private void Start()
    {
        currDirection = AbilityDirection.NORTH;
    }

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs();
    }

    //input methods
    public void UseAbility_N(InputAction.CallbackContext val)
    {
        if(val.performed)
            UseAbility(AbilityDirection.NORTH);
    }
    public void UseAbility_E(InputAction.CallbackContext val)
    {
        if (val.performed) UseAbility(AbilityDirection.EAST);
    }
    public void UseAbility_S(InputAction.CallbackContext val)
    {
        if (val.performed) UseAbility(AbilityDirection.SOUTH);
    }
    public void UseAbility_W(InputAction.CallbackContext val)
    {
        if (val.performed) UseAbility(AbilityDirection.WEST);
    }

    public void RotateCW(InputAction.CallbackContext val)
    {
        if (val.performed) Rotate(1);
    }

    public void RotateCCW(InputAction.CallbackContext val)
    {
        if (val.performed) Rotate(-1);
    }

    private void EnableInputs()
    {
        input.abilityNorthInput += UseAbility_N;
        input.abilityEastInput += UseAbility_E;
        input.abilitySouthInput += UseAbility_S;
        input.abilityWestInput += UseAbility_W;

        input.rotateCWInput += RotateCW;
        input.rotateCounterCWInput += RotateCCW;
    }

    private void DisableInputs()
    {
        input.abilityNorthInput -= UseAbility_N;
        input.abilityEastInput -= UseAbility_E;
        input.abilitySouthInput -= UseAbility_S;
        input.abilityWestInput -= UseAbility_W;

        input.rotateCWInput -= RotateCW;
        input.rotateCounterCWInput -= RotateCCW;
    }
    //actually useful methods

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
