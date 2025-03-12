using UnityEngine;

public class Laser : MonoBehaviour
{
    public AbilityDirection dir;
    public float speed = 1f;

    void Update()
    {
        transform.position += speed * Time.deltaTime * dir.GetUnitDirection();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
