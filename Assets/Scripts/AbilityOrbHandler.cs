using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityOrbHandler : MonoBehaviour
{   
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public int swingDistance = 1;
    public double swingSpeed = 0.01;
    public double gravityChangeScale = 100;
    private bool directionFlag = true;
    private int frame = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {   
        frame++;
        // Initialize movement variable
        Vector3 movement = new Vector3(0,0,0);

        // Switching direction of swing when the swingDistance is reached
        if(transform.position[0] >= swingDistance) directionFlag = false;
        else if(transform.position[0] <= -swingDistance) directionFlag = true;

        // Adds the X movement in swinging back and forth in swing distance
        if(directionFlag == true) movement[0] += (float)swingSpeed;
        else movement[0] -= (float)swingSpeed;

        // Adds Y movement to the swing to mimic feather fall
        rb.gravityScale = Mathf.Abs(rb.velocity[0]/(float)gravityChangeScale);

        // Delete when off screen
        if(!sr.isVisible && rb.velocity[0] != 0){
            Destroy(gameObject);
            Debug.Log("destroy" + gameObject);
        } 
        // Update position based on movement
        rb.velocity += new Vector2(movement[0],movement[1]);
    }
}
