using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteHead : MonoBehaviour
{
    private Color selectedColor;
    //accessor
    public Color SelectedColor { get { return selectedColor; } }

    public delegate void OnColorChangedDel(Color color);
    public event OnColorChangedDel OnColorUpdated;

    public void SetColor(Color col)
    {
        selectedColor = col;
        OnColorUpdated?.Invoke(selectedColor);
    }
}
