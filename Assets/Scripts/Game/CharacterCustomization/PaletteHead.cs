using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteHead : MonoBehaviour
{
    private Color selectedColor;
    //accessor
    public Color SelectedColor { get { return selectedColor; } }

    [SerializeField] private ColorChannelButton[] channelButtons;
    private ColorChannelButton currChannel;

    public delegate void OnColorChangedDel(Color color);
    public event OnColorChangedDel OnColorUpdated;

    public void SetColor(Color col)
    {
        selectedColor = col;
        OnColorUpdated?.Invoke(selectedColor);
    }

    public void SetCurrChannel(ColorChannelButton ccb)
    {
        for(int i = 0; i < channelButtons.Length; i++)
        {
            channelButtons[i].DeselectChannel();
        }
        currChannel = ccb;
        ccb.SelectChannel();
    }
}
