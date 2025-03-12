using UnityEngine;

public class UnparentOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.SetParent(null);
    }
}