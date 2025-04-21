using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CPUController : MonoBehaviour
{
    private CPUMovement cpuM;                               // The cpu movement handler (set in start)
    private CPURotator cpuR;                                // The cpu rotator handler (set in start)
    [SerializeField] private LayerMask groundLayer;         // Ground Layer Mask
    private Rigidbody2D rb;                                 // CPU's Rigid Body Component (set in start)

    [SerializeField] private float stopRange = 0.05f;       // The velocity +/- 0 that the CPU deems acceptable for stop function
    [SerializeField] private float targetXrange = 0.25f;    // The Range in with the CPU will navigate to for its target X position

    private Vector2 previousPosition;                       // Stores previous position for checking if the cpu is stuck
    private int stuckCheck;                                 // Stores consecutive checks of no movement
    private bool deathFlag = false;                         // Keeps track of death handling

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
        previousPosition = transform.position;

        StartCoroutine(Idle());
    }

    private void Update()
    {
        //Debug.Log(stuckCheck);
        // Check if the CPU is stationary for too long and therefore stuck
        if (previousPosition.Equals(transform.position))
        {
            stuckCheck++;
        }
        else
        {
            stuckCheck = 0;
            previousPosition = transform.position;
        }

        if (stuckCheck > 200)
        {
            stuckCheck = 0;
            Debug.Log("Attempt unstuck procedure");
            StopAllCoroutines();
            StartCoroutine(Idle());
        }

        // Reset on death
        if(!deathFlag && GetComponent<DamageableObject>().damageModifier == 0)
        {
            deathFlag = true;
            StopAllCoroutines();
            StartCoroutine(Idle());
        }
        else if (deathFlag && GetComponent<DamageableObject>().damageModifier != 0)
        {
            deathFlag = false;
        }
        
    }



    /* ------------ Basic Movement Functions ------------ */



    private IEnumerator OptimalJumps()
    {
        cpuM.Jump();

        // Double jump if able
        if (cpuM.RemainingJumps() > 0)
        {
            // Wait for optimally timed double jump
            while (rb.velocity.y > 0.5f)
            {
                yield return new WaitForEndOfFrame();
            }
            cpuM.Jump();
        }

        yield return null;
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

    // Finds the nearest platform to the target
    private GameObject FindNearestPlatform(Vector2 target)
    {
        float searchWidth = 30f; 
        float searchHeight = 30f;
        float searchAbove = 4f; // Only search for platforms that a double jumpable

        Collider2D[] hits = Physics2D.OverlapBoxAll(target - new Vector2(0, (searchHeight/2) - searchAbove), new Vector2(searchWidth, searchHeight), 0f, groundLayer);
        //Debug.Log("hits : " + hits);

        float minDist = Mathf.Infinity;
        GameObject best = null;

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Vector2 platformCenter = hit.bounds.center;
                float dist = Vector2.Distance(transform.position, platformCenter);

                // Choose the platform closests to the CPU player
                if (dist < minDist)
                {
                    minDist = dist;
                    best = hit.gameObject;
                }
            }
        }

        return best;
    }

    // Finds a random platform reachable from the current platform including the current platform
    private GameObject FindRandomPlatform()
    {
        GameObject currPlatform = CurrentPlatform();

        float searchWidth = 5f; 
        float searchHeight = 30f; 
        float searchAbove = 4f; // Only search for platforms that a double jumpable

        Vector2 max = currPlatform.GetComponent<Collider2D>().bounds.max;
        Vector2 min = currPlatform.GetComponent<Collider2D>().bounds.min;
        min.y += 1; // Adjust the min y to be more accurate to the top of the platform

        ArrayList platforms = new();

        // Add all platforms reachable from the max side of the current platform
        Collider2D[] hits = Physics2D.OverlapBoxAll(max - new Vector2(0, (searchHeight / 2) - searchAbove), new Vector2(searchWidth, searchHeight), 0f, groundLayer);
        foreach (Collider2D hit in hits)
        {
            platforms.Add(hit.gameObject);
        }

        // Add all platforms reachable from the min side of the current platform
        hits = Physics2D.OverlapBoxAll(min - new Vector2(0, (searchHeight / 2) - searchAbove), new Vector2(searchWidth, searchHeight), 0f, groundLayer);
        foreach (Collider2D hit in hits)
        {
            platforms.Add(hit.gameObject);
        }

        //Debug.Log("platforms : ");
        //foreach (Object platform in platforms)
        //{
        //    Debug.Log(platform.name);
        //}

        GameObject rand = (GameObject)platforms[Random.Range(0,platforms.Count)];
        Debug.Log("Random Platform of " + platforms.Count + " platforms : " + rand);

        return rand;
    }

    // Navigates to a target position on the map safely between platforms
    private IEnumerator NavigateToTargetPosition(Vector2 target)
    {
        Debug.Log("Navigate to " + target);
        bool isJumping = false;
        Coroutine jumping = null;
        while (!(Mathf.Abs(transform.position.x - target.x) < targetXrange))
        {
            Vector2 direction = target - (Vector2)transform.position;

            int moveDir = Mathf.Abs(direction.x) > 0.1f ? (direction.x > 0 ? 1 : -1) : 0;

            // If we're falling, jump
            if (!IsAbovePlatform())
            {
                yield return StartCoroutine(OptimalJumps());
            }

            // Avoid the edge of platforms to get on them
            Collider2D platformEdge = Physics2D.OverlapBox(transform.position, new Vector2(2f,0.8f), 0, groundLayer);
            if(platformEdge && platformEdge.transform.position.x < transform.position.x)
            {
                moveDir = 1;
            }
            else if(platformEdge && platformEdge.transform.position.x > transform.position.x)
            {
                moveDir = -1;
            }

            cpuM.HorizontalMove(moveDir);

            // if the target is much above the bottom of the CPU then double jump when close enough
            if (!isJumping && target.y - transform.position.y > 1.5f && target.x - transform.position.x < (7.5 + (4 - Mathf.Abs(target.y - transform.position.y))))
            {
                isJumping = true;
                jumping = StartCoroutine(OptimalJumps());
            }
            // if the target is above the bottom of the CPU then jump when close enough
            else if (!isJumping && target.y - transform.position.y > -0.5f && Mathf.Abs(target.x - transform.position.x) < (3.5 + (1.5 - Mathf.Abs(target.y - transform.position.y))/2))
            {
                isJumping = true;
                jumping = StartCoroutine(OptimalJumps());
            }

            if(cpuM.jumpCount > 1)
            {
                isJumping = false;
                if (jumping != null) StopCoroutine(jumping);
            }

            yield return new WaitForEndOfFrame();
        }

        if(jumping != null) StopCoroutine(jumping);
        yield return StartCoroutine(StopHorizontal()); // Smooth stop
        Debug.Log("Arrived at " + target);
    }

    private GameObject CurrentPlatform()
    {
        RaycastHit2D ray1 = Physics2D.Raycast((Vector2)transform.position + new Vector2(0.5f,0), Vector2.down, 50f, groundLayer);
        RaycastHit2D ray2 = Physics2D.Raycast((Vector2)transform.position - new Vector2(0.5f,0), Vector2.down, 50f, groundLayer);

        if (ray1) return ray1.collider.gameObject;
        else if (ray2) return ray2.collider.gameObject;
        else return FindNearestPlatform(transform.position);
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

        yield return null;
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
        if(abilitySocket && abilitySocket.socketDirection != dir)
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

        yield return null;
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
        GameObject platform = FindNearestPlatform(transform.position);

        // Use a rocket if no platform is found
        if(!platform)
        {
            yield return StartCoroutine(UseAbility("Rocket", AbilityDirection.SOUTH));
        }
        yield return new WaitForSeconds(1);

        // Check for platform again
        platform = FindNearestPlatform(transform.position);

        // Navigate to nearest platform
        if (platform)
        {
            Vector2 targetPosition = Vector2.zero;
            // Navigate to the closest side of the platform
            if(transform.position.x > platform.transform.position.x)
            {
                targetPosition = platform.GetComponent<Collider2D>().bounds.max + new Vector3(-1, 0, 0);
                
            }
            else if (transform.position.x < platform.transform.position.x)
            {
                targetPosition = platform.GetComponent<Collider2D>().bounds.min + new Vector3(1, 1, 0);
            }

            // If the distance away from the 
            float xDif = Mathf.Abs(targetPosition.x - transform.position.x);
            float yDif = transform.position.y - targetPosition.y;
            if (xDif > yDif * 2)
            {
                yield return StartCoroutine(UseAbility("Rocket", AbilityDirection.SOUTH));
            }

            yield return StartCoroutine(NavigateToTargetPosition(targetPosition));

        }

        cpuM.Crouch();
    }


    private IEnumerator Idle()
    {
        Coroutine navigation = null;
        Vector2? targetPosition = null;
        bool navigationActive = false;
        yield return new WaitForEndOfFrame();
        //Debug.Log(GetComponent<DamageableObject>().currLives);
        while (GetComponent<DamageableObject>().currLives > 0)
        {

            //Debug.Log("Idle Loop");
            if(!targetPosition.HasValue || (Mathf.Abs(transform.position.x - targetPosition.Value.x) < targetXrange*2 && Mathf.Abs(transform.position.y - targetPosition.Value.y) < 2f))
            {
                GameObject platform = FindRandomPlatform();
                //Debug.Log("random platform " + platform);

                // If the navigation is to the current platform, choose a random point on that platform
                if (platform == CurrentPlatform())
                {
                    //Debug.Log("random platform " + platform.name);
                    Vector2 platformMin = platform.GetComponent<Collider2D>().bounds.min;
                    platformMin.y += 1; //adjust to top of the platform
                    platformMin.x += 1f; // adjust away form the edge a bit
                    Vector2 platformMax = platform.GetComponent<Collider2D>().bounds.max;
                    platformMax.x -= 1; // adjust away from the edge a bit
                    targetPosition = Vector2.Lerp(platformMin, platformMax, Random.value); // chooses random point on the platform as the target

                    navigationActive = true;
                    navigation = StartCoroutine(NavigateToTargetPosition(targetPosition.Value));
                }
                else if(platform)
                {
                    Vector2 platformMin = platform.GetComponent<Collider2D>().bounds.min;
                    platformMin.y += 1; //adjust to top of the platform
                    platformMin.x += 1f; // adjust away form the edge a bit
                    Vector2 platformMax = platform.GetComponent<Collider2D>().bounds.max;
                    platformMax.x -= 1; // adjust away from the edge a bit

                    // Navigate to the closest side of the platform
                    navigationActive = true;
                    if (platform.transform.position.x > transform.position.x)
                    {
                        navigation = StartCoroutine(NavigateToTargetPosition(platformMin));
                    }
                    else
                    {
                        navigation = StartCoroutine(NavigateToTargetPosition(platformMax));
                    }
                }
            }
            else if(!navigationActive && !(Mathf.Abs(transform.position.x - targetPosition.Value.x) < targetXrange*2 && Mathf.Abs(transform.position.y - targetPosition.Value.y) < 2f))
            {
                navigationActive = true;
                navigation = StartCoroutine(NavigateToTargetPosition(targetPosition.Value));
            }

            yield return navigation;
            
            // Check for transitions
            Collider2D nearby = IsOrbNear();

            if (!IsAbovePlatform())
            {
                navigationActive = false;
                StopCoroutine(navigation);
                yield return StartCoroutine(Recover());
            }

            if ((numAttackAbilities + numDefenseAbilities + numRecoverAbilites < 4) && nearby && nearby.CompareTag("AbilityOrb"))
            {
                navigationActive = false;
                StopCoroutine(navigation);
                yield return StartCoroutine(CollectOrb(nearby.gameObject));
            }

            if (numAttackAbilities > 0 && IsPlayerNear())
            {
                navigationActive = false;
                StopCoroutine(navigation);
                yield return StartCoroutine(Attack()); 
            }

            yield return StartCoroutine(Defend());

            yield return new WaitForEndOfFrame();

        }

        yield break;
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
                    yield return StartCoroutine(OptimalJumps());
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
