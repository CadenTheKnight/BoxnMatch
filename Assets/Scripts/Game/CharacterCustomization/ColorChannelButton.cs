using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterShaderChannel
{
    _BoxColor, 
    _LineColor,
    _FaceColor
}

public class ColorChannelButton : MonoBehaviour
{
    [SerializeField] private PaletteHead palette;
    [SerializeField] private CharacterShaderChannel shaderChannel;
    [SerializeField] private Material material;
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(EnablePalette);
    }

    private void UpdateColor(Color col)
    {
        material.SetColor(shaderChannel.ToString(), col);
    }

    private void EnablePalette()
    {
        palette.gameObject.SetActive(true);
        palette.SetCurrChannel(this);
    }

    public void SelectChannel()
    {
        palette.OnColorUpdated += UpdateColor;
    }

    public void DeselectChannel()
    {
        palette.OnColorUpdated -= UpdateColor;
    }

}
