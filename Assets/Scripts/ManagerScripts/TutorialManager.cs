using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    private bool taskComplete = false;
    [SerializeField] GameObject[] titleText;
    [SerializeField] GameObject[] tasks;

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
    }
}
