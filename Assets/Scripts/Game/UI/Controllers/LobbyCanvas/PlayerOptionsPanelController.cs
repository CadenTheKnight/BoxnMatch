using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Game.Managers;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Managers;
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
        [SerializeField] private TextMeshProUGUI leaveText;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private TextMeshProUGUI readyUnreadyText;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;

        private void OnEnable()
        {
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;

            lobbyNameText.GetComponent<RectTransform>().anchorMin = new Vector2(GameLobbyManager.Instance.Lobby.IsPrivate ? 0.06f : 0f, 0f);
            lobbyNameText.text = GameLobbyManager.Instance.Lobby.Name;
            privateIndicator.gameObject.SetActive(GameLobbyManager.Instance.Lobby.IsPrivate);
            UpdateStartButtonState();
            lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.Lobby.LobbyCode}";
        }

        private void OnDisable()
        {
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            GameLobbyEvents.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;

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

            await LobbyManager.LeaveLobby(GameLobbyManager.Instance.Lobby.Id);
        }

        private async void OnReadyUnreadyClicked()
        {

            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            await GameLobbyManager.Instance.TogglePlayerReadyStatus(AuthenticationService.Instance.PlayerId, GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId).Data["ReadyStatus"].Value == ((int)ReadyStatus.Ready).ToString() ? ReadyStatus.NotReady : ReadyStatus.Ready);
        }

        private async void OnLobbyLeft(OperationResult result)
        {
            Debug.Log($"OnLobbyLeft: {result.Status}");
            if (result.Status == ResultStatus.Error)
            {
                leaveText.text = "Error Leaving";
                leaveLoadingBar.StopLoading();

                await Task.Delay(1000);

                leaveText.text = "Leave";
                leaveButton.interactable = true;
            }
        }

        private async void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
        {
            readyUnreadyLoadingBar.StopLoading();

            if (success)
            {
                UpdateReadyButtonState(readyStatus);
                UpdateStartButtonState();
            }

            await Task.Delay(1000);

            readyUnreadyButton.interactable = true;
        }

        private void UpdateReadyButtonState(ReadyStatus readyStatus)
        {
            readyUnreadyText.text = readyStatus == ReadyStatus.Ready ? "Ready" : "Not Ready";

            ColorBlock colors = readyUnreadyButton.colors;
            if (readyStatus == ReadyStatus.Ready)
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
        }

        private void UpdateStartButtonState()
        {
            int playersReady = 0;
            foreach (Player player in GameLobbyManager.Instance.Lobby.Players)
                if (player.Data["ReadyStatus"].Value == ((int)ReadyStatus.Ready).ToString()) playersReady++;

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