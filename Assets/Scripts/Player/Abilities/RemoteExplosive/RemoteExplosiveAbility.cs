using UnityEngine;

public class RemoteExplosiveAbility : AbilityBinding
{

    [SerializeField] RemoteExplosive explosivePrefab;
    [SerializeField] float positionOffset;
    [SerializeField] private Sprite noAbility;

    private RemoteExplosive explosive;
    private bool deployed = false;

    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {
        if (!deployed) // First activation deploys the charge
        {
            DeployExplosive(dir, pr);
        }
        else // Second activation detonates the charge then destroys the ability
        {
            DetonateExplosive();
            pr.sockets[(int)dir].GetComponent<SpriteRenderer>().sprite = noAbility;
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
        explosive.Explode();
    }
}