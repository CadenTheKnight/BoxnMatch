using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BattleCountdown : MonoBehaviour
{
    //private TMP_Text 
    private void Start()
    {
        Time.timeScale = 0f;
    }

    private IEnumerator DoCountdown()
    {
        yield return new WaitForSeconds(1f);
    }
}
