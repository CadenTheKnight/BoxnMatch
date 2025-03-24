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
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image readyIndicator;

        public void SetData(LobbyPlayerData playerData)
        {
            bool isHost = LobbyManager.Instance.IsHostId(playerData.PlayerId);

            playerNameText.text = isHost ? $"{playerData.PlayerName} (Host)" : playerData.PlayerName;

            if (isHost)
                readyIndicator.gameObject.SetActive(false);
            else
                readyIndicator.color = playerData.IsReady ? UIColors.greenDefaultColor : UIColors.redDefaultColor;
        }
    }
}