using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
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
            // conversion to get actual socket position for any rotation
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;

            // Spawn the shield
            Vector3 spawnPos = pr.transform.position;
            spawnPos += dir.GetUnitDirection() * positionOffset;
            tmp = Instantiate(shield, spawnPos, pr.transform.rotation, pr.transform);
            tmp.transform.Rotate(0, 0, ((AbilityDirection)socketToFire).GetRotationZ());

            // Remove socket sprite
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;

            active = true;
        }
        else // Destroy on second fire
        {
            tmp.GetComponent<Shield>().delete();
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
