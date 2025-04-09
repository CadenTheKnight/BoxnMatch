using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LifeCounter : MonoBehaviour
{

    [SerializeField] private int lives;

    void Start()
    {
        UpdateLifeDisplay();
    }

    private void UpdateLifeDisplay()
    {
        GetComponent<TMP_Text>().text = lives + "";
    }

    public void LoseLife()
    {
        lives--;
        UpdateLifeDisplay();
    }
    
    public void setLives(int l)
    {
        lives = l;
        UpdateLifeDisplay();
    }
}
