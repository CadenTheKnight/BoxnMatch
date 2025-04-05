using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteUpdater : MonoBehaviour
{
    [SerializeField] private CharacterCustomizer characterCustomizer;
    [SerializeField] private CharacterShaderChannel shaderChannel;
    private Sprite sprite;

    private void Start()
    {
        sprite = GetComponent<Image>().sprite;

        GetComponent<Button>().onClick.AddListener(UpdateSprite);
    }

    public void UpdateSprite()
    {
        characterCustomizer.UpdateShader(sprite, shaderChannel);
    }
}
