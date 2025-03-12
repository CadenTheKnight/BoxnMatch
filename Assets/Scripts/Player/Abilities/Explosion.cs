using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] public float explosionDuration;

    private float time = 0;

    private void Update()
    {
        time += Time.deltaTime;

        if (time > explosionDuration)
            Destroy(gameObject);
    }
}
