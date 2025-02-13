using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public AbilityDirection dir;
    public float speed = 1f;

    private bool immediate = true;

    private SpriteRenderer r;

    public void Start()
    {
        r = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // fireball moves in straight line in fired direction
        transform.position += dir.GetUnitDirection() * Time.deltaTime * speed;

        // Delete when off screen (immediate variable is to prevent immediate destroy call upon creation)
        if (!r.isVisible && !immediate) Destroy(gameObject);
        else immediate = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
