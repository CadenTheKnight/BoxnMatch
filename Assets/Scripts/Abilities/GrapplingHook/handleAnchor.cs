using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class handleAnchor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GrapplingHook script = GetComponentInParent<GrapplingHook>();
        Rigidbody2D rigidbody2D = collision.GetComponent<Rigidbody2D>();
        script.extend = false;
        script.retract = true;
        GetComponent<HingeJoint2D>().connectedBody = rigidbody2D;
        if (collision.CompareTag("Player"))
        {
            script.hookedPlayer = rigidbody2D;
        }
    }
}
