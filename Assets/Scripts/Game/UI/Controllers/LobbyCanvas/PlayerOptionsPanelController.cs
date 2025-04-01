using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Colors;
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

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            startButton.onClick.AddListener(OnStartClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyRefreshed += OnLobbyRefreshed;
            Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady += OnLobbyNotReady;

            UpdateButtons();
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyRefreshed -= OnLobbyRefreshed;
            Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady -= OnLobbyNotReady;
        }

        private void OnLobbyRefreshed()
        {
            UpdateButtons();
        }

        private void OnLobbyReady()
        {
            startText.text = "START";
            startButton.interactable = true;
            UpdateButtonColors(startButton, true);
        }

        private void OnLobbyNotReady(int playersReady, int maxPlayerCount)
        {
            startText.text = $"{playersReady}/{maxPlayerCount} PLAYERS READY";
            startButton.interactable = false;
            UpdateButtonColors(startButton, false);
        }

        private void UpdateButtons()
        {
            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;

            startButton.gameObject.SetActive(isHost);
            readyUnreadyButton.gameObject.SetActive(!isHost);
        }

        private void OnLeaveClicked()
        {
            leaveButton.interactable = false;
            leaveLoadingBar.StartLoading();

            GameLobbyManager.Instance.LeaveLobby();

            leaveLoadingBar.StopLoading();
            leaveButton.interactable = true;
        }

        private async void OnStartClicked()
        {
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
            if (isHost) GameLobbyManager.Instance.SetAllPlayersUnready();
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
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            GameLobbyManager.Instance.TogglePlayerReady();
            readyUnreadyText.text = AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString() ? "READY" : "NOT READY";
            UpdateButtonColors(readyUnreadyButton, AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString());

            await Task.Delay(1500);
            readyUnreadyLoadingBar.StopLoading();
            readyUnreadyButton.interactable = true;
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