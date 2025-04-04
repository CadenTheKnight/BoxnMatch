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
            SetReadyUnreadyButton(Enum.Parse<PlayerStatus>(AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value));
        }

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerStatusChanged += OnPlayerStatusChanged;
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnPlayerStatusChanged -= OnPlayerStatusChanged;
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

            await GameLobbyManager.Instance.TogglePlayerReady(AuthenticationManager.Instance.LocalPlayer);
        }

        public async void OnPlayerStatusChanged(int playerIndex, PlayerStatus newStatus)
        {
            SetReadyUnreadyButton(newStatus);

            await Task.Delay(1000);

            readyUnreadyLoadingBar.StopLoading();
            readyUnreadyButton.interactable = true;
        }

        private void SetReadyUnreadyButton(PlayerStatus status)
        {
            readyUnreadyText.text = status == PlayerStatus.Ready ? "READY" : "NOT READY";

            ColorBlock colors = readyUnreadyButton.colors;

            if (status == PlayerStatus.Ready)
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

            readyUnreadyButton.colors = colors;
        }
    }
}