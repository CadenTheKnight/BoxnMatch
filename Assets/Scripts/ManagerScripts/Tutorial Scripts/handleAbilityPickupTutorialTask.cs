using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handleAbilityPickupTutorialTask : MonoBehaviour
{

    public GameObject orb;
    public ManageOrbs orbManager;

    // Start is called before the first frame update
    void Start()
    {
        orb = orbManager.spawnOrb(transform.position - new Vector3(0, 2, 0), transform.rotation, 0);
        orb.GetComponent<AbilityOrbHandler>().freeze = true;
    }

    private void Update()
    {
        if (!orb) GetComponentInParent<TutorialManager>().setTaskComplete(true); // finish task when orb is picked up
    }
}
