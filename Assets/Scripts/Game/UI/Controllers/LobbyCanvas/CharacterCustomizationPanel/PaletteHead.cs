using UnityEngine;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel
{
    public class PaletteHead : MonoBehaviour
    {
        private Color selectedColor;
        //accessor
        public Color SelectedColor { get { return selectedColor; } }

        [SerializeField] private ColorChannelButton[] channelButtons;
        private ColorChannelButton currChannel;

        public delegate void OnColorChangedDel(Color color);
        public event OnColorChangedDel OnColorUpdated;

        private void Start()
        {
            SetCurrChannel(channelButtons[0]);
        }

        public void SetColor(Color col)
        {
            selectedColor = col;
            OnColorUpdated?.Invoke(selectedColor);
        }

        public void SetCurrChannel(ColorChannelButton ccb)
        {
            for (int i = 0; i < channelButtons.Length; i++)
            {
                channelButtons[i].DeselectChannel();
            }
            currChannel = ccb;
            ccb.SelectChannel();
        }
    }
}
