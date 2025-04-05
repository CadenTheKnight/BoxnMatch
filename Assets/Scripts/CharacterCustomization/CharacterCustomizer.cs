using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterCustomizer : MonoBehaviour
{
    [SerializeField] private GameObject character;
    private Material material;

    private void Start()
    {
        material = character.GetComponent<SpriteRenderer>().material;

        //UI Image components cannot render custom shaders :(
    }

    public void UpdateShader(Sprite boxSprite, CharacterShaderChannel channel)
    {
        material.SetTexture(channel.ToString(), boxSprite.texture);
    }

}
