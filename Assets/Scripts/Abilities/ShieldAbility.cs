using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAbility : AbilityBinding
{
    public GameObject shield;
    public float positionOffset;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;

        GameObject tmp = Instantiate(shield);
        tmp.transform.position = spawnPos;
    }
}
