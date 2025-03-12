using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Collider2D _collider_;
    private bool _playerOnPlatform;
    // Start is called before the first frame update
    void Start()
    {
        _collider_ = GetComponent<Collider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Input: " + Input.GetAxisRaw("Vertical"));
        if(_playerOnPlatform && Input.GetAxisRaw("Vertical") < 0)
        { 
            _collider_.enabled = false;
            StartCoroutine(routine:EnableCollider());
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        _collider_.enabled = true;
    }
    private void SetPlayerOnPlatform(Collision2D other, bool value)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = value;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        SetPlayerOnPlatform(other, true);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        SetPlayerOnPlatform(other, false);
    }

    
}
