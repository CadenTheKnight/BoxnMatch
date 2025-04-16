using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenshakeSetter : MonoBehaviour
{
    private CinemachineImpulseListener listener;

    private void Start()
    {
        int on = PlayerPrefs.GetInt("Screenshake", 1);
        if(on == 1)
        {
            listener.enabled = true;
        } else
        {
            listener.enabled = false;
        }
    }

    public void UpdateScreenshakeSetting(bool on)
    {
        if (on)
        {
            PlayerPrefs.SetInt("Screenshake", 1);
            listener.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("Screenshake", 0);
            listener.enabled = false;
        }
    }
}
