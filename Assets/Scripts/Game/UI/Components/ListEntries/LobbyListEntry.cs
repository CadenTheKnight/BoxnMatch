using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Events;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class LobbyListEntry : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button lobbyButton;
        [SerializeField] private TextMeshProUGUI gameModeText;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyStatusText;
        [SerializeField] private TextMeshProUGUI playerCountText;

        private string lobbyId;
        private float lastClickTime;
        private const float doubleClickTimeThreshold = 0.3f;

        private void OnEnable()
        {
            lobbyButton.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            lobbyButton.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTimeThreshold)
                LobbyEvents.InvokeLobbyDoubleClicked(lobbyId);
            else
                LobbyEvents.InvokeLobbySelected(lobbyId, this);

            lastClickTime = Time.time;
        }

        public void SetLobby(Lobby lobby)
        {
            lobbyId = lobby.Id;
            lobbyNameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            gameModeText.text = lobby.Data["GameMode"].Value;
            lobbyStatusText.text = lobby.Data["InGame"].Value == "true" ? "In Game" : "In Lobby";
        }
    }
}