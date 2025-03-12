using UnityEngine;
using Unity.VisualScripting;

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
            Destroy(gameObject);
            pr.sockets[(int)dir].GetComponent<SpriteRenderer>().sprite = noAbility;
        }
    }

}