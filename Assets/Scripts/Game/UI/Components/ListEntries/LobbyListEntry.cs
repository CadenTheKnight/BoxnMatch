using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
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

        [Header("Data References - might replace")]
        [SerializeField] private MapSelectionData mapSelectionData;

        private string lobbyId;
        private float lastClickTime;
        private bool doubleClickCooldown = false;

        public Action<string, LobbyListEntry> lobbySingleClicked;
        public Action lobbyDoubleClicked;

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
            if (doubleClickCooldown) return;

            if (Time.time - lastClickTime <= 0.5f)
            {
                lobbyDoubleClicked?.Invoke();
                doubleClickCooldown = true;
                Invoke(nameof(ResetDoubleClickCooldown), 1f);
            }
            else
            {
                lobbySingleClicked?.Invoke(lobbyId, this);
                SetSelected(true);
            }

            lastClickTime = Time.time;
        }

        private void ResetDoubleClickCooldown()
        {
            doubleClickCooldown = false;
            lastClickTime = 0f;
        }

        public void SetLobby(Lobby lobby)
        {
            lobbyId = lobby.Id;
            nameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            gameModeText.text = ((GameMode)int.Parse(lobby.Data["GameMode"].Value)).ToString();
            statusText.text = ((LobbyStatus)int.Parse(lobby.Data["Status"].Value)).ToString();
            mapImage.sprite = mapSelectionData.GetMap(int.Parse(lobby.Data["MapIndex"].Value)).Thumbnail;

            SetSelected(false);
        }

        public void SetSelected(bool isSelected)
        {
            ColorBlock colors = lobbyButton.colors;
            colors.normalColor = isSelected ? colors.selectedColor : colors.normalColor;
            lobbyButton.colors = colors;
        }
    }
}