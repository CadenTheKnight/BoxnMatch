using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterShaderChannel
{
    _BoxColor, 
    _LineColor,
    _FaceColor,
    _Box,
    _Face
}

public class ColorChannelButton : MonoBehaviour
{
    [SerializeField] private PaletteHead palette;
    [SerializeField] private CharacterShaderChannel shaderChannel;
    [SerializeField] private GameObject character;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;

    private Material material;
    private Image bgImage;
    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(EnablePalette);

        bgImage = GetComponent<Image>();

        material = character.GetComponent<SpriteRenderer>().sharedMaterial;
    }

    private void UpdateColor(Color col)
    {
        material.SetColor(shaderChannel.ToString(), col);
        Debug.Log("updated color");
    }

    private void EnablePalette()
    {
        palette.gameObject.SetActive(true);
        palette.SetCurrChannel(this);
    }

    public void SelectChannel()
    {
        palette.OnColorUpdated += UpdateColor;
        bgImage.color = selectedColor;
    }

    public void DeselectChannel()
    {
        palette.OnColorUpdated -= UpdateColor;
        bgImage.color = deselectedColor;
    }

}
