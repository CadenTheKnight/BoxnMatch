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



    private IEnumerator OptimalJumps(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        float distance = direction.magnitude;

        // If the distance is too large, attempt a dynamic jump based on direction
        if (distance > 15f)
        {
            // Make a more powerful jump (adjust jump force or use a more direct trajectory)
            cpuM.Jump();
            yield return new WaitForSeconds(0.1f);

            // Double jump if necessary for larger distances
            if (cpuM.RemainingJumps() > 0 && rb.velocity.y < 0.5f)
            {
                cpuM.Jump();
            }
        }
        else
        {
            // Standard jump logic
            cpuM.Jump();
            yield return new WaitForSeconds(0.1f);
            if (cpuM.RemainingJumps() > 0 && rb.velocity.y < 0.5f)
            {
                cpuM.Jump(); // Double jump if the platform is still out of reach
            }
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

    private IEnumerator HandlePlatformGap(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        float heightDifference = target.y - transform.position.y;

        // If the gap is significant vertically and horizontally, prepare for a large jump
        if (Mathf.Abs(heightDifference) > 5f && Mathf.Abs(direction.x) > 10f)
        {
            if (cpuM.RemainingJumps() > 0)
            {
                cpuM.Jump();
                yield return new WaitForSeconds(0.15f); // Allow time for jump completion
                if (cpuM.RemainingJumps() > 0)
                {
                    cpuM.Jump(); // Double jump to cross larger distances
                }
            }
        }
        else
        {
            // Standard approach for closer platforms
            yield return StartCoroutine(NavigateToTargetPosition(target));
        }
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

    // Finds the nearest platform to the cpu
    private Vector2? FindNearestPlatform(Vector2 target)
    {
        float searchWidth = 40f; // Increase to allow wider platform search area
        float searchHeight = 30f; // Extend vertical range to consider higher platforms

        Collider2D[] hits = Physics2D.OverlapBoxAll(target, new Vector2(searchWidth, searchHeight), 0f, groundLayer);

        float minDist = Mathf.Infinity;
        Vector2? best = null;

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Vector2 platformCenter = hit.bounds.center;
                float dist = Vector2.Distance(transform.position, platformCenter);

                // Skip platforms that are too close (main platform zone)
                if (dist < 3f) continue; // Adjust to prevent magnet-like attraction to nearby platforms

                // Prioritize platforms that are within a reachable distance, considering height and horizontal distance
                float heightDiff = Mathf.Abs(platformCenter.y - transform.position.y);
                if (dist < minDist && heightDiff < 15f)  // Allow a larger height tolerance
                {
                    minDist = dist;
                    best = platformCenter;
                }
            }
        }

        return best;
    }

    // Navigates to a target position on the map safely between platforms
    private IEnumerator NavigateToTargetPosition(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.75f)
        {
            Vector2 direction = target - (Vector2)transform.position;

            int moveDir = Mathf.Abs(direction.x) > 0.1f ? (direction.x > 0 ? 1 : -1) : 0;
            cpuM.HorizontalMove(moveDir);

            // If the platform is above or we're falling, determine jump
            if (direction.y > 1f || !IsAbovePlatform())
            {
                if (cpuM.RemainingJumps() > 0)
                {
                    cpuM.Jump();
                    yield return new WaitForSeconds(0.1f); // Adjust wait time for double-jumping decision
                    if (cpuM.RemainingJumps() > 0 && rb.velocity.y < 0.5f)
                    {
                        cpuM.Jump(); // Double jump if needed
                    }
                }
            }
            // If the gap is large, attempt a dynamic jump trajectory to reach farther platforms
            else if (direction.y < -5f && Mathf.Abs(direction.x) > 5f)
            {
                // Attempt a larger jump to cover a bigger gap (considering double jump mechanics)
                cpuM.Jump();
                yield return new WaitForSeconds(0.15f); // Short delay to allow for double jump
                if (cpuM.RemainingJumps() > 0)
                {
                    cpuM.Jump();
                }
            }

            yield return new WaitForEndOfFrame();
        }

        cpuM.HorizontalMove(0); // Stop movement when close enough
        yield return StartCoroutine(StopHorizontal()); // Smooth stop
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


    private IEnumerator Idle()
    {
        while (true)
        {
            // Pick a random point in a reasonable range to scan for platforms
            Vector2 scanCenter = (Vector2)transform.position + new Vector2(Random.Range(-8f, 8f), Random.Range(-3f, 3f));
            Vector2? platform = FindNearestPlatform(scanCenter);

            // If a valid platform was found, decide when to jump to it
            if (platform.HasValue)
            {
                Vector2 targetPlatform = platform.Value;

                // Calculate distance to platform and height difference
                float distance = Vector2.Distance(transform.position, targetPlatform);
                float heightDifference = targetPlatform.y - transform.position.y;

                bool isCloseEnoughHorizontally = Mathf.Abs(targetPlatform.x - transform.position.x) <= 10f; // Horizontal proximity
                bool isJumpableHeight = heightDifference >= -1f && heightDifference <= 5f; // Height difference range
                bool isWithinReach = isCloseEnoughHorizontally && isJumpableHeight;

                // Only jump if the platform is within a reasonable horizontal distance and height difference
                if (isWithinReach)
                {
                    yield return StartCoroutine(NavigateToTargetPosition(targetPlatform));
                }
            }

            // Chill for a random short period before finding the next platform
            float idleTime = Random.Range(1f, 2.5f);
            float timer = 0f;

            while (timer < idleTime)
            {
                timer += Time.deltaTime;

                // Constantly check for danger or nearby orb
                Collider2D nearby = IsOrbNear();

                if (!IsAbovePlatform())
                {
                    yield return StartCoroutine(Recover());
                    break;
                }

                if ((numAttackAbilities + numDefenseAbilities + numRecoverAbilites < 4) && nearby && nearby.CompareTag("AbilityOrb"))
                {
                    yield return StartCoroutine(CollectOrb(nearby.gameObject));
                    break;
                }

                if (numAttackAbilities > 0 && IsPlayerNear())
                {
                    yield return StartCoroutine(Attack());
                    break;
                }

                yield return StartCoroutine(Defend());

                yield return new WaitForEndOfFrame();
            }
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
        yield return StartCoroutine(CountAbilities());

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
        float dangerAngleThreshold = 0.7f; // Cosine threshold to ensure it's mostly heading toward the CPU
        Collider2D[] threats = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D c in threats)
        {
            if (c.CompareTag("DamageObject"))
            {
                Rigidbody2D threatRb = c.attachedRigidbody;
                if (!threatRb) continue;

                Vector2 toSelf = (Vector2)transform.position - (Vector2)c.transform.position;
                Vector2 threatVel = threatRb.velocity;

                // Make sure the threat is heading toward the CPU
                float threatDirDot = Vector2.Dot(toSelf.normalized, threatVel.normalized);
                if (threatDirDot < dangerAngleThreshold) continue;

                // Determine the direction to defend based on the threat's velocity
                AbilityDirection blockDir = GetAbilityDirection(threatVel);

                // Dodge logic (if the threat is coming from above or below and a jump is available)
                if (cpuM.RemainingJumps() > 0 && Mathf.Abs(toSelf.y) > Mathf.Abs(toSelf.x))
                {
                    Debug.Log("DEFENSE: Dodging incoming attack (using jump)!");

                    // Perform an optimal jump to dodge the attack (calculate the direction to jump toward)
                    yield return StartCoroutine(OptimalJumps((Vector2)c.transform.position));
                    yield break;
                }

                // Block with shield if a shield is available
                if (FindAbilitySocket("Shield"))
                {
                    Debug.Log("DEFENSE: Blocking incoming attack with shield!");
                    yield return StartCoroutine(UseAbility("Shield", blockDir));
                    yield break;
                }

                // If falling or if no other defense is available, crouch
                if (rb.velocity.y < -0.5f) // Falling threshold, more aggressive crouch behavior when falling
                {
                    Debug.Log("DEFENSE: Crouching to avoid falling damage!");
                    cpuM.Crouch();
                    yield break;
                }

                // Evade horizontally if no jumps are available and a threat is too close
                if (Mathf.Abs(toSelf.x) < 2f && cpuM.RemainingJumps() == 0)
                {
                    Debug.Log("DEFENSE: Evading horizontally!");
                    int evadeDirection = toSelf.x > 0 ? -1 : 1;
                    cpuM.HorizontalMove(evadeDirection); // Move away from the threat
                    yield return new WaitForSeconds(0.3f); // Allow time for evasion
                    yield return StartCoroutine(StopHorizontal()); // Stop after evasion
                    yield break;
                }

                // If no specific defense logic applies, fallback to crouching
                Debug.Log("DEFENSE: Crouching as last resort!");
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
