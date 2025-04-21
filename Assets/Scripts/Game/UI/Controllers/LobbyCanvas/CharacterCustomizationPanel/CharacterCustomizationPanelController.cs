using UnityEngine;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel
{
    public class CharacterCustomizationPanelController : MonoBehaviour
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
}