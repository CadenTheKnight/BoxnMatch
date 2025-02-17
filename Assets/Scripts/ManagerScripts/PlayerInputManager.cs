using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public delegate void InputEvent(InputAction.CallbackContext val);

    public event InputEvent x_movementInput;
    public event InputEvent jumpInput;
    public event InputEvent rotateCWInput;
    public event InputEvent rotateCounterCWInput;
    public event InputEvent abilityNorthInput;
    public event InputEvent abilityEastInput;
    public event InputEvent abilitySouthInput;
    public event InputEvent abilityWestInput;
    public event InputEvent signatureAbilityInput;
    public event InputEvent crouchInput;
    public event InputEvent pauseInput;

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

    public void XMovement(InputAction.CallbackContext val)
    {
        x_movementInput?.Invoke(val);
    }
    public void Jump(InputAction.CallbackContext val)
    {
        jumpInput?.Invoke(val);
    }
    public void RotateCW(InputAction.CallbackContext val)
    {
        rotateCWInput?.Invoke(val);
    }
    public void RotateCounterCW(InputAction.CallbackContext val)
    {
        rotateCounterCWInput?.Invoke(val);
    }
    public void AbilityNorth(InputAction.CallbackContext val)
    {
        abilityNorthInput?.Invoke(val);
    }

    public void AbilityEast(InputAction.CallbackContext val)
    {
        abilityEastInput?.Invoke(val);
    }
    public void AbilitySouth(InputAction.CallbackContext val)
    {
        abilitySouthInput?.Invoke(val);
    }
    public void AbilityWest(InputAction.CallbackContext val)
    {
        abilityWestInput?.Invoke(val);
    }
    public void SignatureAbility(InputAction.CallbackContext val)
    {
        signatureAbilityInput?.Invoke(val);
    }
    public void Crouch(InputAction.CallbackContext val)
    {
        crouchInput?.Invoke(val);
    }
    public void Pause(InputAction.CallbackContext val)
    {
        pauseInput?.Invoke(val);
    }
}
