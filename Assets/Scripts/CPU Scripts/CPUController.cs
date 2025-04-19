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

    // Checks if a player is near
    private bool IsPlayerNear()
    {

        // Circle collider sizing
        Vector2 point = transform.position;
        float radius = 10f;

        // Creating circle sensing area
        Collider2D[] senseAreaCollisions = Physics2D.OverlapCircleAll(point, radius);

        // Checks if a player is in the circle area
        foreach (Collider2D c in senseAreaCollisions)
        {
            if (c.CompareTag("Player"))
            {
                Debug.Log("Found Player Nearby");
                return c; // Return ability orb as collider2d
            }
        }

        return false;
    }

    private GameObject GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            if (p == gameObject) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }

        return closest;
    }

    private Vector2? FindNearestPlatform(Vector2 target)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            target + Vector2.up * 5f,
            new Vector2(3f, 1f),
            0f,
            Vector2.down,
            10f,
            groundLayer
        );

        float minDist = Mathf.Infinity;
        Vector2? best = null;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                float dist = Vector2.Distance(transform.position, hit.point);
                if (dist < minDist)
                {
                    minDist = dist;
                    best = hit.point;
                }
            }
        }

        return best;
    }

    private IEnumerator NavigateToTargetPosition(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 1.5f)
        {
            Vector2 direction = target - (Vector2)transform.position;

            int moveDir = Mathf.Abs(direction.x) > 0.1f ? (direction.x > 0 ? 1 : -1) : 0;
            cpuM.HorizontalMove(moveDir);

            // Decide if we need to jump
            if (direction.y > 1f || !IsAbovePlatform())
            {
                if (cpuM.RemainingJumps() > 0)
                {
                    cpuM.Jump();
                }
            }

            yield return new WaitForEndOfFrame();
        }

        cpuM.HorizontalMove(0);
        yield return StartCoroutine(StopHorizontal());
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
            // Check null cases (skip other checks if either is null)
            if (!s || !s.ability);

            // Classify the ability in the socket as attack, defense, or recovery
            else if (s.ability.CompareTag("Fireball") || s.ability.CompareTag("Hammer") || s.ability.CompareTag("Laser") || s.ability.CompareTag("RemoteExplosive") || s.ability.CompareTag("Grapple"))
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

    // Find Ability Socket that has the given ability
    private AbilitySocket FindAbilitySocket (string tag)
    {
        foreach (AbilitySocket s in cpuR.sockets)
        {
            if (s.ability && s.ability.CompareTag(tag))
            {
                Debug.Log("Found ability " + tag + " in slot " + s.socketDirection);
                return s;
            }
        }

        return null;
    }

    // Rotate selected ability to selected direction
    private IEnumerator RotateAbility(string tag, AbilityDirection dir)
    {
        // Find the socket that needs to be rotated
        AbilitySocket abilitySocket = FindAbilitySocket(tag);
        

        // If ability is found in a socket, rotate it to the given direction
        if(abilitySocket)
        {
            // Calculate spin direction
            int rotation = dir - abilitySocket.socketDirection;
            // Optimize spin direction
            if (rotation > 2) rotation = -(4 - rotation);
            else if (rotation < -2) rotation = -(-4 - rotation);

            // Spin to correct position
            cpuR.RotateProgrammatically(rotation);

            yield return new WaitUntil(() => abilitySocket.socketDirection == dir);
        }
        // Handle no ability matching
        else
        {
            Debug.Log("CPU ABILITY ERROR : No ability of that tag found to rotate");
        }

        yield break;
    }

    // Use Ability in the selected direction
    private IEnumerator UseAbility(string abilityTag, AbilityDirection dir)
    {

        Coroutine rotate = StartCoroutine(RotateAbility(abilityTag, dir));
        yield return rotate;
        yield return new WaitForSeconds(0.25f);

        cpuR.UseAbilityProgrammatically(dir);
        Coroutine count = StartCoroutine(CountAbilities());
        yield return count;
        
    }

    // Check if there if a launched fireball will hit a player
    private bool CanFireballHit(GameObject target, AbilityDirection dir)
{
    Vector2 startPos = GetAbilityOrigin(dir); ;
    Vector2 velocity;

    // Define velocity based on direction
    switch (dir)
    {
        case AbilityDirection.EAST:
            velocity = new Vector2(15f, 10f); // Arced right
            break;
        case AbilityDirection.WEST:
            velocity = new Vector2(-15f, 10f); // Arced left
            break;
        case AbilityDirection.NORTH:
            velocity = new Vector2(0f, 10f); // Straight up
            break;
        case AbilityDirection.SOUTH:
            velocity = new Vector2(0f, -10f); // Shot downward
            break;
        default:
            return false;
    }

    float gravity = Physics2D.gravity.y * 4f; // Account for gravity scale of fireball
    float timestep = 0.05f;
    float totalTime = 2f;

    for (float t = 0; t < totalTime; t += timestep)
    {
        Vector2 currentPos = startPos + velocity * t + 0.5f * t * t * new Vector2(0, gravity);
        Vector2 nextPos = startPos + velocity * (t + timestep) + 0.5f * Mathf.Pow(t + timestep, 2) * new Vector2(0, gravity);

        RaycastHit2D hit = Physics2D.Linecast(currentPos, nextPos);
        if (hit.collider)
        {
            if (hit.collider.gameObject == target)
                return true;

            return false; // Hit something else
        }
    }

    return false;
}


    // Converts Ability Direction to Vector
    private Vector2 DirectionToVector(AbilityDirection dir)
    {
        return dir switch
        {
            AbilityDirection.NORTH => Vector2.up,
            AbilityDirection.SOUTH => Vector2.down,
            AbilityDirection.EAST => Vector2.right,
            AbilityDirection.WEST => Vector2.left,
            _ => Vector2.zero,
        };
    }

    // Converts Vector to Ability Direction
    private AbilityDirection GetAbilityDirection(Vector2 direction)
    {
        direction.Normalize();

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? AbilityDirection.EAST : AbilityDirection.WEST;
        }
        else
        {
            return direction.y > 0 ? AbilityDirection.NORTH : AbilityDirection.SOUTH;
        }
    }

    // Gets the origin of the ability to be used based on 1 offset
    private Vector2 GetAbilityOrigin(AbilityDirection dir)
    {
        return (Vector2)transform.position + DirectionToVector(dir); // Offset by 1 unit in direction
    }








    /* ------------ Finite State Machine Nodes ------------ */



    private IEnumerator Recover()
    {
        Vector2? platform = FindNearestPlatform(Vector2.zero);
        if (platform.HasValue)
        {
            yield return StartCoroutine(NavigateToTargetPosition(platform.Value));
        }

        cpuM.Crouch();
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
                Debug.Log("RECOVER!");
                Coroutine recover = StartCoroutine(Recover()); // Recover back to a platform
                yield return recover;
            }

            // Check if an orb is nearby
            if((numAttackAbilities + numDefenseAbilities + numRecoverAbilites < 4) && nearby && nearby.CompareTag("AbilityOrb"))
            {
                Debug.Log("ORB!");
                Coroutine collectOrb = StartCoroutine(CollectOrb(nearby.gameObject));
                yield return collectOrb;
            }

            // Check if has attack ability and a player is nearby to attack
            if (numAttackAbilities > 0 && IsPlayerNear())
            {
                Coroutine attack = StartCoroutine(Attack());
                yield return attack;
            }

            // Check for hostile objects nearby and Dodge/Block if necessary
            Coroutine defend = StartCoroutine(Defend());
            yield return defend;




            // Added delay so it only checks every frame to prevent overloading
            yield return new WaitForEndOfFrame();
        }
    
    }

    private IEnumerator CollectOrb(GameObject abilityOrb)
    {

        // Shield abilities before grabbing
        int numShields = numDefenseAbilities;

        // Navigate to orb
        if (abilityOrb)
        {
            yield return StartCoroutine(NavigateToTargetPosition(abilityOrb.transform.position));
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

        // Use shields immediately if gotten just in case since they don't have time limit
        if (numShields < numDefenseAbilities)
        {
            Coroutine useAbility = StartCoroutine(UseAbility("Shield", AbilityDirection.NORTH));
            yield return useAbility;
        }
            

    }

    private IEnumerator Attack()
    {
        GameObject target = GetClosestPlayer();
        if (!target || !IsPlayerNear()) yield break;

        Vector2 toTarget = target.transform.position - transform.position;
        AbilityDirection dir = GetAbilityDirection(toTarget);
        Vector2 origin = GetAbilityOrigin(dir);

        // FIREBALL
        if (FindAbilitySocket("Fireball") && CanFireballHit(target, dir))
        {
            yield return StartCoroutine(UseAbility("Fireball", dir));
            yield break;
        }

        // HAMMER - check hit within 2.5 units in direction
        if (FindAbilitySocket("Hammer"))
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, DirectionToVector(dir), 2.5f, LayerMask.GetMask("Default"));
            if (hit.collider && hit.collider.gameObject == target)
            {
                yield return StartCoroutine(UseAbility("Hammer", dir));
                yield break;
            }
        }

        // LASER - raycast in direction, long range
        if (FindAbilitySocket("Laser"))
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, DirectionToVector(dir), 50f, LayerMask.GetMask("Default"));
            if (hit.collider && hit.collider.gameObject == target)
            {
                yield return StartCoroutine(UseAbility("Laser", dir));
                yield break;
            }
        }

        // REMOTE EXPLOSIVE - drop if close
        if (FindAbilitySocket("RemoteExplosive"))
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist < 2f)
            {
                yield return StartCoroutine(UseAbility("RemoteExplosive", AbilityDirection.SOUTH)); // drop down
                yield break;
            }
        }

        // GRAPPLE - check raycast to see if player is in hook line
        if (FindAbilitySocket("Grapple"))
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, DirectionToVector(dir), 10f, LayerMask.GetMask("Default"));
            if (hit.collider && hit.collider.gameObject == target)
            {
                yield return StartCoroutine(UseAbility("Grapple", dir));
                yield break;
            }
        }

        yield break;
    }

    private IEnumerator Defend()
    {
        float detectionRadius = 8f;
        float dangerAngleThreshold = 0.7f; // cosine threshold to ensure it's mostly heading toward CPU
        Collider2D[] threats = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D c in threats)
        {
            if (c.CompareTag("DamageObject"))
            {
                Rigidbody2D threatRb = c.attachedRigidbody;
                if (!threatRb) continue;

                Vector2 toSelf = (Vector2)transform.position - (Vector2)c.transform.position;
                Vector2 threatVel = threatRb.velocity;

                // Make sure it's flying toward the CPU
                float threatDirDot = Vector2.Dot(toSelf.normalized, threatVel.normalized);
                if (threatDirDot < dangerAngleThreshold) continue;

                // Pick a direction to defend
                AbilityDirection blockDir = GetAbilityDirection(threatVel);

                // Dodge logic
                if (cpuM.RemainingJumps() > 0)
                {
                    Debug.Log("DEFENSE: Dodging incoming attack!");
                    yield return StartCoroutine(OptimalJumps());
                    yield break;
                }

                // Block logic
                if (FindAbilitySocket("Shield"))
                {
                    Debug.Log("DEFENSE: Blocking incoming attack with shield!");
                    yield return StartCoroutine(UseAbility("Shield", blockDir));
                    yield break;
                }

                // Crouch if falling or no other defense
                Debug.Log("DEFENSE: Crouching to avoid!");
                cpuM.Crouch();
                yield break;
            }
        }

        yield break;
    }




    /* ------------ Displaying Sensor Fields ------------ */

    private void OnDrawGizmosSelected()
    {
        DrawFireballArc();
        DrawOrbDetectionArea();
    }

    private void DrawFireballArc()
    {
        Vector2 velocity = new(15f, 10f);
        float gravity = Physics2D.gravity.y * 4f;
        float timestep = 0.05f;
        float totalTime = 2f;

        Vector2 offset = DirectionToVector(AbilityDirection.EAST); // Offset vector based on ability direction
        Vector2 previousPoint = (Vector2)transform.position + offset; // Apply 1-unit offset

        for (float t = timestep; t < totalTime; t += timestep)
        {
            Vector2 nextPoint = (Vector2)transform.position + velocity * t + 0.5f * t * t * new Vector2(0, gravity);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    private void DrawOrbDetectionArea()
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
        Vector2 perpendicular = new(-dir.y, dir.x); // get perpendicular vector

        Vector2 p1a = capsulePoint1 + perpendicular * radius;
        Vector2 p1b = capsulePoint1 - perpendicular * radius;
        Vector2 p2a = capsulePoint2 + perpendicular * radius;
        Vector2 p2b = capsulePoint2 - perpendicular * radius;

        Gizmos.DrawLine(p1a, p2a);
        Gizmos.DrawLine(p1b, p2b);
    }



}
