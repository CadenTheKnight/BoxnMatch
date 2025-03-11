using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class completeOnPlayerTriggerArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponentInParent<TutorialManager>().setTaskComplete(true);
        }
    }
}
