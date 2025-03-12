using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);


        DontDestroyOnLoad(gameObject);
    }

    public void OnHorizontal(InputAction.CallbackContext val)
    {
        HorizontalInput?.Invoke(val);
    }

    public void OnJump(InputAction.CallbackContext val)
    {
        JumpInput?.Invoke(val);
    }

    public void OnCrouch(InputAction.CallbackContext val)
    {
        CrouchInput?.Invoke(val);
    }

    public void OnRotateCW(InputAction.CallbackContext val)
    {
        RotateCWInput?.Invoke(val);
    }

    public void OnRotateCCW(InputAction.CallbackContext val)
    {
        RotateCCWInput?.Invoke(val);
    }

    public void OnAbilityNorth(InputAction.CallbackContext val)
    {
        AbilityNorthInput?.Invoke(val);
    }

    public void OnAbilityEast(InputAction.CallbackContext val)
    {
        AbilityEastInput?.Invoke(val);
    }

    public void OnAbilitySouth(InputAction.CallbackContext val)
    {
        AbilitySouthInput?.Invoke(val);
    }

    public void OnAbilityWest(InputAction.CallbackContext val)
    {
        AbilityWestInput?.Invoke(val);
    }

    public void OnSignatureAbility(InputAction.CallbackContext val)
    {
        SignatureAbilityInput?.Invoke(val);
    }

    public void OnPause(InputAction.CallbackContext val)
    {
        PauseInput?.Invoke(val);
    }
}