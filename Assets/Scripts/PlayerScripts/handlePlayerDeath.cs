using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class handlePlayerDeath : MonoBehaviour
{

    [SerializeField] Vector3 respawnPoint;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("exit");
            collision.gameObject.GetComponent<DamageableObject>().currentDamage = 0;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            collision.gameObject.transform.position = respawnPoint;
        }
    }
}
