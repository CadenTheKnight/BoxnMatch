using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
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

        //Debug.Log("Handle Exit " + colObj.name);

        // Ensure one trigger per exit
        if (debounceList.Contains(colObj)) return;

        if (colObj.CompareTag("Player")) // handle player exiting
        {
            Debug.Log("Handle Exit 1 " + colObj.name);

            // Add players to the debounce list
            StartCoroutine(HandleDebouncing(colObj));
            Debug.Log("Handle Exit 2 " + colObj.name);

            GetComponent<AudioSource>().Play();
            Debug.Log("Handle Exit 3 " + colObj.name);

            //Debug.Log("exit");
            DamageableObject damagableObj = colObj.GetComponent<DamageableObject>();
            Debug.Log("Handle Exit 4 " + colObj.name);

            colObj.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Debug.Log("Handle Exit 5 " + colObj.name);
            damagableObj.MakeInvincible(respawnInvincibilityTimer);
            Debug.Log("Handle Exit 6 " + colObj.name);
            damagableObj.Die();
            Debug.Log("Handle Exit 7 " + colObj.name);
            damagableObj.currentDamage = 0;
            Debug.Log("Handle Exit 8 " + colObj.name);

            int playerNumber = colObj.name == "Player-Couch-P1" ? 1 : 2;
            GameManager.Instance.PlayerEliminated(playerNumber);

            colObj.transform.position = respawnPoint;
            Debug.Log("Reset " + colObj.name + " to spawn point");
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
        Debug.Log(obj + " added to the debounce list");

        yield return new WaitForSeconds(debounceTimer);

        debounceList.Remove(obj);
        Debug.Log(obj + " removed from the debounce list");
    }
}
