using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageOrbs : MonoBehaviour
{

    public GameObject orb;
    public float secBetweenSpawn = 5f;
    public bool endGame = false;
    public Vector2 spawnRangeX;
    public float spawnY;

    [SerializeField] private AbilityBinding[] abilities;

    private float tempTime = 0;

    private void Update()
    {
        tempTime += Time.deltaTime;

        if (tempTime >= secBetweenSpawn)
        {
            spawnOrb();
            tempTime = 0;
        }
    }
    private void spawnOrb()
    {
        if (!endGame)
        {
            GameObject temp = Instantiate(orb, new Vector3(Random.Range(spawnRangeX[0], spawnRangeX[1]), spawnY, 0), transform.rotation);
            temp.GetComponent<AbilityOrbHandler>().ability = abilities[0];
        }
    }
}
