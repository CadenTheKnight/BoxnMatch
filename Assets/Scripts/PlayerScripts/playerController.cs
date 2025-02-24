using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //NEW

public class playerController : MonoBehaviour
{
    [SerializeField] private float BASE_SPEED = 5;
    private Rigidbody2D rb;

    float currentSpeed;
    //NEW
    [SerializeField] private float JUMP_FORCE = 5f;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;
    public int jumpCount = 0;
    public int maxJumps = 2;
    bool isGrounded = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = BASE_SPEED;
    }

    public IEnumerator SpeedChange(float newSpeed, float timeInSecs)
    {
        currentSpeed = newSpeed;
        yield return new WaitForSeconds(timeInSecs);
        currentSpeed = BASE_SPEED;
    }




    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, 0);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        
        rb.velocity = new Vector2((dir * currentSpeed).x, rb.velocity.y);

        if (horizontal < 0)
        {
            this.transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        //NEW
        //jumping 1
        Debug.Log(isGrounded + " " + rb.velocity.y);
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, JUMP_FORCE), ForceMode2D.Impulse);
            jumpCount++;
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rb.AddForce(new Vector2(0, -JUMP_FORCE), ForceMode2D.Impulse);
        }





    }
}