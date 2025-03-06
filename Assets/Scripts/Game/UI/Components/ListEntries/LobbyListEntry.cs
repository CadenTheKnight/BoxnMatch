using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Events;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class LobbyListEntry : MonoBehaviour
    {
        [SerializeField] private Button joinButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI gameModeText;

        private Lobby lobby;
        public bool isSelected;
        private float lastClickTime;
        private const float doubleClickTimeThreshold = 0.3f;

        private void OnEnable()
        {
            joinButton.onClick.AddListener(HandleClick);
            isSelected = false;
        }

        private void OnDestroy()
        {
            joinButton.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTimeThreshold)
                LobbyEvents.InvokeLobbyDoubleClicked(lobby);
            else
            {
                isSelected = true;
                LobbyEvents.InvokeLobbySelected(lobby, this);
            }

            lastClickTime = Time.time;
        }

        public void SetLobby(Lobby lobby)
        {
            this.lobby = lobby;

            lobbyNameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

            if (lobby.Data != null && lobby.Data.ContainsKey("GameMode"))
                gameModeText.text = lobby.Data["GameMode"].Value;
            else
                gameModeText.text = "Standard";
        }
    }
}