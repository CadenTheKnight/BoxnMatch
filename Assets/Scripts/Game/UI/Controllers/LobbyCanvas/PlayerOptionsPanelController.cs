using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private TextMeshProUGUI readyUnreadyText;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationManager.Instance.LocalPlayer);
        }

        private async void OnPlayerConnect(Player player)
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerStatusChanged += OnPlayerStatusChanged;

            await GameLobbyManager.Instance.TogglePlayerReady(player, setUnready: true);
        }

        private void OnPlayerDisconnect(Player player)
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerStatusChanged -= OnPlayerStatusChanged;
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;

            leaveLoadingBar.StopLoading();
            readyUnreadyLoadingBar.StopLoading();
        }

        public async void OnPlayerStatusChanged(Player player)
        {
            SetReadyUnreadyButton(Enum.Parse<PlayerStatus>(player.Data["Status"].Value));

            await Task.Delay(1000);

            readyUnreadyLoadingBar.StopLoading();
            readyUnreadyButton.interactable = true;
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;
            leaveLoadingBar.StartLoading();

            await LobbyManager.Instance.LeaveLobby();

            if (LobbyManager.Instance.Lobby != null)
            {
                leaveButton.interactable = true;
                leaveLoadingBar.StopLoading();
            }
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            Player player = LobbyManager.Instance.Lobby.Players.Find(player => player.Id == AuthenticationManager.Instance.LocalPlayer.Id);
            await GameLobbyManager.Instance.TogglePlayerReady(player);
        }

        private void SetReadyUnreadyButton(PlayerStatus status)
        {
            readyUnreadyText.text = status == PlayerStatus.Ready ? "READY" : "NOT READY";

            ColorBlock colors = readyUnreadyButton.colors;

            if (status == PlayerStatus.Ready)
            {
                colors.normalColor = UIColors.Green.One;
                colors.highlightedColor = UIColors.Green.Two;
                colors.pressedColor = UIColors.Green.Three;
                colors.selectedColor = UIColors.Green.Three;
            }
            else
            {
                colors.normalColor = UIColors.Red.One;
                colors.highlightedColor = UIColors.Red.Two;
                colors.pressedColor = UIColors.Red.Three;
                colors.selectedColor = UIColors.Red.Three;
            }

            readyUnreadyButton.colors = colors;
        }
    }
}