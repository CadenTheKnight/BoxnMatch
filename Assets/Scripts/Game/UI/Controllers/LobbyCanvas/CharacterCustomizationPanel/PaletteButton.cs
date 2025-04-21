using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel
{
    public class PaletteButton : Button
    {
        [Header("Palette")]
        [SerializeField] private Color color = Color.white;
        [SerializeField] private PaletteHead paletteHead;

        protected override void Start()
        {
            base.Start();
            onClick.AddListener(SelectColor);
        }

        private void SelectColor()
        {
            SendColorToPalette();
        }

        private void SendColorToPalette()
        {
            paletteHead.SetColor(color);
        }
    }
}
