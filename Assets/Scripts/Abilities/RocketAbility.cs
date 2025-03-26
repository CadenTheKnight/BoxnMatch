using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.Rendering.DebugUI;

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
            // conversion to get actual socket position for any rotation
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;

            // Spawns rocket in the direction used
            Vector3 spawnPos = pr.transform.position;
            spawnPos += dir.GetUnitDirection() * positionOffset;
            temp = Instantiate(rocket, spawnPos, pr.transform.rotation, pr.transform);
            temp.transform.Rotate(0, 0, ((AbilityDirection)socketToFire).GetRotationZ() + 180);

            rb = pr.GetComponent<Rigidbody2D>();
            
            isActive = true;
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
