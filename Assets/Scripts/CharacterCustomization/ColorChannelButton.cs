using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterShaderChannel
{
    _BoxColor, 
    _LineColor,
    _EyeColor,
    _EyebrowsColor,
    _MouthColor,
    _NoseColor
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

        palette.OnColorUpdated += UpdateColor;
    }

    private void UpdateColor(Color col)
    {
        material.SetColor(shaderChannel.ToString(), col);
    }

    private void EnablePalette()
    {
        palette.gameObject.SetActive(true);
    }

}
