using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Testing;
using System.Threading.Tasks;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

// Todo
// 1. Fix the issue when the join button remains active after cllicking off (unselecting) a lobby
// 2. Fix the issue where the lobby list sometimes gets 0 lobbies after refreshing
// 3. ?

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the logic for the join panel.
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

        /// <summary>
        /// The currently selected lobby.
        /// </summary>
        private Lobby selectedLobby;
        private readonly List<LobbyListEntry> lobbyList = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);

            OnRefreshButtonClicked();
            UpdateJoinButtonState();

            LobbyEvents.OnLobbySelected += OnLobbySelected;
            LobbyEvents.OnLobbyDoubleClicked += OnLobbyDoubleClicked;
            Framework.Events.LobbyEvents.OnLobbyListChanged += OnLobbyListChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

            LobbyEvents.OnLobbySelected -= OnLobbySelected;
            LobbyEvents.OnLobbyDoubleClicked -= OnLobbyDoubleClicked;
            Framework.Events.LobbyEvents.OnLobbyListChanged -= OnLobbyListChanged;
        }

        private void OnLobbyListChanged(List<Lobby> lobbies)
        {
            UpdateLobbyList(lobbies);
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
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || selectedLobby != null;
        }

        /// <summary>
        /// Handles the join button click event. First tries to join by code, then by selected lobby.
        /// </summary>
        private async void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            joinLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                await JoinByCode(lobbyCodeInput.text);
            else if (selectedLobby != null)
                await JoinSelectedLobby();
            joinLoadingBar.StopLoading();

            joinButton.interactable = true;
        }

        /// <summary>
        /// Joins a lobby by code.
        /// </summary>
        private async Task JoinByCode(string code)
        {
            OperationResult result = await GameLobbyManager.Instance.JoinLobbyByCode(code);

            if (result.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);
        }

        /// <summary>
        /// Joins the selected (single or double clicked) lobby.
        /// </summary>
        private async Task JoinSelectedLobby()
        {
            if (selectedLobby == null) return;

            OperationResult result = await GameLobbyManager.Instance.JoinLobbyById(selectedLobby.Id);

            if (result.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);
        }

        /// <summary>
        /// Updates the lobby list with the provided lobbies.
        /// </summary>
        /// <param name="lobbies">The list of lobbies to display.</param>
        private void UpdateLobbyList(List<Lobby> lobbies)
        {
            selectedLobby = null;

            foreach (var item in lobbyList)
                Destroy(item.gameObject);

            lobbyList.Clear();

            foreach (var lobby in lobbies)
            {
                var lobbyItemObject = Instantiate(lobbyItemPrefab, lobbyListContainer);
                var lobbyItem = lobbyItemObject.GetComponent<LobbyListEntry>();
                lobbyItem.SetLobby(lobby);
                lobbyList.Add(lobbyItem);
            }

            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the lobby selected (single click) event.
        /// </summary>
        private void OnLobbySelected(Lobby lobby, LobbyListEntry lobbyListEntry)
        {
            foreach (var item in lobbyList)
                if (item != lobbyListEntry)
                    item.isSelected = false;

            selectedLobby = lobby;
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the lobby double clicked event.
        /// </summary>
        private async void OnLobbyDoubleClicked(Lobby lobby)
        {
            joinButton.interactable = false;

            selectedLobby = lobby;
            await JoinSelectedLobby();

            joinButton.interactable = true;
        }

        private async void OnRefreshButtonClicked()
        {
            refreshButton.interactable = false;

            refreshLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            var result = await GameLobbyManager.Instance.RefreshLobbyList();
            refreshLoadingBar.StopLoading();

            resultHandler.HandleResult(result);
            refreshButton.interactable = true;
        }
    }
}