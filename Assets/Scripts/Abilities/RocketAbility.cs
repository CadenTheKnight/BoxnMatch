using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketlAbility : AbilityBinding
{

    [SerializeField] float acceleration = 10f;
    [SerializeField] float positionOffset = 0.5f;
    [SerializeField] float abilityDuration = 5f;

    private Rigidbody2D rb;
    public bool isActive = false;
    private AbilityDirection direction;
    private float time = 0f;

    public GameObject rocket;
    private GameObject temp;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isActive)
        {
            time += Time.deltaTime;

            // accelerates player in opposite direction of use
            rb.velocity += -1 * acceleration * (Vector2)direction.GetUnitDirection();

            if (time > abilityDuration)
            {
                isActive = false;
                Destroy(temp); // Destroy rocket
                Destroy(this); // Destroy this script
            }
        }
    }


    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Spawns fireball in the direction used
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;

        temp = Instantiate(rocket, spawnPos, pr.transform.rotation);
        direction = dir;
        isActive = true;
    }

}
