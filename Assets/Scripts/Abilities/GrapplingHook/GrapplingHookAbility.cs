using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GrapplingHookAbility : AbilityBinding
{
    [SerializeField] float positionOffset;

    [SerializeField] private GameObject grapplinghook;
    private GameObject tmp;

    private bool active = false;

    private AbilityDirection d;
    private PlayerRotator p;


    public override void Fire(AbilityDirection dir, PlayerRotator pr)
    {

        // Set direction and rotator for later use
        p = pr;
        d = dir;

        if (!active) // Activate on first fire
        {
            // conversion to get actual socket position for any rotation
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;

            // Spawn grappling hook
            Vector3 spawnPos = pr.transform.position;
            spawnPos += dir.GetUnitDirection() * positionOffset;
            tmp = Instantiate(grapplinghook, spawnPos, pr.transform.rotation);
            tmp.transform.Rotate(0, 0, ((AbilityDirection)socketToFire).GetRotationZ());
            tmp.GetComponent<GrapplingHook>().dir = dir;
            tmp.GetComponent<GrapplingHook>().pr = pr;

            // Remove Sprite from socket
            pr.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;

            active = true;
        }
        else delete(); // Destory on second fire

    }

    private void delete()
    {
        Destroy(tmp);
        int socketToFire = ((int)d - (int)p.currDirection);
        if (socketToFire < 0) socketToFire += 4;
        p.sockets[socketToFire].GetComponent<SpriteRenderer>().sprite = null;
        Destroy(gameObject);
    }
}
