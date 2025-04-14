using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class handleExitPlay : MonoBehaviour
{

    [SerializeField] Vector3 respawnPoint;
    [SerializeField] float respawnInvincibilityTimer;
    [SerializeField] float debounceTimer;

    private ArrayList debounceList = new();

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject colObj = collision.gameObject;

        // Ensure one trigger per exit
        if (debounceList.Contains(colObj)) return; 
        StartCoroutine(HandleDebouncing(colObj));
        
        if (collision.gameObject.CompareTag("Player")) // handle player exiting
        {
            GetComponent<AudioSource>().Play();

            //Debug.Log("exit");
            DamageableObject damagableObj = colObj.GetComponent<DamageableObject>();

            damagableObj.Die();
            damagableObj.currentDamage = 0;
            colObj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            colObj.transform.position = respawnPoint;
            damagableObj.MakeInvincible(respawnInvincibilityTimer);
        }
        else // Destroy everything that exits thats not a player
        {
            Destroy(collision.gameObject);
        }
    }

    // Prevents multiple triggers on exit
    private IEnumerator HandleDebouncing(GameObject obj)
    {
        debounceList.Add(obj);

        yield return new WaitForSeconds(debounceTimer);

        debounceList.Remove(obj);
    }
}
