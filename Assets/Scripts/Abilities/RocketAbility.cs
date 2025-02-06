using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketlAbility : AbilityBinding
{

    [SerializeField] float acceleration = 10f;
    [SerializeField] float positionOffset = 0.5f;
    [SerializeField] float abilityDuration = 5f;

    private Rigidbody2D rb;
    private bool isActive = false;
    private AbilityDirection direction;
    private float time = 0f;

    public GameObject rocket;
    private GameObject temp;

    private void Update()
    {
        if (isActive)
        {
            time += Time.deltaTime;

            // accelerates player in opposite direction of use
            rb.velocity += -1 * acceleration * Time.deltaTime * (Vector2)direction.GetUnitDirection();

            if (time > abilityDuration)
            {
                isActive = false;
                Destroy(temp); // Destroy rocket
            }
        }
    }


    public override void Fire(AbilityDirection dir, PlayerRotator pr)
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

}
