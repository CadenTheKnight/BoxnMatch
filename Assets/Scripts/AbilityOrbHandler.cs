using UnityEngine;
using UnityEngine.UIElements;

public class AbilityOrbHandler : MonoBehaviour
{
    [SerializeField] float swingWidth = 1f;       // Width of the swing
    [SerializeField] float swingDepth = 0.1f;     // Depth of the swing
    [SerializeField] float movementSpeed = 1f;    // Speed of movement
    [SerializeField] float fallSpeed = 0.01f;     // How fast the object falls off the screen

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
}
