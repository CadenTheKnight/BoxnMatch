using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;

namespace Assets.Scripts.Game.Models
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
                readyIndicatorImage.color = new Color(46f / 255f, 213f / 255f, 115f / 255f);
            }
            else
            {
                readyIndicatorImage.color = new Color(255f / 255f, 71f / 255f, 87f / 255f);
            }
            gameObject.SetActive(true);
        }

        public void ClearData()
        {
            data = null;
            playerNameText.text = string.Empty;
            readyIndicatorImage.color = new Color(255f / 255f, 71f / 255f, 87f / 255f);
            gameObject.SetActive(false);
        }
    }
}
