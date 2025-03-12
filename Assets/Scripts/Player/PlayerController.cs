// using UnityEngine;
// using Unity.Netcode;
// using UnityEngine.InputSystem;
// using Assets.Scripts.Game.Managers;

// public class PlayerController : NetworkBehaviour
// {
//     [Header("References")]
//     [SerializeField] private Rigidbody2D rb;
//     [SerializeField] private GameManager gameManager;
//     [SerializeField] private PlayerInputManager input;


//     [Header("Movement")]
//     [SerializeField] private int MAX_JUMPS = 2;
//     [SerializeField] private float BASE_SPEED = 5;
//     [SerializeField] private float TOP_SPEED = 20;
//     [SerializeField] private float JUMP_FORCE = 5f;
//     [SerializeField] private float FALL_FORCE = 5f;
//     [SerializeField] private float groundCheckOffset = -0.5f;

//     public Vector2 groundCheckSize = new(0.5f, 0.1f);
//     public LayerMask groundLayer;
//     public int jumpCount = 0;
//     public int maxJumps = 2;
//     bool isGrounded = false;
//     public float forceAmount = 10f;
//     private float horizontalInput;
//     private bool jumpInput;
//     private bool crouchInput;

//     private NetworkVariable<Vector3> networkPosition = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
//     // private NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

//     private bool inputsEnabled = false;

//     public override void OnNetworkSpawn()
//     {
//         Debug.Log($"Player spawned - IsOwner: {IsOwner}, NetworkObjectId: {NetworkObjectId}");

//         if (IsOwner) gameManager.GameStateChanged += OnGameStateChanged;
//     }

//     public override void OnNetworkDespawn()
//     {
//         if (IsOwner) gameManager.GameStateChanged -= OnGameStateChanged;
//     }

//     private void OnGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
//     {
//         if (IsOwner)
//         {
//             Debug.Log($"Game state changed from {previousState} to {newState}");

//             if (newState == GameManager.GameState.RoundInProgress)
//                 EnablePlayerInput();
//             else
//                 DisablePlayerInput();
//         }
//     }

//     private void EnablePlayerInput()
//     {
//         Debug.Log("Enabling player input");

//         inputsEnabled = true;
//         if (IsOwner) EnableInputs();
//     }

//     private void DisablePlayerInput()
//     {
//         Debug.Log("Disabling player input");

//         inputsEnabled = false;
//         if (IsOwner) DisableInputs();
//     }

//     private void Update()
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         Vector3 checkPosition = new(transform.position.x, transform.position.y + groundCheckOffset);
//         isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);

//         if (isGrounded)
//             jumpCount = 0;

//         transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
//     }

//     private void FixedUpdate()
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         Vector2 force = new(horizontalInput * forceAmount, 0);
//         rb.AddForce(force, ForceMode2D.Impulse);

//         if (Mathf.Abs(rb.velocity.x) > TOP_SPEED)
//             rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * TOP_SPEED, rb.velocity.y);

//         if (jumpInput && jumpCount < maxJumps)
//         {
//             rb.velocity = new Vector2(rb.velocity.x, 0);
//             rb.AddForce(Vector2.up * JUMP_FORCE, ForceMode2D.Impulse);
//             jumpCount++;
//         }

//         jumpInput = false;

//         if (crouchInput)
//         {
//             rb.AddForce(new Vector2(0, -FALL_FORCE), ForceMode2D.Impulse);
//             crouchInput = false;
//         }

//         networkPosition.Value = transform.position;
//     }

//     private void TryJump(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         jumpInput = true;
//     }

//     private void TryGoDown(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         crouchInput = true;
//     }

//     private void TryHorizontalMovement(InputAction.CallbackContext val)
//     {
//         if (!IsOwner || !inputsEnabled) return;

//         horizontalInput = val.ReadValue<float>();
//     }

//     private void EnableInputs()
//     {
//         if (!IsOwner || input == null) return;

//         input.jumpInput += TryJump;
//         input.crouchInput += TyGoDown;
//         input.x_movementInput += TryHorizontalMovement;
//     }

//     private void DisableInputs()
//     {
//         if (!IsOwner || input == null) return;

//         input.jumpInput -= TryJump;
//         input.crouchInput -= TryGoDown;
//         input.x_movementInput -= TryHorizontalMovement;

//         horizontalInput = 0;
//         jumpInput = false;
//         crouchInput = false;
//     }
// }