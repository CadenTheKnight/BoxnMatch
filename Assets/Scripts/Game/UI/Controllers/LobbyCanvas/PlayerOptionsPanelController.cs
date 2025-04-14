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
        [SerializeField] private Image privateIndicator;
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

                GameLobbyEvents.OnPlayerStatusChanged += UpdateStartButtonState;

                lobbyNameText.GetComponent<RectTransform>().anchorMin = new Vector2(GameLobbyManager.Instance.Lobby.IsPrivate ? 0.06f : 0f, 0f);
                lobbyNameText.text = GameLobbyManager.Instance.Lobby.Name;
                privateIndicator.gameObject.SetActive(GameLobbyManager.Instance.Lobby.IsPrivate);
                UpdateStartButtonState();
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

                GameLobbyEvents.OnPlayerStatusChanged -= UpdateStartButtonState;
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

            Task<OperationResult> leaveTask = GameLobbyManager.Instance.LeaveCurrentLobby();
            await Task.WhenAny(leaveTask, Task.Delay(5000));

            leaveLoadingBar.StopLoading();
            if (!leaveTask.IsCompletedSuccessfully || leaveTask.IsCompletedSuccessfully && leaveTask.Result.Status == ResultStatus.Error)
            {
                await Task.Delay(1000);

                leaveButton.interactable = true;
            }
        }

        private async void OnReadyUnreadyClicked()
        {
            try
            {
                readyUnreadyButton.interactable = false;
                readyUnreadyLoadingBar.StartLoading();

                Task<OperationResult> toggleTask = GameLobbyManager.Instance.TogglePlayerStatus(GameLobbyManager.Instance.GetPlayerById(AuthenticationService.Instance.PlayerId));
                await Task.WhenAny(toggleTask, Task.Delay(5000));

                if (toggleTask.IsCompletedSuccessfully && toggleTask.Result.Status == ResultStatus.Success)
                {
                    if (toggleTask.Result.Data is Dictionary<string, PlayerDataObject> dataDict && dataDict.ContainsKey("Status"))
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

                        GameLobbyEvents.InvokePlayerStatusChanged(AuthenticationService.Instance.PlayerId);
                    }
                }
            }
            finally
            {
                readyUnreadyLoadingBar.StopLoading();

                await Task.Delay(1000);
                readyUnreadyButton.interactable = true;
            }
        }

        private void UpdateStartButtonState(string playerId = null)
        {
            int playersReady = GameLobbyManager.Instance.GetPlayersReady();
            int maxPlayers = GameLobbyManager.Instance.Lobby.MaxPlayers;
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