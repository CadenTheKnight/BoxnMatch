using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class PlayerListEntry : MonoBehaviour
    {

        [Header("State Panels")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private GameObject activeStatePanel;
        [SerializeField] private GameObject disconnectedStatePanel;

        [Header("Player Info Components")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyUnreadyIndicatorPanel;

        private LobbyPlayerData lobbyPlayerData;

        public void SetData(LobbyPlayerData lobbyPlayerData)
        {
            this.lobbyPlayerData = lobbyPlayerData;

            if (lobbyPlayerData.Status == PlayerStatus.Connected)
            {
                bool isHost = LobbyManager.Instance.IsHostId(lobbyPlayerData.Id);
                playerNameText.text = lobbyPlayerData.Name + (isHost ? "(Host)" : "");

                if (isHost)
                    readyUnreadyIndicatorPanel.gameObject.SetActive(false);
                else
                    readyUnreadyIndicatorPanel.color = lobbyPlayerData.Status == PlayerStatus.Ready ? UIColors.greenDefaultColor : UIColors.redDefaultColor;
            }

            SetState(lobbyPlayerData.Status);
        }

        public void SetState(PlayerStatus status)
        {
            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);

            switch (status)
            {
                case PlayerStatus.Disconnected:
                    disconnectedStatePanel.SetActive(true);
                    break;
                case PlayerStatus.NotReady:
                case PlayerStatus.Ready:
                    activeStatePanel.SetActive(true);
                    break;
                default:
                    emptyStatePanel.SetActive(true);
                    break;
            }
        }
    }
}