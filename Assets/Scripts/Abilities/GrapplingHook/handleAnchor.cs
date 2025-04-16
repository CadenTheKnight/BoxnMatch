using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandleAnchor : MonoBehaviour
{
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GrapplingHook script = GetComponentInParent<GrapplingHook>();
        Rigidbody2D rigidbody2D = collision.GetComponent<Rigidbody2D>();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        script.extend = false;
        script.retract = true;
        GetComponent<HingeJoint2D>().connectedBody = rigidbody2D;
        if (collision.CompareTag("Player"))
        {
            script.hookedPlayer = rigidbody2D;
        }
    }
    */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GrapplingHook script = GetComponentInParent<GrapplingHook>();
        Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        script.extend = false;
        script.retract = true;
        GetComponent<HingeJoint2D>().connectedBody = rigidbody2D;
        if (collision.gameObject.CompareTag("Player"))
        {
            script.hookedPlayer = rigidbody2D;
        }
    }
}
