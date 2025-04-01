using TMPro;
using UnityEngine;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class PlayerListEntry : MonoBehaviour
    {

        [Header("UI Components")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private GameObject activeStatePanel;
        [SerializeField] private GameObject inGameStatePanel;
        [SerializeField] private GameObject disconnectedStatePanel;
        [SerializeField] private TextMeshProUGUI playerNameText;

        public void SetEmpty()
        {
            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);
        }

        public void SetPlayer(Player player)
        {
            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);

            if (player.Data["Status"].Value == PlayerStatus.Ready.ToString() || player.Data["Status"].Value == PlayerStatus.NotReady.ToString())
            {
                bool isHost = LobbyManager.Instance.Lobby.HostId == player.Id;
                playerNameText.text = player.Data["Name"].Value + (isHost ? " (Host)" : "");
                playerNameText.color = isHost ? UIColors.greenDefaultColor : player.Data["Status"].Value == PlayerStatus.Ready.ToString() ? UIColors.greenDefaultColor : UIColors.redDefaultColor;
            }
            else if (player.Data["Status"].Value == PlayerStatus.InGame.ToString())
            {
                playerNameText.color = UIColors.yellowDefaultColor;
                inGameStatePanel.SetActive(true);
            }
            else if (player.Data["Status"].Value == PlayerStatus.Disconnected.ToString())
            {
                playerNameText.color = UIColors.redDefaultColor;
                disconnectedStatePanel.SetActive(true);
            }
        }
    }
}