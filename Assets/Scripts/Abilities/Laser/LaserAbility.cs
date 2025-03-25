using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class LaserAbility : AbilityBinding
{

    [SerializeField] float positionOffset;
    [SerializeField] int maxShots = 1;
    [SerializeField] private Sprite noAbility;

    public GameObject Laser;
    
    private GameObject activeLaser;
    private int currentShots = 0;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Spawns fireball in the direction used
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;
        activeLaser = Instantiate(Laser, spawnPos, transform.rotation);
        activeLaser.transform.Rotate(0, 0, dir.GetRotationZ());
        activeLaser.GetComponent<Laser>().dir = dir;

        currentShots++;
        if (currentShots >= maxShots)
        {
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
            Destroy(gameObject);
        }
    }

}
