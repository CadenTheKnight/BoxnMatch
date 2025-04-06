using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI startText;
        [SerializeField] private LoadingBar startLoadingBar;
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private TextMeshProUGUI readyUnreadyText;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;

        [Header("UI References")]
        [SerializeField] private PlayerListPanelController playerListPanelController;

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
                leaveButton.onClick.AddListener(OnLeaveClicked);
                readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

                lobbyNameText.text = $"{LobbyManager.Instance.Lobby.Name}" + (LobbyManager.Instance.Lobby.IsPrivate ? " (PRIVATE)" : "");
                SetStartButtonState(playerId);
                lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
                leaveButton.onClick.RemoveListener(OnLeaveClicked);
                readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;

            startLoadingBar.StopLoading();
            leaveLoadingBar.StopLoading();
            readyUnreadyLoadingBar.StopLoading();
        }

        private async void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.Lobby.LobbyCode;

            lobbyCodeText.text = $"Copied!";
            lobbyCodeText.color = UIColors.Green.One;

            await Task.Delay(1000);

            lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
            lobbyCodeText.color = UIColors.Primary.One;
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;
            leaveLoadingBar.StartLoading();

            if (!await LobbyManager.Instance.LeaveLobby())
            {
                leaveLoadingBar.StopLoading();
                leaveButton.interactable = true;
            }
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            OperationResult result = await GameLobbyManager.Instance.TogglePlayerReady(GameLobbyManager.Instance.GetPlayerById(AuthenticationService.Instance.PlayerId));

            readyUnreadyLoadingBar.StopLoading();
            if (result.Status == ResultStatus.Success)
            {
                Player updatedPlayer = GameLobbyManager.Instance.GetPlayerById(AuthenticationService.Instance.PlayerId);
                SetReadyUnreadyButtonState((PlayerStatus)int.Parse(updatedPlayer.Data["Status"].Value));
                SetStartButtonState(updatedPlayer.Id);
                playerListPanelController.SetPlayerStatus(updatedPlayer.Id, (PlayerStatus)int.Parse(updatedPlayer.Data["Status"].Value));
                await Task.Delay(1000);
            }

            readyUnreadyButton.interactable = true;
        }

        public void SetStartButtonState(string playerId)
        {
            int playersReady = GameLobbyManager.Instance.GetPlayersReady();
            int maxPlayers = LobbyManager.Instance.Lobby.MaxPlayers;
            if (playersReady < maxPlayers) startText.text = $"{playersReady} / {maxPlayers} Ready";
            else
            {
                if (playerId == LobbyManager.Instance.Lobby.HostId)
                {
                    startButton.interactable = true;
                    startText.text = "Start Game";
                }
                else
                {
                    startText.text = "Waiting for host...";
                    startButton.interactable = false;
                }
            }

        }

        private void SetReadyUnreadyButtonState(PlayerStatus status)
        {
            readyUnreadyText.text = status == PlayerStatus.Ready ? "Ready" : "Not Ready";

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

        public void UpdateInteractable(bool interactable)
        {
            readyUnreadyButton.interactable = interactable;
        }
    }
}