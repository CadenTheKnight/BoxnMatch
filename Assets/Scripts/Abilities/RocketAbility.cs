using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketlAbility : AbilityBinding
{

    [SerializeField] float acceleration = 60;
    [SerializeField] float positionOffset = 0.5f;
    [SerializeField] float abilityDuration = 5f;

    private Rigidbody2D rb;
    private bool isActive = false;
    private AbilityDirection direction;
    private float time = 0f;

    public GameObject rocket; // prefab of rocket model
    private GameObject temp;

    private void Update()
    {
        if (isActive)
        {
            time += Time.deltaTime;

            // accelerates player in opposite direction of use
            rb.AddForce(-1 * Time.deltaTime * (Vector2)direction.GetUnitDirection() * acceleration, ForceMode2D.Impulse);

            if (time > abilityDuration)
            {
                isActive = false;
                Destroy(temp); // Destroy rocket
                Destroy(gameObject); // Destroy ability
            }
        }
    }


    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        direction = dir;

        if (!isActive)
        {
            // Spawns rocket in the direction used
            Vector3 spawnPosition = pr.transform.position;
            spawnPosition += dir.GetUnitDirection() * positionOffset;

            rb = pr.GetComponent<Rigidbody2D>();

            temp = Instantiate(rocket, spawnPosition, pr.transform.rotation, pr.transform);
            
            isActive = true;
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
        }
        else // if pressed a second time it will cancel the ability so that players can prevent flying to much
        {
            isActive = false;
            Destroy(temp); // Destroy rocket
            Destroy(gameObject); // Destroy ability
        }
    }

}
