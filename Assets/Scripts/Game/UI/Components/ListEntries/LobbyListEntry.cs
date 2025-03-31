using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class LobbyListEntry : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button lobbyButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI gameModeText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Image mapImage;

        private string lobbyId;
        private float lastClickTime;

        public Action<string, LobbyListEntry> lobbySingleClicked;
        public Action<string> lobbyDoubleClicked;

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

            if (timeSinceLastClick <= 0.3f)
                lobbyDoubleClicked?.Invoke(lobbyId);
            else
                lobbySingleClicked?.Invoke(lobbyId, this);

            lastClickTime = Time.time;
        }

        public void SetLobby(Lobby lobby)
        {
            lobbyId = lobby.Id;
            nameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            statusText.text = lobby.Data["Status"].Value;
            gameModeText.text = lobby.Data["GameMode"].Value;
            // mapImage.sprite = Resources.Load<Sprite>($"Maps/{lobby.Data["MapName"].Value}");
        }
    }
}