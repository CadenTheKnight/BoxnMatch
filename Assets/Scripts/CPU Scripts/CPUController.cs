using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CPUController : MonoBehaviour
{
    private CPUMovement cpuM;                           // The cpu movement handler (set in start)
    private CPURotator cpuR;                            // The cpu rotator handler (set in start)
    [SerializeField] private LayerMask groundLayer;     // Ground Layer Mask
    private Rigidbody2D rb;                             // CPU's Rigid Body Component (set in start)

    [SerializeField] private float stopRange = 0.05f;   // The velocity +/- 0 that the CPU deems acceptable for stop function

    // Stores the number of abilities attached that are used in various states
    private int numAttackAbilities = 0;
    private int numDefenseAbilities = 0;
    private int numRecoverAbilites = 0;



    // Start is called before the first frame update
    void Start()
    {
        cpuM = GetComponent<CPUMovement>();
        cpuR = GetComponent<CPURotator>();
        rb = GetComponent<Rigidbody2D>();

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

    private Collider2D IsOrbNear()
    {

        // Capsule collider sizing
        Vector2 point = transform.position;
        Vector2 capsulePoint1 = point + new Vector2(5f, 1.5f);
        Vector2 capsulePoint2 = point + new Vector2(-5f, 1.5f);
        float radius = 2f;

        // Creating capsule sensing area
        Collider2D[] senseAreaCollisions = Physics2D.OverlapCapsuleAll((capsulePoint1 + capsulePoint2) * 0.5f, // center
                                                                        new Vector2(Vector2.Distance(capsulePoint1, capsulePoint2), radius * 2), // size
                                                                        CapsuleDirection2D.Horizontal,
                                                                        0f // angle
                                                                        );

        // Checks if an ability orb is in the capsule area
        foreach (Collider2D c in senseAreaCollisions)
        {
            if (c.CompareTag("AbilityOrb"))
            {
                Debug.Log("Found Ability Orb Nearby");
                return c; // Return ability orb as collider2d
            }
        }

        return null;
    }





    /* ------------ Ability Handling ------------ */



    // Counts the number of abilities currently attached and classifies them for ease of access
    private IEnumerator CountAbilities()
    {
        numAttackAbilities = 0;
        numDefenseAbilities = 0;
        numRecoverAbilites = 0;
        foreach(AbilitySocket s in cpuR.sockets)
        {
            // Classify the ability in the socket as attack, defense, or recovery
            if(s.ability.CompareTag("Fireball") || s.ability.CompareTag("Hammer") || s.ability.CompareTag("Laser") || s.ability.CompareTag("RemoteExplosive") || s.ability.CompareTag("Grapple"))
            {
                // Fireball, Hammer, Laser, Remote Explosive, Grappling Hook
                numAttackAbilities++;
            }
            else if (s.ability.CompareTag("Shield"))
            {
                // Shield
                numDefenseAbilities++;
            }
            else if (s.ability.CompareTag("Grapple") || s.ability.CompareTag("Rocket"))
            {
                // Grappling Hook, Rocket
                numDefenseAbilities++;
            }
        }

        yield break;
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

        // End this coroutine to return to previous Node
        yield return stop;
        
    }

    private IEnumerator Idle() // Moves back and forth on the current platform
    {
        int direction = -1;
        cpuM.HorizontalMove(direction);
        
        while (true)
        {
            // Check for important things nearby
            Collider2D nearby = IsOrbNear();

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
                Coroutine recover = StartCoroutine(Recover()); // Recover back to a platform
                yield return recover;
            }

            // Check if an orb is nearby
            if((numAttackAbilities + numDefenseAbilities + numRecoverAbilites < 4) && nearby && nearby.CompareTag("AbilityOrb"))
            {
                Coroutine collectOrb = StartCoroutine(CollectOrb(nearby.gameObject));
                yield return collectOrb;
            }


            // Added delay so it only checks every frame to prevent overloading
            yield return new WaitForEndOfFrame();
        }
    
    }

    private IEnumerator CollectOrb(GameObject abilityOrb)
    {
        // Navigate to orb
        Vector2 distanceFromOrb;
        while (abilityOrb) // Navigate to the orb until it is collected and therefore destroyed
        {
            distanceFromOrb = abilityOrb.transform.position - transform.position;

            // Horizontal Navigation
            cpuM.HorizontalMove(distanceFromOrb.x/Mathf.Abs(distanceFromOrb.x)); // Normalize the horizontal input to 1 or -1

            // If the Orb is above then jump to get it
            if(distanceFromOrb.x < 1.5 && distanceFromOrb.y > 0.5)
            {
                cpuM.Jump();
            }
            else if(rb.velocity.y < -0.1) // If falling off a platform to get it
            {
                cpuM.Jump();
            }

            // Calculate once per frame
            yield return new WaitForEndOfFrame();
        }


        // Handle ability
        Coroutine count = StartCoroutine(CountAbilities());
        yield return count;

        // Recover if not above a platform
        if(!IsAbovePlatform())
        {
             Coroutine recover = StartCoroutine(Recover());
            yield return recover;
        }

    }

    private IEnumerator Attack()
    {
        yield return null;
    }

    private IEnumerator Defend()
    {
        yield return null;
    }



    /* ------------ Displaying Sensor Fields ------------ */

    private void OnDrawGizmosSelected()
    {
        // Orb detection

        Vector2 point = transform.position;
        Vector2 capsulePoint1 = point + new Vector2(5f, 1.5f);
        Vector2 capsulePoint2 = point + new Vector2(-5f, 1.5f);
        float radius = 2f;

        Gizmos.color = Color.cyan;

        // Draw ends
        Gizmos.DrawWireSphere(capsulePoint1, radius);
        Gizmos.DrawWireSphere(capsulePoint2, radius);

        // Draw sides (a capsule is two circles + a rectangle between them)
        Vector2 dir = (capsulePoint2 - capsulePoint1).normalized;
        Vector2 perpendicular = new Vector2(-dir.y, dir.x); // get perpendicular vector

        Vector2 p1a = capsulePoint1 + perpendicular * radius;
        Vector2 p1b = capsulePoint1 - perpendicular * radius;
        Vector2 p2a = capsulePoint2 + perpendicular * radius;
        Vector2 p2b = capsulePoint2 - perpendicular * radius;

        Gizmos.DrawLine(p1a, p2a);
        Gizmos.DrawLine(p1b, p2b);
    }

}
