using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class FireballAbility : AbilityBinding
{

    [SerializeField] float positionOffset;
    [SerializeField] int maxShots = 1;

    public GameObject Fireball;
    
    private GameObject activeFireball;
    private int currentShots = 0;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Spawns fireball in the direction used
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;
        activeFireball = Instantiate(Fireball, spawnPos, transform.rotation);
        activeFireball.GetComponent<Fireball>().dir = dir;

        currentShots++;
        if (currentShots >= maxShots) Destroy(gameObject); // set used to true to remove ability after max shots used
    }

}
