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
    [SerializeField] Vector2 launchForce;
    [SerializeField] private Sprite noAbility;

    public GameObject Fireball;
    
    private GameObject activeFireball;
    private int currentShots = 0;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Spawns fireball in the direction used
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;
        activeFireball = Instantiate(Fireball, spawnPos, transform.rotation);

        // Handle launching the fireball
        if (dir == AbilityDirection.NORTH) activeFireball.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, launchForce.y), ForceMode2D.Impulse);
        else if (dir == AbilityDirection.EAST) activeFireball.GetComponent<Rigidbody2D>().AddForce(launchForce, ForceMode2D.Impulse);
        else if (dir == AbilityDirection.SOUTH) activeFireball.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -launchForce.y), ForceMode2D.Impulse);
        else if (dir == AbilityDirection.WEST) activeFireball.GetComponent<Rigidbody2D>().AddForce(new Vector2(-launchForce.x, launchForce.y), ForceMode2D.Impulse);

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
