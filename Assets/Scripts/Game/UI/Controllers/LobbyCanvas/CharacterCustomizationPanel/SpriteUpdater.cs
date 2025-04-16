using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel
{
    public class SpriteUpdater : MonoBehaviour
    {
        [SerializeField] private CharacterCustomizationPanelController characterCustomizer;
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
}
