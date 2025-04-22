using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteUpdater : MonoBehaviour
{
    [SerializeField] private CharacterCustomizer characterCustomizer;
    [SerializeField] private CharacterShaderChannel shaderChannel;
    private Sprite sprite;
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        sprite = img.sprite;

        GetComponent<Button>().onClick.AddListener(UpdateSprite);
    }

    public void UpdateSprite()
    {
        characterCustomizer.UpdateShader(sprite, shaderChannel);
        Highlight();
    }

    private void Highlight()
    {
        UnhighlightNeighbors();
        img.color = Color.grey;
    }

    public void Unhighlight()
    {
        img.color = Color.white;
    }

    private void UnhighlightNeighbors()
    {
        Transform par = transform.parent;

        for(int i = 0; i < par.childCount; i++)
        {
            par.GetChild(i).GetComponent<SpriteUpdater>().Unhighlight();
        }
    }
}
