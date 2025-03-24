using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the UI for the join panel.
    /// </summary>
    public class JoinPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private Button joinButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private LoadingBar joinLoadingBar;
        [SerializeField] private LoadingBar refreshLoadingBar;
        [SerializeField] private TMP_InputField lobbyCodeInput;

        [Header("List Manager")]
        [SerializeField] private LobbyListManager lobbyListManager;

        protected override void OnEnable()
        {
            base.OnEnable();
            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);

            LobbyEvents.OnLobbyListUpdated += OnLobbyListUpdated;
            Events.LobbyEvents.OnLobbySelected += OnLobbySelected;
            Events.LobbyEvents.OnLobbyDoubleClicked += OnLobbyDoubleClicked;

            OnRefreshButtonClicked();
            UpdateJoinButtonState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

            LobbyEvents.OnLobbyListUpdated -= OnLobbyListUpdated;
            Events.LobbyEvents.OnLobbySelected -= OnLobbySelected;
            Events.LobbyEvents.OnLobbyDoubleClicked -= OnLobbyDoubleClicked;

            joinLoadingBar.StopLoading();
            refreshLoadingBar.StopLoading();
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || lobbyListManager.SelectedLobbyId != null;
        }

        /// <summary>
        /// Handles the join button click event. First tries to join by code, then by selected lobby.
        /// </summary>
        private async void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            joinLoadingBar.StartLoading();
            OperationResult result;
            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                result = await GameLobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                result = await GameLobbyManager.Instance.JoinLobbyById(lobbyListManager.SelectedLobbyId);
            joinLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Error)
                NotificationManager.Instance.HandleResult(result);

            joinButton.interactable = true;
        }

        /// <summary>
        /// Updates the lobby list UI with the provided lobbies.
        /// </summary>
        /// <param name="lobbies">The list of lobbies to display.</param>
        private void OnLobbyListUpdated(List<Lobby> lobbies)
        {
            lobbyListManager.UpdateLobbyList(lobbies);
        }

        /// <summary>
        /// Handles the lobby selected (single click) event.
        /// </summary>
        private void OnLobbySelected(string lobbyId, LobbyListEntry lobbyListEntry)
        {
            lobbyListManager.SelectLobby(lobbyId);
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the lobby double clicked event.
        /// </summary>
        private async void OnLobbyDoubleClicked(string lobbyId)
        {
            joinButton.interactable = false;

            lobbyListManager.SelectLobby(lobbyId);

            joinLoadingBar.StartLoading();
            var result = await GameLobbyManager.Instance.JoinLobbyById(lobbyId);
            joinLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Error)
                NotificationManager.Instance.HandleResult(result);

            joinButton.interactable = true;
        }

        /// <summary>
        /// Handles the refresh button click event.
        /// </summary>
        private async void OnRefreshButtonClicked()
        {
            refreshButton.interactable = false;

            refreshLoadingBar.StartLoading();
            OperationResult result = await lobbyListManager.RefreshLobbyList();
            UpdateJoinButtonState();

            await Task.Delay(1000);
            NotificationManager.Instance.HandleResult(result);
            refreshLoadingBar.StopLoading();

            refreshButton.interactable = true;

        }
    }
}