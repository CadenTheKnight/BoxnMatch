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
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;

        tmp = Instantiate(shield, pr.transform);
        tmp.transform.position = spawnPos;

        d = dir;
        r = pr;

        if (active) // Destroy on second fire
        {
            Destroy(tmp);
            delete();
        }
        else active = true; // Set Active on first fire
    }

    private void delete()
    {
        int socketToFire = ((int)d - (int)r.currDirection);
        if (socketToFire < 0) socketToFire += 4;
        r.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
        Destroy(gameObject);
    }

    
}
