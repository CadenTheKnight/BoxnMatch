using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class PlayerListEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyIndicator;
        [SerializeField] private Color readyColor = UIColors.successDefaultColor;
        [SerializeField] private Color unreadyColor = UIColors.errorDefaultColor;

        public void SetPlayer(Player player)
        {
            playerNameText.text = player.Data["PlayerName"].Value;
            readyIndicator.color = player.Data["IsReady"].Value == "true" ? readyColor : unreadyColor;
        }
    }
}