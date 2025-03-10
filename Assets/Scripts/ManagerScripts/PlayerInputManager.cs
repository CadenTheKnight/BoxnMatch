using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public void SetUpNetworkedPlayer(bool isLocalPlayer)
    {
        GetComponent<PlayerInput>().enabled = isLocalPlayer;
    }

    public bool inputsEnabled = false;

    private void EnableAllInputs()
    {
        inputsEnabled = true;
    }

    private void DisableAllInputs()
    {
        inputsEnabled = false;
    }

    public delegate void InputEvent(InputAction.CallbackContext val);

    public event InputEvent HorizontalInput;
    public event InputEvent JumpInput;
    public event InputEvent RotateCWInput;
    public event InputEvent RotateCCWInput;
    public event InputEvent AbilityNorthInput;
    public event InputEvent AbilityEastInput;
    public event InputEvent AbilitySouthInput;
    public event InputEvent AbilityWestInput;
    public event InputEvent SignatureAbilityInput;
    public event InputEvent CrouchInput;
    public event InputEvent PauseInput;
    public event InputEvent UnPauseInput;
    public event InputEvent OpenChatInput;
    public event InputEvent CloseChatInput;

    //events -------------------------------

    /*
     * These events will fire on every interaction with the input, including:
     *      the frame it is pressed/performed, every frame it is pressed, and the frame it is released
     * To differentiate between this in script, you can access the 'val' parameter of the delegate event.
     *      Because of this parameter, include the UnityEngine.InputSystem namespace. 
     * 
     * To subscribe/listen to these events, make a method with the same parameter signature
     * Then get a reference to this non-static component (must be non-static because we were doing multiplayer).
     * Then on the object's OnEnable, subscribe to the InputEvents defined above. 
     *      OnEnable is a Unity specific method, that calls after Awake, 
     *          when objects are instantiated or become active. Similar to Start. 
     *      OnDisable is similarly called when objects are destroyed or set inactive. 
     * 
     * It is important to unsubscribe from events using OnDisable so the delegate event does not try
     *      to invoke methods that no longer exist. 
     * 
     * Example: 

     private void OnEnable(){
            x_movementInput += MyXMoveMethod;
        }

     private void OnDisable(){
            x_movementInput -= MyXMoveMethod;
        }

     * 
     */

    //input actions ------------------------
    public void OnHorizontal(InputAction.CallbackContext val)
    {
        if (inputsEnabled) HorizontalInput?.Invoke(val);
    }

    public void OnJump(InputAction.CallbackContext val)
    {
        if (inputsEnabled) JumpInput?.Invoke(val);
    }

    public void OnRotateCW(InputAction.CallbackContext val)
    {
        if (inputsEnabled) RotateCWInput?.Invoke(val);
    }

    public void OnRotateCCW(InputAction.CallbackContext val)
    {
        if (inputsEnabled) RotateCCWInput?.Invoke(val);
    }

    public void OnAbilityNorth(InputAction.CallbackContext val)
    {
        if (inputsEnabled) AbilityNorthInput?.Invoke(val);
    }

    public void OnAbilityEast(InputAction.CallbackContext val)
    {
        if (inputsEnabled) AbilityEastInput?.Invoke(val);
    }

    public void OnAbilitySouth(InputAction.CallbackContext val)
    {
        if (inputsEnabled) AbilitySouthInput?.Invoke(val);
    }

    public void OnAbilityWest(InputAction.CallbackContext val)
    {
        if (inputsEnabled) AbilityWestInput?.Invoke(val);
    }

    public void OnSignatureAbility(InputAction.CallbackContext val)
    {
        if (inputsEnabled) SignatureAbilityInput?.Invoke(val);
    }

    public void OnCrouch(InputAction.CallbackContext val)
    {
        if (inputsEnabled) CrouchInput?.Invoke(val);
    }

    public void OnPause(InputAction.CallbackContext val)
    {
        if (inputsEnabled) PauseInput?.Invoke(val);
    }

    public void OnUnPause(InputAction.CallbackContext val)
    {
        if (inputsEnabled) UnPauseInput?.Invoke(val);
    }

    public void OnOpenChat(InputAction.CallbackContext val)
    {
        if (inputsEnabled) OpenChatInput?.Invoke(val);
    }

    public void OnCloseChat(InputAction.CallbackContext val)
    {
        if (inputsEnabled) CloseChatInput?.Invoke(val);
    }
}
