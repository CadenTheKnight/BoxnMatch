using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startReadyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startReadyUnreadyText;

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            startReadyUnreadyButton.onClick.AddListener(OnStartReadyUnreadyClicked);

            LobbyEvents.OnLobbyReady += OnLobbyReady;
            LobbyEvents.OnLobbyNotReady += OnLobbyNotReady;
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            startReadyUnreadyButton.onClick.RemoveListener(OnStartReadyUnreadyClicked);

            LobbyEvents.OnLobbyReady -= OnLobbyReady;
            LobbyEvents.OnLobbyNotReady -= OnLobbyNotReady;
        }

        private void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            GameLobbyManager.Instance.LeaveLobby();

            leaveButton.interactable = true;
        }

        private async void OnStartReadyUnreadyClicked()
        {
            if (LobbyManager.Instance.IsLobbyHost)
            {
                StartGame();
            }
            else
            {
                startReadyUnreadyButton.interactable = false;

                await GameLobbyManager.Instance.TogglePlayerReady();
                UpdateStartReadyUnreadyButton(GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.LocalPlayer.Id));

                await Task.Delay(1500);
                startReadyUnreadyButton.interactable = true;
            }
        }

        private void UpdateStartReadyUnreadyButton(bool isReady)
        {
            if (LobbyManager.Instance.IsLobbyHost)
            {
                startReadyUnreadyButton.interactable = true;
                startReadyUnreadyText.text = "START";
                return;
            }
            else
            {
                startReadyUnreadyText.text = isReady ? "READY" : "NOT READY";

                ColorBlock colors = startReadyUnreadyButton.colors;
                UpdateButtonColors(isReady);
                startReadyUnreadyButton.colors = colors;
            }
        }

        private void OnLobbyReady()
        {
            if (LobbyManager.Instance.IsLobbyHost)
            {
                startReadyUnreadyText.text = "START";
                startReadyUnreadyButton.interactable = true;
                UpdateButtonColors(true);
            }
        }

        private void OnLobbyNotReady(int playersReady, int maxPlayerCount)
        {
            if (LobbyManager.Instance.IsLobbyHost)
            {
                startReadyUnreadyText.text = $"{playersReady}/{maxPlayerCount} PLAYERS READY";
                startReadyUnreadyButton.interactable = false;
                UpdateButtonColors(false);
            }
        }

        private async void StartGame()
        {
            leaveButton.interactable = false;
            startReadyUnreadyButton.interactable = false;
            startReadyUnreadyText.text = "STARTING GAME...";
            await Task.Delay(1500);
            await GameLobbyManager.Instance.SetAllPlayersUnready();
            leaveButton.interactable = true;
        }

        private void UpdateButtonColors(bool ready)
        {
            ColorBlock colors = startReadyUnreadyButton.colors;

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

            startReadyUnreadyButton.colors = colors;
        }
    }
}