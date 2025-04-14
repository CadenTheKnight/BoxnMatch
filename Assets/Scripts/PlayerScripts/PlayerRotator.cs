using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager input;

    [Header("Settings")]
    //serializing makes it editable in inspector but not public to other classes
    [SerializeField] private float rotationTime;
    [SerializeField] private bool overrideBufferedInputs;

    [Header("Sockets")]
    [SerializeField] public AbilitySocket[] sockets;

    public AbilityDirection currDirection;
    private bool currentlyRotating;

    //-1 means CCW input queued
    //0 means nothing queued
    //1 means CW input queued
    private int rotateInputBuffer;

    private void Start()
    {
        currDirection = AbilityDirection.NORTH;
    }

    protected virtual void OnEnable()
    {
        EnableInputs();
        GetComponent<DamageableObject>().OnDeath += ClearBindings;
    }

    protected virtual void OnDisable()
    {
        DisableInputs();
        GetComponent<DamageableObject>().OnDeath -= ClearBindings;
    }

    //actually useful methods

    protected void UseAbility(AbilityDirection ab)
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

    protected void Rotate(int quarterCirclesCW)
    {
        //buffer input
        if (currentlyRotating)
        {
            //if already 1 input buffered, just ignore based on setting (?)
            if(rotateInputBuffer != 0 && !overrideBufferedInputs)
            {
                return;
            }

            //set input queue flag
            rotateInputBuffer = quarterCirclesCW;

            //kick out. aka essentially a big if-else
            return;
        }

        //tween rotation
        float rotation = -90f * quarterCirclesCW;

        //perform the actual rotation, set its tween, and add a delegate to handle end of rotation stuff
        transform.DORotate(new Vector3(0f, 0f, rotation),
            rotationTime, RotateMode.WorldAxisAdd).SetEase(Ease.OutCubic).onComplete += FinishRotation;

        //set flag
        currentlyRotating = true;

        //update direction
        currDirection.Rotate(quarterCirclesCW);

        //update sockets
        foreach (AbilitySocket socket in sockets)
        {
            socket.UpdateRotation(quarterCirclesCW);
        }
    }

    private void FinishRotation()
    {
        currentlyRotating = false;
        if(rotateInputBuffer != 0)
        {
            int tmp = rotateInputBuffer;
            rotateInputBuffer = 0;
            Rotate(tmp);
        }
    }

    private void ClearBindings(DamageableObject dObj)
    {
        for(int i = 0; i < sockets.Length; i++)
        {
            sockets[i].TrashAbilityBinding();
        }
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

    protected virtual void EnableInputs()
    {
        input.abilityNorthInput += UseAbility_N;
        input.abilityEastInput += UseAbility_E;
        input.abilitySouthInput += UseAbility_S;
        input.abilityWestInput += UseAbility_W;

        input.rotateCWInput += RotateCW;
        input.rotateCounterCWInput += RotateCCW;
    }

    protected virtual void DisableInputs()
    {
        input.abilityNorthInput -= UseAbility_N;
        input.abilityEastInput -= UseAbility_E;
        input.abilitySouthInput -= UseAbility_S;
        input.abilityWestInput -= UseAbility_W;

        input.rotateCWInput -= RotateCW;
        input.rotateCounterCWInput -= RotateCCW;
    }

}
