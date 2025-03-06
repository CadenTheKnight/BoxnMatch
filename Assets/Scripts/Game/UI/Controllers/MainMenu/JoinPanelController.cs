using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Testing;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
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
        [SerializeField] private Button joinButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private LoadingBar joinLoadingBar;
        [SerializeField] private GameObject lobbyItemPrefab;
        [SerializeField] private ResultHandler resultHandler;
        [SerializeField] private Transform lobbyListContainer;
        [SerializeField] private LoadingBar refreshLoadingBar;
        [SerializeField] private TMP_InputField lobbyCodeInput;


        protected override void OnEnable()
        {
            base.OnEnable();
            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);

            LobbyEvents.OnLobbyListChanged += UpdateLobbyList;
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

            LobbyEvents.OnLobbyListChanged -= UpdateLobbyList;
            Events.LobbyEvents.OnLobbySelected -= OnLobbySelected;
            Events.LobbyEvents.OnLobbyDoubleClicked -= OnLobbyDoubleClicked;
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Updates the state of the join button based on the current input/selection.
        /// </summary>
        private void UpdateJoinButtonState()
        {
            bool hasValidCode = lobbyCodeInput.text.Length == 6;
            bool hasSelectedLobby = LobbyListManager.Instance.SelectedLobby != null;
            joinButton.interactable = hasValidCode || hasSelectedLobby;
        }

        /// <summary>
        /// Handles the join button click event. First tries to join by code, then by selected lobby.
        /// </summary>
        private async void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            joinLoadingBar.StartLoading();
            await Tests.TestDelay(1000);

            OperationResult result;
            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                result = await LobbyListManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                result = await LobbyListManager.Instance.JoinSelectedLobby(LobbyListManager.Instance.SelectedLobby.Id);

            joinLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            joinButton.interactable = true;
        }

        /// <summary>
        /// Updates the lobby list UI with the provided lobbies.
        /// </summary>
        /// <param name="lobbies">The list of lobbies to display.</param>
        private void UpdateLobbyList(List<Lobby> lobbies)
        {

            foreach (Transform t in lobbyListContainer)
                Destroy(t.gameObject);

            foreach (Lobby lobby in lobbies)
            {
                var lobbyItemObject = Instantiate(lobbyItemPrefab, lobbyListContainer);
                var lobbyItem = lobbyItemObject.GetComponent<LobbyListEntry>();
                lobbyItem.SetLobby(lobby);
            }

            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the lobby selected (single click) event.
        /// </summary>
        private void OnLobbySelected(Lobby lobby, LobbyListEntry lobbyListEntry)
        {
            LobbyListManager.Instance.SelectLobby(lobby);

            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the lobby double clicked event.
        /// </summary>
        private async void OnLobbyDoubleClicked(Lobby lobby)
        {
            joinButton.interactable = false;

            LobbyListManager.Instance.SelectLobby(lobby);

            joinLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            var result = await LobbyListManager.Instance.JoinSelectedLobby(LobbyListManager.Instance.SelectedLobby.Id);
            joinLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            joinButton.interactable = true;
        }

        /// <summary>
        /// Handles the refresh button click event.
        /// </summary>
        private async void OnRefreshButtonClicked()
        {
            refreshButton.interactable = false;

            refreshLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            var result = await LobbyListManager.Instance.RefreshLobbyList();
            refreshLoadingBar.StopLoading();

            resultHandler.HandleResult(result);
            refreshButton.interactable = true;
        }
    }
}