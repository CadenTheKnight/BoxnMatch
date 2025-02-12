using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;

namespace Assets.Scripts.Game
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyIndicatorImage;

        private LobbyPlayerData data;

        public void SetData(LobbyPlayerData data)
        {
            this.data = data;
            playerNameText.text = this.data.PlayerName;

            if (this.data.IsReady)
            {
                if (readyIndicatorImage != null)
                {
                    readyIndicatorImage.color = Color.green;
                }
            }
            gameObject.SetActive(true);
        }
    }
}
