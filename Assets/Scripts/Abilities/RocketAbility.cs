using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketlAbility : AbilityBinding
{

    [SerializeField] Vector2 acceleration;
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
            rb.velocity += -1 * Time.deltaTime * (Vector2)direction.GetUnitDirection() * acceleration;
            Debug.Log(rb.velocity);

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
        if (!isActive)
        {
            // Spawns rocket in the direction used
            Vector3 spawnPosition = pr.transform.position;
            spawnPosition += dir.GetUnitDirection() * positionOffset;

            rb = pr.GetComponent<Rigidbody2D>();

            temp = Instantiate(rocket, spawnPosition, pr.transform.rotation, pr.transform);
            temp.transform.Rotate(0, 0, dir.GetRotationZ() + 180);
            direction = dir;
            isActive = true;
        }
        else // if pressed a second time it will cancel the ability so that players can prevent flying to much
        {
            isActive = false;
            Destroy(temp); // Destroy rocket
            Destroy(gameObject); // Destroy ability
        }
    }

}
