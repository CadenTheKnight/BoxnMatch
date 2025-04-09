using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CPUController : MonoBehaviour
{
    private CPUMovement cpuM;                            // The cpu movement handler (set in start)
    [SerializeField] private LayerMask groundLayer;     // Ground Layer Mask
    private Rigidbody2D rb;                             // CPU's Rigid Body Component (set in start)
    private CPURotator cr;

    [SerializeField] private float stopRange = 0.05f;   // The velocity +/- 0 that the CPU deems acceptable for stop function

    

    // Start is called before the first frame update
    void Start()
    {
        cpuM = GetComponent<CPUMovement>();
        rb = GetComponent<Rigidbody2D>();
        cr = GetComponent<CPURotator>();
        StartCoroutine(Idle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    /* ------------ Basic Movement Functions ------------ */



    private IEnumerator OptimalJumps()
    {

        cpuM.Jump();

        yield return new WaitForSeconds(0.1f);// Wait to ensure remaining jumps is updated

        if (cpuM.RemainingJumps() > 0) // If there is still a jump left then do a double jump
        {
            while(rb.velocity.y > 0.1) // Loop until the peak of the jump is reached
            {
                yield return new WaitForEndOfFrame(); // Delay to prevent overload
            }

            cpuM.Jump();
        }
    }

    private IEnumerator StopHorizontal()
    {
        while (rb.velocity.x > stopRange || rb.velocity.x < -stopRange) // Loop until suffieciently slowed/stopped
        {
            cpuM.HorizontalMove(-rb.velocity.x/cpuM.TOP_SPEED); // set horizontal movement in the opposite direction from current x velocity

            yield return new WaitForEndOfFrame(); // Delay to prvent overload
        }

        cpuM.HorizontalMove(0); // set move to 0 after
    }

    private IEnumerator MoveToEdge(int direction) // Moves to the edge of the platform it's on
    {
        // Set movement in direction
        cpuM.HorizontalMove(direction);

        // Loop to continuously check if the ray hits the ground layer
        RaycastHit2D ray;
        while (true)
        {
            // Cast a ray downwards with a slight horizontal direction based on the given direction
            ray = Physics2D.Raycast(transform.position, new Vector2(direction, -1), 2f, groundLayer);

            // If no collision with the ground, break out of the loop
            if (!ray.collider)
            {
                break;
            }

            // Added delay so it only checks every frame to prevent overloading
            yield return new WaitForEndOfFrame();
        }

        // Stop movement after finding edge
        Coroutine stop = StartCoroutine(StopHorizontal());
        yield return stop; // wait until stop is finished

    }



    /* ------------ Sensor Functions ------------ */



    private bool IsAbovePlatform()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.down, 50f, groundLayer);

        //Debug.Log(ray.collider);
        return ray.collider;
    }



    /* ------------ Finite State Machine Nodes ------------ */



    private IEnumerator Recover() // Recovers to a platform when falling
    {
        // Set the recovery direction to the center of the map
        int direction = (int)-(transform.position.x / (Mathf.Abs(transform.position.x)));

        // Set Move Direction
        cpuM.HorizontalMove(direction);

        // Jump to help get back to platforms
        Coroutine jumpRoutine = StartCoroutine(OptimalJumps());

        // Loop until there is a platform under the cpu
        while(!IsAbovePlatform())
        {
            yield return new WaitForEndOfFrame(); // Delay to prevent overloading
        }

        // Stop jump routine if already over a platform before it finishes
        StopCoroutine(jumpRoutine);

        // stops horizontal movement
        Coroutine stop = StartCoroutine(StopHorizontal());

        // fast falls to the platform
        cpuM.Crouch();

        // Transition to Idle after stopped
        yield return stop;
        StartCoroutine(Idle());
        
    }

    private IEnumerator Idle() // Moves back and forth on the current platform
    {
        int direction = -1;
        cpuM.HorizontalMove(direction);
        
        while (true)
        {
            // Move side to side switching directions at the edge of the platform
            RaycastHit2D ray;
            // Cast a ray downwards with a slight horizontal direction based on the given direction
            ray = Physics2D.Raycast(transform.position, new Vector2(direction, -1), 2f, groundLayer);

            // If no collision with the ground, break out of the loop
            if (!ray.collider)
            {
                direction = -direction;
                cpuM.HorizontalMove(direction);
            }
            


            // Check if not Above a platform
            if (!IsAbovePlatform())
            {
                StartCoroutine(Recover()); // Recover back to a platform
                break;
            }



            // Added delay so it only checks every frame to prevent overloading
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

}
