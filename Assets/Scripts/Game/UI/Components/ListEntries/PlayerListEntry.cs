using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components.Colors;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class PlayerListEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyIndicator;
        [SerializeField] private Color readyColor = UIColors.greenDefaultColor;
        [SerializeField] private Color unreadyColor = UIColors.redDefaultColor;

        private LobbyPlayerData _playerData;

        public void SetData(LobbyPlayerData playerData)
        {
            bool isHost = LobbyManager.Instance.IsHostId(playerData.PlayerId);

            playerNameText.text = isHost ? $"{playerData.PlayerName} (Host)" : playerData.PlayerName;
            readyIndicator.color = playerData.IsReady ? readyColor : unreadyColor;
        }
    }
}