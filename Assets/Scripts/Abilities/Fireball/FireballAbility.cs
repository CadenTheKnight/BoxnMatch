using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAbility : AbilityBinding
{

    public float speed = 1f;
    public float positionOffset;

    public GameObject Fireball;
    
    private GameObject activeFireball;


    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Spawns fireball in the direction used
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;
        GameObject activeFireball = Instantiate(Fireball);
        activeFireball.GetComponent<Fireball>().dir = dir;
    }

}
