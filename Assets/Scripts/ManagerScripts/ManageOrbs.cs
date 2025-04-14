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
    [SerializeField] private Sprite[] ability_sprites;

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
            int randomAbilityIndex = (int)Random.Range(0, abilities.Length);
            GameObject temp = Instantiate(orb, new Vector3(Random.Range(spawnRangeX[0], spawnRangeX[1]), spawnY, 0), transform.rotation);
            temp.GetComponent<AbilityOrbHandler>().ability = abilities[randomAbilityIndex];
            temp.GetComponent<SpriteRenderer>().sprite = ability_sprites[randomAbilityIndex];
            //Debug.Log("ability is : " + randomAbilityIndex);
        }
    }

    public GameObject spawnOrb(Vector3 location, Quaternion rotation, int abilityIndex)
    {
        GameObject temp = Instantiate(orb, location, rotation);
        temp.GetComponent<AbilityOrbHandler>().ability = abilities[abilityIndex];
        temp.GetComponent<SpriteRenderer>().sprite = ability_sprites[abilityIndex];
        Debug.Log("ability is : " + abilityIndex);

        return temp;
    }
}
