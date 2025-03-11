using UnityEngine;
using UnityEngine.UIElements;

public class AbilityOrbHandler : MonoBehaviour
{
    [SerializeField] float swingWidth = 1f;       // Width of the swing
    [SerializeField] float swingDepth = 0.1f;     // Depth of the swing
    [SerializeField] float movementSpeed = 1f;    // Speed of movement
    [SerializeField] float fallSpeed = 0.01f;     // How fast the object falls off the screen

    public AbilityBinding ability;

    private Vector3 startPos;
    private float time;
    private float collsionTime; //ensures that the ability orb wont collide with both of the player's colliders
    private bool immediate = true;

    public SpriteRenderer r;

    private void Start()
    {
        r = GetComponent<SpriteRenderer>();
        time = 0;
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate delta time for smooth consistent movement
        time += Time.deltaTime * movementSpeed;

        // Convert time to x values in range of swing width (-swingWidth to swingWidth)
        float x = Mathf.PingPong(time, swingWidth*2) - swingWidth;

        // Back and forth swing along wide quadratic arc
        Vector3 currentPosition = new Vector3(x,Mathf.Pow(x,2)/swingWidth * swingDepth,0);

        // Make the orb slowly fall off screen
        currentPosition.y -= fallSpeed * time;

        // Delete when off screen (immediate variable is to prevent immediate destroy call upon creation)
        if (!r.isVisible && !immediate) Destroy(gameObject);
        else immediate = false;

        // Update object position
        transform.position = currentPosition + startPos;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.tag == "Player")
        {
            if ((time - collsionTime) < 0.5) return; // exit if collsions are happening to quickly (multiple colliders in player)
            collsionTime = time;

            Debug.Log("Player collision");
            Vector2 playerPos = collision.gameObject.transform.position;
            Vector2 relativePos = (Vector2)transform.position - playerPos;
            int[] bindingOrder = new int[4];

            // Set the order of the closest ability bindings to the collision spot
            if(Mathf.Abs(relativePos.x) >= Mathf.Abs(relativePos.y))
            {
                if (relativePos.x >= 0)
                {
                    bindingOrder[0] = 1;
                    bindingOrder[2] = 3;
                }
                else
                {
                    bindingOrder[0] = 3;
                    bindingOrder[2] = 1;
                }

                if (relativePos.y >= 0)
                {
                    bindingOrder[1] = 0;
                    bindingOrder[3] = 2;
                }
                else
                {
                    bindingOrder[1] = 2;
                    bindingOrder[3] = 0;
                }
            }

            else
            {
                if (relativePos.x >= 0)
                {
                    bindingOrder[1] = 1;
                    bindingOrder[3] = 3;
                }
                else
                {
                    bindingOrder[1] = 3;
                    bindingOrder[3] = 1;
                }

                if (relativePos.y >= 0)
                {
                    bindingOrder[0] = 0;
                    bindingOrder[2] = 2;
                }
                else
                {
                    bindingOrder[0] = 2;
                    bindingOrder[2] = 0;
                }
            }

            // try to bind the abilty
            bool success = bindToAvailableInOrder(bindingOrder, collision.gameObject);

            if (success) Destroy(gameObject);

        }
    }

    private bool bindToAvailableInOrder(int[] bindingOrder, GameObject player)
    {
        //Debug.Log("try bind");
        foreach (int slot in bindingOrder)
        {
            Debug.Log("try slot: " + slot + " ability: " + player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability);
            if (!(player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability))
            {
                Debug.Log("succes slot: " + slot);
                player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability = Instantiate(ability);
                player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                return true;
            }
        }

        return false;
    }
}
