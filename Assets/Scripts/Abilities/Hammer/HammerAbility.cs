using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HammerAbility : AbilityBinding
{
    [SerializeField] float positionOffset;
    [SerializeField] float duration;

    [SerializeField] private GameObject hammer;
    private GameObject tmp;

    private float time = 0f;
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
            // Spawn hammer
            Vector3 spawnPos = pr.transform.position;
            spawnPos += dir.GetUnitDirection() * positionOffset;
            tmp = Instantiate(hammer, spawnPos, pr.transform.rotation, pr.transform);
            tmp.transform.Rotate(0, 0, pr.currDirection.GetRotationZ());

            // Remove Sprite from socket
            int socketToFire = ((int)dir - (int)pr.currDirection);
            if (socketToFire < 0) socketToFire += 4;
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

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            time += Time.deltaTime;
            if(time >= duration)
            {
                delete();
            }
        }
    }
}
