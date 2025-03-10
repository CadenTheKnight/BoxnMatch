using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private NetworkVariable<Vector3> networkPosition = new();
    private NetworkVariable<float> networkHorizontalInput = new();
    private NetworkVariable<bool> networkIsJumping = new(false);

    [Header("References")]
    [SerializeField] private PlayerInputManager input;

    [Header("Movement")]
    [SerializeField] private float BASE_SPEED = 5;
    private Rigidbody2D rb;

    float currentSpeed;

    //NEW
    [SerializeField] private float JUMP_FORCE = 5f;
    [SerializeField] private float groundCheckOffset = -0.5f;
    public Vector2 groundCheckSize = new(0.5f, 0.1f);
    public LayerMask groundLayer;
    public int jumpCount = 0;
    public int maxJumps = 2;
    bool isGrounded = false;


    //gathering input
    private float horizontalInput;
    private bool jumpInput;
    private bool crouchInput;

    public override void OnNetworkSpawn()
    {
        // Set up player based on ownership
        if (IsOwner)
        {
            // Local player setup
            // if (Camera.main != null && Camera.main.TryGetComponent<CameraFollow>(out var cameraFollow))
            // {
            //     cameraFollow.SetTarget(transform);
            // }

            // Initialize PlayerInputManager for the local player
            if (input != null)
            {
                input.SetUpNetworkedPlayer(true);
            }
        }
        else
        {
            // Remote player setup - disable local input processing
            if (input != null)
            {
                input.SetUpNetworkedPlayer(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = BASE_SPEED;
    }

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs();
    }

    public IEnumerator SpeedChange(float newSpeed, float timeInSecs)
    {
        currentSpeed = newSpeed;
        yield return new WaitForSeconds(timeInSecs);
        currentSpeed = BASE_SPEED;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsOwner)
        {
            Vector3 dir = new(horizontalInput, 0, 0);

            //switched it over to use a universal offset, because rotation
            //makes the groundCheckTransform unusable
            Vector3 checkPosition = new(transform.position.x, transform.position.y + groundCheckOffset);
            isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);

            //debugging
            /*
            Debug.Log("isGrounded: " + isGrounded);
            Debug.DrawLine(checkPosition + new Vector3(-0.05f, 0f), 
                checkPosition + new Vector3(0.05f, 0f), Color.red, 0.5f);
            */

            rb.velocity = new Vector2((dir * currentSpeed).x, rb.velocity.y);

            //NEW
            //jumping 1
            //Debug.Log(isGrounded + " " + rb.velocity.y);
            if (isGrounded && rb.velocity.y <= 0)
            {
                jumpCount = 0;
            }

            UpdateServerPositionServerRpc(transform.position, horizontalInput);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 15f);
        }
    }

    [ServerRpc]
    private void UpdateServerPositionServerRpc(Vector3 position, float hInput)
    {
        networkPosition.Value = position;
        networkHorizontalInput.Value = hInput;
    }



    //physics based things in fixedupdate
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            //Debug.Log("jumpInput: " + jumpInput);
            if (jumpInput && jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, JUMP_FORCE), ForceMode2D.Impulse);
                jumpCount++;
            }

            /*flush the jumpInput so it doesnt buffer in air.
                we could potentially delay this by input flush by a small timer (like 0.2 sec)
                to add a friendly jump buffer (?)
            */
            jumpInput = false;

            if (crouchInput)
            {
                rb.AddForce(new Vector2(0, -JUMP_FORCE), ForceMode2D.Impulse);

                crouchInput = false;
            }
        }
    }

    private void TryJump(InputAction.CallbackContext val)
    {
        if (val.performed)
        {
            jumpInput = true;

            if (IsOwner)
                JumpServerRpc();
        }
    }

    [ServerRpc]
    private void JumpServerRpc()
    {
        networkIsJumping.Value = true;
    }

    private void TryGoDown(InputAction.CallbackContext val)
    {
        crouchInput = true;
    }

    private void TryHorizontalMovement(InputAction.CallbackContext val)
    {
        horizontalInput = val.ReadValue<float>();
    }

    private void EnableInputs()
    {
        if (!IsLocalPlayer) return;

        input.JumpInput += TryJump;
        input.CrouchInput += TryGoDown;
        input.HorizontalInput += TryHorizontalMovement;
    }

    private void DisableInputs()
    {
        if (!IsLocalPlayer) return;

        input.JumpInput -= TryJump;
        input.CrouchInput -= TryGoDown;
        input.HorizontalInput -= TryHorizontalMovement;
    }
}