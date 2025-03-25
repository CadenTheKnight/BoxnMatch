using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    private bool taskComplete = false;
    [SerializeField] GameObject[] titleText;
    [SerializeField] GameObject[] tasks;
    [SerializeField] GameObject dummy;
    [SerializeField] GameObject orbManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ADMovement());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setTaskComplete(bool complete)
    {
        this.taskComplete = complete;
    }

    private IEnumerator ADMovement()
    {
        setTaskComplete(false);
        tasks[0].SetActive(true);

        yield return new WaitUntil(() => taskComplete);

        tasks[0].SetActive(false);
        StartCoroutine(WMovement());
    }

    private IEnumerator WMovement()
    {
        setTaskComplete(false);
        tasks[1].SetActive(true);

        yield return new WaitUntil(() => taskComplete);

        tasks[1].SetActive(false);
        StartCoroutine(SMovement1());
    }

    private IEnumerator SMovement1()
    {
        setTaskComplete(false);
        tasks[2].SetActive(true);

        yield return new WaitUntil(() => taskComplete);

        tasks[2].SetActive(false);
        StartCoroutine(AbilityPickup());
    }

    private IEnumerator AbilityPickup()
    {
        setTaskComplete(false);
        tasks[3].SetActive(true);

        yield return new WaitUntil(() => taskComplete);

        tasks[3].SetActive(false);
        UseAbility();
    }

    private void UseAbility()
    {
        setTaskComplete(false);
        titleText[0].SetActive(false);
        tasks[4].SetActive(true);
        dummy.SetActive(true);
        orbManager.GetComponent<ManageOrbs>().enabled = true;
    }

}
