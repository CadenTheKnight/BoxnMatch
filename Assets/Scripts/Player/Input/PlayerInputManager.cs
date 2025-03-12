using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.Game.Managers;

public class PlayerInputManager : MonoBehaviour
{
    public delegate void InputEvent(InputAction.CallbackContext val);

    [SerializeField] private bool menuInputsEnabled = true;
    [SerializeField] private bool gameInputsEnabled = true;


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

    private void Start()
    {
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
        EnableMenuInputs();
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameStateChanged -= OnGameStateChanged;
        DisableAllInputs();
    }


    private void OnGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        Debug.Log($"PlayerInputManager responding to game state change: {newState}");

        switch (newState)
        {
            case GameManager.GameState.RoundInProgress:
                EnableGameInputs();
                break;
            case GameManager.GameState.RoundEnding:
                DisableGameInputs();
                break;
            case GameManager.GameState.GameEnding:
                DisableGameInputs();
                break;
        }
    }

    public void DisableAllInputs()
    {
        menuInputsEnabled = false;
        gameInputsEnabled = false;
        Debug.Log("All inputs disabled");
    }

    public void EnableGameInputs()
    {
        gameInputsEnabled = true;
        Debug.Log("Game inputs enabled");
    }

    public void DisableGameInputs()
    {
        gameInputsEnabled = false;
        Debug.Log("Game inputs disabled");
    }

    public void EnableMenuInputs()
    {
        menuInputsEnabled = true;
        Debug.Log("Menu inputs enabled");
    }

    public void OnHorizontal(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) HorizontalInput?.Invoke(val);
    }

    public void OnJump(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) JumpInput?.Invoke(val);
    }

    public void OnCrouch(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) CrouchInput?.Invoke(val);
    }

    public void OnRotateCW(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) RotateCWInput?.Invoke(val);
    }

    public void OnRotateCCW(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) RotateCCWInput?.Invoke(val);
    }

    public void OnAbilityNorth(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) AbilityNorthInput?.Invoke(val);
    }

    public void OnAbilityEast(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) AbilityEastInput?.Invoke(val);
    }

    public void OnAbilitySouth(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) AbilitySouthInput?.Invoke(val);
    }

    public void OnAbilityWest(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) AbilityWestInput?.Invoke(val);
    }

    public void OnSignatureAbility(InputAction.CallbackContext val)
    {
        if (gameInputsEnabled) SignatureAbilityInput?.Invoke(val);
    }

    public void OnPause(InputAction.CallbackContext val)
    {
        if (menuInputsEnabled) PauseInput?.Invoke(val);
    }

    public void OnUnPause(InputAction.CallbackContext val)
    {
        if (menuInputsEnabled) UnPauseInput?.Invoke(val);
    }

    public void OnOpenChat(InputAction.CallbackContext val)
    {
        if (menuInputsEnabled) OpenChatInput?.Invoke(val);
    }

    public void OnCloseChat(InputAction.CallbackContext val)
    {
        if (menuInputsEnabled) CloseChatInput?.Invoke(val);
    }
}