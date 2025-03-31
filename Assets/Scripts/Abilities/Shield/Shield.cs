using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public AbilityDirection dir;
    private AudioSource audiosource;
    [SerializeField] private float destroyTime = 2;

    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DamageObject>())
        {
            delete();
        }
    }

    public IEnumerator handleDelete()
    {
        audiosource.Play();
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    public void delete()
    {
        StartCoroutine(handleDelete());
    }

}
