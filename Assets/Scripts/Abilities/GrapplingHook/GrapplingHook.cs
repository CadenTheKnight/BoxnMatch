using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] public AbilityDirection dir;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float retractForce = 1f;
    [SerializeField] public bool extend = true;
    [SerializeField] public bool retract = false;
    [SerializeField] public float minimumRopeLength = 0.5f;
    [SerializeField] private float knockbackMult = 1f;

    public Rigidbody2D hookedPlayer;
    [SerializeField] public PlayerRotator pr;
    private GameObject rope;
    private GameObject anchor;

    [SerializeField] private AudioClip extending;
    [SerializeField] private AudioClip retracting;
    private AudioSource audioSource;

    private void Start()
    {
        rope = GameObject.Find("Rope");
        anchor = GameObject.Find("Anchor");
        audioSource = rope.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!anchor)
        {
            // Knockback the hooked player before deleting the hook
            if(hookedPlayer)
            {
                hookedPlayer.AddForce(pr.GetComponentInParent<Rigidbody2D>().velocity * knockbackMult);
            }
            Destroy(gameObject);
        }
        // Extend the rope
        if (extend)
        {
            // Move the anchor according to the designated speed
            anchor.transform.position += speed * Time.deltaTime * dir.GetUnitDirection();
            audioSource.clip = extending;
        }

        /* Update the Rope */
        // Set the rope to bridge the gap between the anchor and the player
        rope.transform.position = (anchor.transform.position + pr.transform.position) / 2;

        // Set the rope angle to correctly connect the ends to the anchor and player
        Vector2 vDif = rope.transform.position - anchor.transform.position;
        float angle = Mathf.Atan2(vDif.y, vDif.x) * Mathf.Rad2Deg;
        rope.transform.rotation = Quaternion.Euler(0f, 0f, angle + dir.GetRotationZ());

        // Set the rope scale to fill the distance completely
        rope.transform.localScale = new Vector3(0.1f, vDif.magnitude * 2, 1f);

        if (retract)
        {
            pr.GetComponent<Rigidbody2D>().AddForce(-retractForce * vDif.normalized);
            audioSource.clip = retracting;
        }
        if (rope.transform.localScale.y < minimumRopeLength && retract)
        {
            //Destroy(gameObject);
        }
    }
}
