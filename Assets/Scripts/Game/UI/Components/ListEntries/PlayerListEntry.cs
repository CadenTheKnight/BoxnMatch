using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Enums;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components.Colors;

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

        private LobbyPlayerData playerData;
        private LobbyPlayerSpotState currentState = LobbyPlayerSpotState.Empty;

        public void SetData(LobbyPlayerData data, LobbyPlayerSpotState state)
        {
            playerData = data;

            if (state != LobbyPlayerSpotState.Empty)
            {
                bool isHost = LobbyManager.Instance.IsHostId(playerData.PlayerId);
                playerNameText.text = isHost ? $"{playerData.PlayerName} (Host)" : playerData.PlayerName;

                if (isHost)
                    readyUnreadyIndicatorPanel.gameObject.SetActive(false);
                else
                    readyUnreadyIndicatorPanel.color = playerData.IsReady ? UIColors.greenDefaultColor : UIColors.redDefaultColor;
            }

            SetState(state);
        }

        public void SetState(LobbyPlayerSpotState state)
        {
            currentState = state;

            emptyStatePanel.SetActive(state == LobbyPlayerSpotState.Empty);
            activeStatePanel.SetActive(state == LobbyPlayerSpotState.Active || state == LobbyPlayerSpotState.Disconnected);
            disconnectedStatePanel.SetActive(state == LobbyPlayerSpotState.Disconnected);
        }
    }
}