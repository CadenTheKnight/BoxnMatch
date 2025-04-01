using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Game.Data;

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

        public Action<string> lobbySingleClicked;
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
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= 0.5f)
                lobbyDoubleClicked?.Invoke();
            else
                lobbySingleClicked?.Invoke(lobbyId);

            lastClickTime = Time.time;
        }

        public void SetLobby(Lobby lobby)
        {
            lobbyId = lobby.Id;
            nameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            gameModeText.text = ((GameMode)int.Parse(lobby.Data["GameMode"].Value)).ToString();
            statusText.text = ((LobbyStatus)int.Parse(lobby.Data["Status"].Value)).ToString();
            mapImage.sprite = mapSelectionData.GetMap(int.Parse(lobby.Data["MapIndex"].Value)).Thumbnail;
        }
    }
}