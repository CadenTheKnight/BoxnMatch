
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //NEW
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager input;

    [Header("Movement")]
    [SerializeField] private float BASE_SPEED = 5;
    [SerializeField] private float TOP_SPEED = 20;
    private Rigidbody2D rb;

    float currentSpeed;

    //NEW
    [SerializeField] private float JUMP_FORCE = 5f;
    [SerializeField] private float groundCheckOffset = -0.5f;
    [SerializeField] private float FALL_FORCE = -5f;
    [SerializeField] private float sideDrag = 2f;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;
    public int jumpCount = 0;
    public int maxJumps = 2;
    bool isGrounded = false;

    public float forceAmount = 10f;
    private bool isFalling = false;

    //gathering input
    private float horizontalInput;
    private bool jumpInput;
    private bool crouchInput;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = BASE_SPEED;
    }

    private void OnEnable()
    {
        DamageableObject dobj = GetComponent<DamageableObject>();
        if (dobj != null) dobj.OnDeath += TrashInput;
        EnableInputs();
    }

    private void OnDisable()
    {
        DamageableObject dobj = GetComponent<DamageableObject>();
        if (dobj != null) dobj.OnDeath -= TrashInput;
        DisableInputs();
    }

    private void TrashInput(DamageableObject dobj)
    {
        horizontalInput = 0f;
        jumpInput = false;
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
        Vector3 dir = new Vector3(horizontalInput, 0, 0);

        //switched it over to use a universal offset, because rotation
        //makes the groundCheckTransform unusable
        Vector3 checkPosition = new Vector3(transform.position.x, transform.position.y + groundCheckOffset);



        isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);

        if (isGrounded)
        {
            jumpCount = 0;
        }
        if (Input.GetKey(KeyCode.LeftControl) && !isFalling)
        {
            rb.AddForce(new Vector2(0, FALL_FORCE), ForceMode2D.Impulse);
            isFalling = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isFalling = false;
        }
        //Debug.Log("isGrounded: " + isGrounded);
    }


    //physics based things in fixedupdate
    private void FixedUpdate()
    {

        Vector2 force = new Vector2(horizontalInput * forceAmount, 0);
        rb.AddForce(force, ForceMode2D.Impulse);


        if (Mathf.Abs(rb.velocity.x) > TOP_SPEED)
        {
            // Limit speed while keeping direction
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * TOP_SPEED, rb.velocity.y);

            Vector2 current_velocity = rb.velocity;
            current_velocity.x *= 1f - (sideDrag * Time.fixedDeltaTime);

            rb.velocity = current_velocity;
        }
        //Debug.Log("Velocity: " + rb.velocity.magnitude);

        //Debug.Log("jumpInput: " + jumpInput);
        if (jumpInput && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * JUMP_FORCE, ForceMode2D.Impulse);
            jumpCount++;
        }

        /*flush the jumpInput so it doesnt buffer in air.
            we could potentially delay this by input flush by a small timer (like 0.2 sec)
            to add a friendly jump buffer (?)
        */
        jumpInput = false;

        /*if (crouchInput)
        {
            rb.AddForce(new Vector2(0, FALL_FORCE), ForceMode2D.Impulse);

            crouchInput = false;
        }*/
    }

    private void TryJump(InputAction.CallbackContext val)
    {
        if (val.performed)
        {
            jumpInput = true;
        }
    }

    private void TryGoDown(InputAction.CallbackContext val)
    {
        crouchInput = true;
    }

    private void TryHorizontalMovement(InputAction.CallbackContext val)
    {
        horizontalInput = val.ReadValue<float>();
    }

    public void EnableInputs()
    {
        input.jumpInput += TryJump;
        input.crouchInput += TryGoDown;
        input.x_movementInput += TryHorizontalMovement;
    }

    public void DisableInputs()
    {
        input.jumpInput -= TryJump;
        input.crouchInput -= TryGoDown;
        input.x_movementInput -= TryHorizontalMovement;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerRotator tempPlayerRotator = collision.gameObject.GetComponent<PlayerRotator>();
        if (tempPlayerRotator != null)
        {
            // Get the contact points to check if we're on top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    // Apply small upward force to prevent sticking
                    jumpCount = 0;
                    rb.AddForce(Vector2.up * 0.1f, ForceMode2D.Impulse);
                    break;
                }
            }
        }
    }
}