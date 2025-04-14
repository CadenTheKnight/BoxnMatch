using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image privateIcon;
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

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (GameLobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
                leaveButton.onClick.AddListener(OnLeaveClicked);
                readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

                LobbyEvents.OnLobbyLeft += OnLobbyLeft;
                LobbyEvents.OnPlayerDataUpdated += OnPlayerDataUpdated;

                GameLobbyEvents.OnLobbyReadyStatusUpdated += OnLobbyReadyStatusUpdated;

                lobbyNameText.GetComponent<RectTransform>().anchorMin = new Vector2(GameLobbyManager.Instance.Lobby.IsPrivate ? 0.05f : 0f, 0f);
                lobbyNameText.text = GameLobbyManager.Instance.Lobby.Name;
                privateIcon.gameObject.SetActive(GameLobbyManager.Instance.Lobby.IsPrivate);
                GameLobbyManager.Instance.GetPlayersReady();
                lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.Lobby.LobbyCode}";
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
                leaveButton.onClick.RemoveListener(OnLeaveClicked);
                readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

                LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
                LobbyEvents.OnPlayerDataUpdated -= OnPlayerDataUpdated;

                GameLobbyEvents.OnLobbyReadyStatusUpdated -= OnLobbyReadyStatusUpdated;
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
            GUIUtility.systemCopyBuffer = GameLobbyManager.Instance.Lobby.LobbyCode;

            lobbyCodeText.text = $"Copied!";

            await Task.Delay(1000);

            lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.Lobby.LobbyCode}";
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;
            leaveLoadingBar.StartLoading();

            await GameLobbyManager.Instance.LeaveCurrentLobby();
        }

        private async void OnLobbyLeft(OperationResult result)
        {
            if (result.Status == ResultStatus.Error)
            {
                await Task.Delay(1000);

                leaveLoadingBar.StopLoading();
                leaveButton.interactable = true;
            }
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            await GameLobbyManager.Instance.TogglePlayerStatus(GameLobbyManager.Instance.GetPlayerById(AuthenticationService.Instance.PlayerId));
        }

        private async void OnPlayerDataUpdated(OperationResult result)
        {
            if (result.Status == ResultStatus.Success && result.Data is Dictionary<string, PlayerDataObject> dataDict && dataDict.ContainsKey("Status"))
            {
                readyUnreadyLoadingBar.StopLoading();

                if (result.Status == ResultStatus.Success)
                {
                    readyUnreadyText.text = (PlayerStatus)int.Parse(dataDict["Status"].Value) == PlayerStatus.Ready ? "Ready" : "Not Ready";

                    ColorBlock colors = readyUnreadyButton.colors;
                    if ((PlayerStatus)int.Parse(dataDict["Status"].Value) == PlayerStatus.Ready)
                    {
                        colors.normalColor = UIColors.Green.One;
                        colors.highlightedColor = UIColors.Green.Two;
                        colors.pressedColor = UIColors.Green.Three;
                        colors.selectedColor = UIColors.Green.Three;
                        colors.disabledColor = UIColors.Green.Five;
                    }
                    else
                    {
                        colors.normalColor = UIColors.Red.One;
                        colors.highlightedColor = UIColors.Red.Two;
                        colors.pressedColor = UIColors.Red.Three;
                        colors.selectedColor = UIColors.Red.Three;
                        colors.disabledColor = UIColors.Red.Five;
                    }
                    readyUnreadyButton.colors = colors;

                    GameLobbyManager.Instance.GetPlayersReady();
                }

                await Task.Delay(1000);

                readyUnreadyButton.interactable = true;
            }
        }

        public void OnLobbyReadyStatusUpdated(int playersReady, int maxPlayers)
        {
            if (playersReady == maxPlayers)
            {
                if (AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId)
                {
                    startButton.interactable = true;
                    startText.text = "Start Game";
                }
                else startText.text = "Waiting for host...";
            }
            else
            {
                startButton.interactable = false;
                startText.text = playersReady + " / " + maxPlayers + " ready";
            }
        }
    }
}