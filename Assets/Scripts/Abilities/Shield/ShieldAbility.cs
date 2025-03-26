using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ShieldAbility : AbilityBinding
{
    public GameObject shield;
    private bool active = false;
    public float positionOffset;

    private GameObject tmp;
    private AbilityDirection d;
    private PlayerRotator r;
    private bool immediate = true;

    private void Update()
    {
        if (active && !tmp && !immediate)
        {
            immediate = false;
            delete();
        }
    }

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        // Set direction and rotator for later use
        d = dir;
        r = pr;

        // Activate Ability on first fire
        if (!active)
        { 
            // Spawn the shield
            Vector3 spawnPos = pr.transform.position;
            spawnPos += dir.GetUnitDirection() * positionOffset;
            tmp = Instantiate(shield, spawnPos, pr.transform.rotation, pr.transform);

            // Remove socket sprite
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;

            active = true;
        }
        else // Destroy on second fire
        {
            Destroy(tmp);
            delete();
        }
    }

    private void delete()
    {
        int socketToFire = ((int)d - (int)r.currDirection);
        if (socketToFire < 0) socketToFire += 4;
        r.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
        Destroy(gameObject);
    }

    
}
