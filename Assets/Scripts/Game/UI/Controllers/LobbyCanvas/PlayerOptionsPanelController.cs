using TMPro;
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
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private Button startButton;
        [SerializeField] private LoadingBar startLoadingBar;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;
        [SerializeField] private TextMeshProUGUI startText;
        [SerializeField] private TextMeshProUGUI readyUnreadyText;

        private bool isStarting = false;

        private void Start()
        {
            UpdateButtons();
        }

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            startButton.onClick.AddListener(OnStartClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
            LobbyEvents.OnNewLobbyHost += OnNewLobbyHost;

            Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady += OnLobbyNotReady;
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
            LobbyEvents.OnNewLobbyHost -= OnNewLobbyHost;

            Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady -= OnLobbyNotReady;
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

        private async void OnStartClicked()
        {
            if (isStarting) return;
            isStarting = true;

            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;
            if (isHost)
            {
                startButton.interactable = false;
                startText.text = "STARTING GAME...";
                startLoadingBar.StartLoading();
            }
            else
            {
                readyUnreadyButton.interactable = false;
                readyUnreadyText.text = "STARTING GAME...";
                readyUnreadyLoadingBar.StartLoading();
            }

            leaveButton.interactable = false;

            await Task.Delay(500);
            if (isHost) await GameLobbyManager.Instance.SetAllPlayersUnready();
            await Task.Delay(500);

            leaveButton.interactable = true;

            if (isHost)
            {
                startButton.interactable = true;
                startLoadingBar.StopLoading();
            }
            else
            {
                readyUnreadyButton.interactable = true;
                readyUnreadyLoadingBar.StopLoading();
            }

            isStarting = false;
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            await GameLobbyManager.Instance.TogglePlayerReady(AuthenticationManager.Instance.LocalPlayer);
            SetReadyUnreadyButton(AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString());

            await Task.Delay(1500);
            readyUnreadyLoadingBar.StopLoading();
            readyUnreadyButton.interactable = true;
        }

        private void OnPlayerDataChanged(string playerId)
        {
            SetReadyUnreadyButton(AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString());
        }

        private void OnNewLobbyHost(Player player)
        {
            UpdateHost();
        }

        private void OnLobbyReady()
        {
            if (isStarting) return;

            startText.text = "START";
            startButton.interactable = true;
            UpdateButtonColors(startButton, true);
        }

        private void OnLobbyNotReady(int playersReady, int maxPlayerCount)
        {
            if (isStarting) return;

            startText.text = $"{playersReady}/{maxPlayerCount} PLAYERS READY";
            startButton.interactable = false;
            UpdateButtonColors(startButton, false);
        }

        private void UpdateHost()
        {
            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;
            startButton.gameObject.SetActive(isHost);
            readyUnreadyButton.gameObject.SetActive(!isHost);
        }

        private void UpdateButtons()
        {
            SetStartButton(GameLobbyManager.Instance.GetPlayersReady());
        }

        private void SetStartButton(int playersReady)
        {
            startText.text = $"{playersReady}/{LobbyManager.Instance.Lobby.MaxPlayers} PLAYERS READY";
        }

        private void SetReadyUnreadyButton(bool isReady)
        {
            readyUnreadyText.text = isReady ? "READY" : "NOT READY";
            UpdateButtonColors(readyUnreadyButton, isReady);
        }

        private void UpdateButtonColors(Button button, bool green)
        {
            ColorBlock colors = button.colors;

            if (green)
            {
                colors.normalColor = UIColors.greenDefaultColor;
                colors.highlightedColor = UIColors.greenHoverColor;
                colors.pressedColor = UIColors.greenDefaultColor;
                colors.selectedColor = UIColors.greenHoverColor;
            }
            else
            {
                colors.normalColor = UIColors.redDefaultColor;
                colors.highlightedColor = UIColors.redHoverColor;
                colors.pressedColor = UIColors.redDefaultColor;
                colors.selectedColor = UIColors.redHoverColor;
            }

            button.colors = colors;
        }
    }
}