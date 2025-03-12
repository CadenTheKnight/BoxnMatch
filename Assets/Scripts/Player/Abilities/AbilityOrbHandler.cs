using UnityEngine;

public class AbilityOrbHandler : MonoBehaviour
{
    [SerializeField] float swingWidth = 1f;       // Width of the swing
    [SerializeField] float swingDepth = 0.1f;     // Depth of the swing
    [SerializeField] float movementSpeed = 1f;    // Speed of movement
    [SerializeField] float fallSpeed = 0.01f;     // How fast the object falls off the screen

    public AbilityBinding ability;

    private Vector3 startPos;
    private float time;
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
        time += Time.deltaTime * movementSpeed;
        float x = Mathf.PingPong(time, swingWidth * 2) - swingWidth;

        Vector3 currentPosition = new(x, Mathf.Pow(x, 2) / swingWidth * swingDepth, 0);

        currentPosition.y -= fallSpeed * time;

        if (!r.isVisible && !immediate) Destroy(gameObject);
        else immediate = false;

        transform.position = currentPosition + startPos;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collision");
            Vector2 playerPos = collision.gameObject.transform.position;
            Vector2 relativePos = (Vector2)transform.position - playerPos;
            int[] bindingOrder = new int[4];

            if (Mathf.Abs(relativePos.x) >= Mathf.Abs(relativePos.y))
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

            bool success = BindToAvailableInOrder(bindingOrder, collision.gameObject);
            if (success) Destroy(gameObject);

        }
    }

    private bool BindToAvailableInOrder(int[] bindingOrder, GameObject player)
    {
        foreach (int slot in bindingOrder)
        {
            Debug.Log("try slot: " + slot + " ability: " + player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability);
            if (!player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability)
            {
                Debug.Log("succes slot: " + slot);
                player.GetComponent<PlayerRotator>().sockets[slot].GetComponent<AbilitySocket>().ability = Instantiate(ability);
                return true;
            }
        }

        return false;
    }
}
