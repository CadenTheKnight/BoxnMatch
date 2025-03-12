using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerRotator : NetworkBehaviour
{

    [Header("Settings")]
    //serializing makes it editable in inspector but not public to other classes
    [SerializeField] private float rotationTime;
    [SerializeField] private bool overrideBufferedInputs;

    [Header("Sockets")]
    [SerializeField] public AbilitySocket[] sockets;


    private PlayerInputManager input;
    private AbilityDirection currDirection;
    private bool currentlyRotating;

    //-1 means CCW input queued
    //0 means nothing queued
    //1 means CW input queued
    private int rotateInputBuffer;

    private void Start()
    {
        currDirection = AbilityDirection.NORTH;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log($"PlayerRotator spawned. IsOwner: {IsOwner}, IsServer: {IsServer}");

        if (IsOwner)
        {
            SetupInputManager();
            EnableInputs();
        }
    }

    private void SetupInputManager()
    {
        input = PlayerInputManager.Instance;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner)
            DisableInputs();
    }

    private void OnDisable()
    {
        if (IsOwner)
            DisableInputs();
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

        int socketToFire = (int)ab - (int)currDirection;
        if (socketToFire < 0) socketToFire += 4;
        //Debug.Log(socketToFire);

        sockets[socketToFire % 4].FireAbility(this);
    }

    private void Rotate(int quarterCirclesCW)
    {
        //buffer input
        if (currentlyRotating)
        {
            //if already 1 input buffered, just ignore based on setting (?)
            if (rotateInputBuffer != 0 && !overrideBufferedInputs)
                return;

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
            socket.UpdateRotation(quarterCirclesCW);
    }

    private void FinishRotation()
    {
        currentlyRotating = false;
        if (rotateInputBuffer != 0)
        {
            int tmp = rotateInputBuffer;
            rotateInputBuffer = 0;
            Rotate(tmp);
        }
    }

    public void UseAbility_N(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) UseAbility(AbilityDirection.NORTH);
    }

    public void UseAbility_E(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) UseAbility(AbilityDirection.EAST);
    }

    public void UseAbility_S(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) UseAbility(AbilityDirection.SOUTH);
    }

    public void UseAbility_W(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) UseAbility(AbilityDirection.WEST);
    }

    public void RotateCW(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) Rotate(1);
    }

    public void RotateCCW(InputAction.CallbackContext val)
    {
        if (!IsOwner) return;

        if (val.performed) Rotate(-1);
    }

    private void EnableInputs()
    {
        if (input == null) return;

        DisableInputs();

        input.AbilityNorthInput += UseAbility_N;
        input.AbilityEastInput += UseAbility_E;
        input.AbilitySouthInput += UseAbility_S;
        input.AbilityWestInput += UseAbility_W;
        input.RotateCWInput += RotateCW;
        input.RotateCCWInput += RotateCCW;
    }

    private void DisableInputs()
    {
        if (input == null) return;

        input.AbilityNorthInput -= UseAbility_N;
        input.AbilityEastInput -= UseAbility_E;
        input.AbilitySouthInput -= UseAbility_S;
        input.AbilityWestInput -= UseAbility_W;
        input.RotateCWInput -= RotateCW;
        input.RotateCCWInput -= RotateCCW;
    }
}