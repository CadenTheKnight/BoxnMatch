using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteExplosiveAbility : AbilityBinding
{

    [SerializeField] RemoteExplosive explosivePrefab;
    [SerializeField] float positionOffset;

    private RemoteExplosive explosive;
    private bool deployed = false;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        if(!deployed) // First activation deploys the charge
        {
            DeployExplosive(dir, pr);
        }
        else // Second activation detonates the charge then destroys the ability
        {
            if (explosive)
            {
                DetonateExplosive();
            }
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
            Destroy(gameObject);

        }
    }

    private void DeployExplosive(AbilityDirection dir, PlayerRotator pr)
    {
        deployed = true;
        Vector3 spawnPos = pr.transform.position;
        spawnPos += dir.GetUnitDirection() * positionOffset;
        explosive = Instantiate(explosivePrefab, spawnPos, transform.rotation);
    }

    private void DetonateExplosive()
    {
        explosive.explode();
    }
}
