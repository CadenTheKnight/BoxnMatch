using UnityEngine;

public class HandleExitPlay : MonoBehaviour
{

    [SerializeField] Vector3 respawnPoint;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // handle player exiting
        {
            Debug.Log("exit");
            collision.gameObject.GetComponent<DamageableObject>().currentDamage = 0;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            collision.gameObject.transform.position = respawnPoint;
        }
        else // Destroy everything that exits thats not a player
            Destroy(collision.gameObject);
    }
}
