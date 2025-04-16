using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private CanvasGroup cg;
    [Header("Pause Aesthetics")]
    public float pauseFadeTime = 0.5f;

    [Header("Scenes")]
    public string menuSceneName;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        cg.DOFade(1f, pauseFadeTime).SetUpdate(true);
        cg.blocksRaycasts = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        cg.DOFade(0f, pauseFadeTime).SetUpdate(true);
        cg.blocksRaycasts = false;
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
