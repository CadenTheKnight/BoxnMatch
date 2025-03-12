// using UnityEngine;
// using DG.Tweening;
// using Unity.Netcode;
// using UnityEngine.InputSystem;
// using Assets.Scripts.Game.Managers;

// public class PlayerRotator : NetworkBehaviour
// {
//     [Header("References")]
//     [SerializeField] private PlayerInputManager input;

//     [Header("Settings")]
//     [SerializeField] private float rotationTime;
//     [SerializeField] private bool overrideBufferedInputs;

//     [Header("Sockets")]
//     [SerializeField] public AbilitySocket[] sockets;

//     private AbilityDirection currDirection;
//     private bool currentlyRotating;
//     private int rotateInputBuffer;

//     private NetworkVariable<int> networkCurrDirection = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
//     private NetworkVariable<Quaternion> networkRotation = new(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

//     private bool inputsEnabled = false;
//     private GameManager gameManager;

//     public override void OnNetworkSpawn()
//     {
//         base.OnNetworkSpawn();

//         if (!IsOwner)
//         {
//             currDirection = (AbilityDirection)networkCurrDirection.Value;
//             transform.rotation = networkRotation.Value;
//         }

//         networkCurrDirection.OnValueChanged += OnNetworkDirectionChanged;
//         networkRotation.OnValueChanged += OnNetworkRotationChanged;

//         if (IsOwner)
//         {
//             gameManager = GameManager.Instance;
//             gameManager.GameStateChanged += OnGameStateChanged;
//             if (gameManager.CurrentGameState == GameManager.GameState.RoundInProgress)
//                 EnablePlayerInput();
//             else
//                 DisablePlayerInput();
//         }
//     }

//     public override void OnNetworkDespawn()
//     {
//         base.OnNetworkDespawn();

//         networkCurrDirection.OnValueChanged -= OnNetworkDirectionChanged;
//         networkRotation.OnValueChanged -= OnNetworkRotationChanged;

//         if (IsOwner && gameManager != null)
//             gameManager.GameStateChanged -= OnGameStateChanged;

//         DisableInputs();
//     }

//     private void OnNetworkDirectionChanged(int previous, int current)
//     {
//         if (!IsOwner)
//         {

//             currDirection = (AbilityDirection)current;

//             int diff = current - previous;
//             if (diff != 0)
//             {
//                 if (diff > 2) diff -= 4;
//                 if (diff < -2) diff += 4;

//                 foreach (AbilitySocket socket in sockets)
//                     socket.UpdateRotation(diff);
//             }
//         }
//     }

//     private void OnNetworkRotationChanged(Quaternion previous, Quaternion current)
//     {
//         if (!IsOwner)
//             transform.rotation = current;
//     }

//     private void OnGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
//     {
//         if (IsOwner)
//         {
//             if (newState == GameManager.GameState.RoundInProgress)
//                 EnablePlayerInput();
//             else
//                 DisablePlayerInput();
//         }
//     }

//     private void EnablePlayerInput()
//     {
//         inputsEnabled = true;
//         EnableInputs();
//     }

//     private void DisablePlayerInput()
//     {
//         inputsEnabled = false;
//         DisableInputs();
//     }

//     private void OnEnable()
//     {
//         if (IsOwner)
//             EnableInputs();
//     }

//     private void OnDisable()
//     {
//         if (IsOwner)
//             DisableInputs();
//     }

//     private void UseAbility(AbilityDirection abDirection)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         int socketToFire = (int)abDirection - (int)currDirection;
//         if (socketToFire < 0) socketToFire += 4;
//         sockets[socketToFire % 4].FireAbility(this);

//         UseAbilityClientRpc((int)abDirection);
//     }


//     [ClientRpc]
//     private void UseAbilityClientRpc(int direction)
//     {
//         if (IsOwner) return;

//         AbilityDirection abDirection = (AbilityDirection)direction;
//         int socketToFire = (int)abDirection - (int)currDirection;
//         if (socketToFire < 0) socketToFire += 4;

//         sockets[socketToFire % 4].FireAbility(this);
//     }

//     private void Rotate(int quarterCirclesCW)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (currentlyRotating)
//         {
//             if (rotateInputBuffer != 0 && !overrideBufferedInputs)
//                 return;

//             rotateInputBuffer = quarterCirclesCW;
//             return;
//         }

//         float rotation = -90f * quarterCirclesCW;
//         transform.DORotate(new Vector3(0f, 0f, rotation), rotationTime, RotateMode.WorldAxisAdd)
//             .SetEase(Ease.OutCubic).onComplete += FinishRotation;

//         currentlyRotating = true;

//         int newDirectionValue = ((int)currDirection + quarterCirclesCW) % 4;
//         if (newDirectionValue < 0) newDirectionValue += 4;
//         currDirection = (AbilityDirection)newDirectionValue;

//         foreach (AbilitySocket socket in sockets)
//             socket.UpdateRotation(quarterCirclesCW);

//         if (IsOwner)
//         {
//             networkCurrDirection.Value = (int)currDirection;
//             networkRotation.Value = transform.rotation * Quaternion.Euler(0, 0, rotation);
//         }
//     }

//     private void FinishRotation()
//     {
//         currentlyRotating = false;

//         if (rotateInputBuffer != 0)
//         {
//             int buffer = rotateInputBuffer;
//             rotateInputBuffer = 0;
//             Rotate(buffer);
//         }
//     }

//     //input methods
//     public void UseAbility_N(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) UseAbility(AbilityDirection.NORTH);
//     }
//     public void UseAbility_E(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) UseAbility(AbilityDirection.EAST);
//     }
//     public void UseAbility_S(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) UseAbility(AbilityDirection.SOUTH);
//     }
//     public void UseAbility_W(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) UseAbility(AbilityDirection.WEST);
//     }

//     public void RotateCW(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) Rotate(1);
//     }

//     public void RotateCCW(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         if (val.performed) Rotate(-1);
//     }

//     private void EnableInputs()
//     {
//         input.abilityNorthInput += UseAbility_N;
//         input.abilityEastInput += UseAbility_E;
//         input.abilitySouthInput += UseAbility_S;
//         input.abilityWestInput += UseAbility_W;

//         input.rotateCWInput += RotateCW;
//         input.rotateCounterCWInput += RotateCCW;
//     }

//     private void DisableInputs()
//     {
//         input.abilityNorthInput -= UseAbility_N;
//         input.abilityEastInput -= UseAbility_E;
//         input.abilitySouthInput -= UseAbility_S;
//         input.abilityWestInput -= UseAbility_W;

//         input.rotateCWInput -= RotateCW;
//         input.rotateCounterCWInput -= RotateCCW;

//         currentlyRotating = false;
//         rotateInputBuffer = 0;
//     }

// }