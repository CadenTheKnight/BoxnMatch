using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startText;
        [SerializeField] private TextMeshProUGUI readyUnreadyText;

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            startButton.onClick.AddListener(OnStartClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyRefreshed += UpdateButtons;
            Events.LobbyEvents.OnLobbyReady += OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady += OnLobbyNotReady;

            UpdateButtons();
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyRefreshed -= UpdateButtons;
            Events.LobbyEvents.OnLobbyReady -= OnLobbyReady;
            Events.LobbyEvents.OnLobbyNotReady -= OnLobbyNotReady;
        }

        private void UpdateButtons()
        {
            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;

            startButton.gameObject.SetActive(isHost);
            readyUnreadyButton.gameObject.SetActive(!isHost);

            if (isHost)
            {
                int readyPlayers = 0;
                int totalPlayers = LobbyManager.Instance.Lobby.Players.Count;

                foreach (Player player in LobbyManager.Instance.Lobby.Players)
                    if (player.Data["Status"].Value == PlayerStatus.Ready.ToString())
                        readyPlayers++;

                bool canStart = (totalPlayers > 1) && (readyPlayers == totalPlayers - 1);
                startButton.interactable = canStart;
                startText.text = canStart ? "START" : $"{readyPlayers}/{totalPlayers - 1} PLAYERS READY";
                UpdateButtonColors(startButton, canStart);
            }
            else
            {
                readyUnreadyText.text = AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString() ? "READY" : "NOT READY";
                UpdateButtonColors(readyUnreadyButton, AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString());
            }
        }

        private void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            GameLobbyManager.Instance.LeaveLobby();

            leaveButton.interactable = true;
        }

        private async void OnStartClicked()
        {
            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;
            if (isHost)
            {
                startButton.interactable = false;
                startText.text = "STARTING GAME...";
            }
            else
            {
                readyUnreadyButton.interactable = false;
                readyUnreadyText.text = "STARTING GAME...";
            }

            leaveButton.interactable = false;

            await Task.Delay(500);
            if (isHost) GameLobbyManager.Instance.SetAllPlayersUnready();
            await Task.Delay(500);

            leaveButton.interactable = true;

            if (isHost) startButton.interactable = true;
            else readyUnreadyButton.interactable = true;
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;

            GameLobbyManager.Instance.TogglePlayerReady();
            readyUnreadyText.text = AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString() ? "READY" : "NOT READY";
            UpdateButtonColors(readyUnreadyButton, AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString());

            await Task.Delay(1500);
            readyUnreadyButton.interactable = true;
        }

        private void OnLobbyReady()
        {
            if (LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id)
            {
                startText.text = "START";
                startButton.interactable = true;
                UpdateButtonColors(startButton, true);
            }
        }

        private void OnLobbyNotReady(int playersReady, int maxPlayerCount)
        {
            if (LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id)
            {
                startText.text = $"{playersReady}/{maxPlayerCount} PLAYERS READY";
                startButton.interactable = false;
                UpdateButtonColors(startButton, false);
            }
        }

        private void UpdateButtonColors(Button button, bool ready)
        {
            ColorBlock colors = button.colors;

            if (ready)
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