using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.JoinMenu
{
    /// <summary>
    /// Handles the UI for the join panel.
    /// </summary>
    public class JoinPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private Button joinButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private GameObject refreshingPanel;
        [SerializeField] private LoadingBar refeshingLoadingBar;
        [SerializeField] private LobbyListEntry lobbyListEntry;
        [SerializeField] private Transform lobbyListContainer;

        private string selectedLobbyId = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);

            OnRefreshButtonClicked();
            UpdateJoinButtonState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

            ClearSelection();
            refeshingLoadingBar.StopLoading();
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Updates the state of the join button based on the input field and selected lobby.
        /// </summary>
        private void UpdateJoinButtonState()
        {
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || selectedLobbyId != null;
        }

        /// <summary>
        /// Sets the selected lobby.
        /// </summary>
        /// <param name="lobbyId">The ID of the selected lobby.</param>
        private void SelectLobby(string lobbyId)
        {
            selectedLobbyId = lobbyId;
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Clears the selected lobby.
        /// </summary>
        public void ClearSelection()
        {
            selectedLobbyId = null;
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the join button click event and lobby double click. First tries to join by code, then by selected lobby.
        /// </summary>
        public void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                GameLobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                GameLobbyManager.Instance.JoinLobbyById(selectedLobbyId);

            joinButton.interactable = true;
        }


        /// <summary>
        /// Handles the refresh button click event.
        /// </summary>
        private async void OnRefreshButtonClicked()
        {
            refreshButton.interactable = false;
            refreshingPanel.SetActive(true);
            refeshingLoadingBar.StartLoading();

            await Task.Delay(1500);
            List<Lobby> lobbies = await GameLobbyManager.Instance.GetLobbies();

            ClearSelection();

            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>())
            {
                entry.lobbySingleClicked -= SelectLobby;
                entry.lobbyDoubleClicked -= OnJoinButtonClicked;

                Destroy(entry.gameObject);
            }

            foreach (Lobby lobby in lobbies)
            {
                LobbyListEntry lobbyEntry = Instantiate(lobbyListEntry, lobbyListContainer);
                lobbyEntry.SetLobby(lobby);

                lobbyEntry.lobbySingleClicked += SelectLobby;
                lobbyEntry.lobbyDoubleClicked += OnJoinButtonClicked;
            }

            UpdateJoinButtonState();

            refeshingLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshButton.interactable = true;
        }
    }
}