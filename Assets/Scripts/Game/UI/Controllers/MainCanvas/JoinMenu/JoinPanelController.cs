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
        [SerializeField] private TextMeshProUGUI joinText;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TextMeshProUGUI refreshText;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private GameObject refreshingPanel;
        [SerializeField] private LoadingBar refeshingLoadingBar;
        [SerializeField] private LobbyListEntry lobbyListEntry;
        [SerializeField] private Transform lobbyListContainer;

        private string currentSelectedId = null;
        private LobbyListEntry currentSelectedEntry = null;

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
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || currentSelectedId != null;
        }

        /// <summary>
        /// Sets the selected lobby.
        /// </summary>
        /// param name="newSelectedId">The ID of the selected lobby.</param>
        /// <param name="newSelectedEntry">The entry of the selected lobby.</param>
        private void SelectLobby(string newSelectedId, LobbyListEntry newSelectedEntry)
        {
            if (currentSelectedEntry != null && currentSelectedEntry != newSelectedEntry)
                currentSelectedEntry.SetSelected(false);

            currentSelectedId = newSelectedId;
            currentSelectedEntry = newSelectedEntry;
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Clears the selected lobby.
        /// </summary>
        public void ClearSelection()
        {
            currentSelectedId = null;
            currentSelectedEntry = null;
            UpdateJoinButtonState();
        }

        /// <summary>
        /// Handles the join button click event and lobby double click. First tries to join by code, then by selected lobby.
        /// </summary>
        public async void OnJoinButtonClicked()
        {
            joinButton.interactable = false;
            joinText.text = "Joining...";

            await Task.Delay(1500);
            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                GameLobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                GameLobbyManager.Instance.JoinLobbyById(currentSelectedId);

            joinText.text = "Join";
            UpdateJoinButtonState();
        }


        /// <summary>
        /// Handles the refresh button click event.
        /// </summary>
        private async void OnRefreshButtonClicked()
        {
            ClearSelection();

            joinButton.interactable = false;
            refreshButton.interactable = false;
            refreshText.text = "Refreshing...";
            refreshingPanel.SetActive(true);
            refeshingLoadingBar.StartLoading();

            await Task.Delay(1500);
            List<Lobby> lobbies = await GameLobbyManager.Instance.GetLobbies();

            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>())
                DeleteLobbyListEntry(entry);

            foreach (Lobby lobby in lobbies) CreateLobbyListEntry(lobby);

            refeshingLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshText.text = "Refresh";
            refreshButton.interactable = true;
            UpdateJoinButtonState();
        }

        private void DeleteLobbyListEntry(LobbyListEntry entry)
        {
            entry.lobbySingleClicked -= SelectLobby;
            entry.lobbyDoubleClicked -= OnJoinButtonClicked;

            Destroy(entry.gameObject);
        }

        private void CreateLobbyListEntry(Lobby lobby)
        {
            LobbyListEntry lobbyEntry = Instantiate(lobbyListEntry, lobbyListContainer);
            LayoutElement item = lobbyEntry.GetComponent<LayoutElement>();
            item.preferredHeight = item.minHeight = Screen.height * 0.15f;
            lobbyEntry.SetLobby(lobby);

            lobbyEntry.lobbySingleClicked += SelectLobby;
            lobbyEntry.lobbyDoubleClicked += OnJoinButtonClicked;
        }
    }
}