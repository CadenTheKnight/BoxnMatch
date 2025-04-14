using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
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
        [SerializeField] private LoadingBar joinLoadingBar;
        [SerializeField] private TextMeshProUGUI joinText;
        [SerializeField] private Button refreshButton;
        [SerializeField] private LoadingBar refreshLoadingBar;
        [SerializeField] private TextMeshProUGUI refreshText;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private GameObject refreshingPanel;
        [SerializeField] private LoadingBar refreshingLoadingBar;
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

            LobbyEvents.OnLobbiesQueried += OnLobbiesQueried;
            LobbyEvents.OnLobbyJoined += OnLobbyJoined;

            UpdateJoinButtonState();
            OnRefreshButtonClicked();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

            LobbyEvents.OnLobbiesQueried -= OnLobbiesQueried;
            LobbyEvents.OnLobbyJoined -= OnLobbyJoined;

            ClearSelection();
            joinLoadingBar.StopLoading();
            refreshLoadingBar.StopLoading();
            refreshingLoadingBar.StopLoading();
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            joinText.text = lobbyCodeInput.text.Length == 6 ? "Join by Code" : currentSelectedId != null ? "Join Selected" : "Join";
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || currentSelectedId != null;
        }

        private void SelectLobby(string newSelectedId, LobbyListEntry newSelectedEntry)
        {
            if (currentSelectedEntry != null && currentSelectedEntry != newSelectedEntry)
                currentSelectedEntry.SetSelected(false);

            currentSelectedId = newSelectedId;
            currentSelectedEntry = newSelectedEntry;
            UpdateJoinButtonState();
        }

        public void ClearSelection()
        {
            currentSelectedId = null;
            currentSelectedEntry = null;
            UpdateJoinButtonState();
        }

        public async void OnJoinButtonClicked()
        {
            base.UpdateInteractable(false);
            joinButton.interactable = false;
            refreshButton.interactable = false;
            lobbyCodeInput.interactable = false;
            joinText.text = "Joining...";
            joinLoadingBar.StartLoading();
            refreshingPanel.SetActive(true);
            refreshingLoadingBar.StartLoading();

            await Task.Delay(1000);

            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6) await LobbyManager.JoinLobbyByCode(lobbyCodeInput.text);
            else await LobbyManager.JoinLobbyById(currentSelectedId);
        }

        private async void OnRefreshButtonClicked()
        {
            ClearSelection();

            base.UpdateInteractable(false);
            joinButton.interactable = false;
            refreshButton.interactable = false;
            lobbyCodeInput.interactable = false;
            refreshText.text = "Refreshing...";
            refreshLoadingBar.StartLoading();
            refreshingPanel.SetActive(true);
            refreshingLoadingBar.StartLoading();

            await Task.Delay(1000);

            await LobbyManager.QueryLobbies();
        }

        private void OnLobbiesQueried(OperationResult result)
        {
            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>()) DeleteLobbyListEntry(entry);
            foreach (Lobby lobby in (List<Lobby>)result.Data) CreateLobbyListEntry(lobby);

            refreshText.text = "Refresh";
            refreshingLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshLoadingBar.StopLoading();
            UpdateJoinButtonState();
            lobbyCodeInput.interactable = true;
            base.UpdateInteractable(true);

            refreshButton.interactable = true;
        }

        private void OnLobbyJoined(OperationResult result)
        {
            if (result.Status == ResultStatus.Error)
            {
                joinText.text = "Join";
                refreshingPanel.SetActive(false);
                refreshingLoadingBar.StopLoading();
                joinLoadingBar.StopLoading();
                lobbyCodeInput.interactable = true;
                refreshButton.interactable = true;
                base.UpdateInteractable(true);

                UpdateJoinButtonState();
            }
        }

        private void CreateLobbyListEntry(Lobby lobby)
        {
            LobbyListEntry lobbyEntry = Instantiate(lobbyListEntry, lobbyListContainer);
            LayoutElement item = lobbyEntry.GetComponent<LayoutElement>();
            item.preferredHeight = item.minHeight = Screen.height * 0.07f;
            lobbyEntry.SetLobby(lobby);

            lobbyEntry.lobbySingleClicked += SelectLobby;
            lobbyEntry.lobbyDoubleClicked += OnJoinButtonClicked;
        }

        private void DeleteLobbyListEntry(LobbyListEntry entry)
        {
            entry.lobbySingleClicked -= SelectLobby;
            entry.lobbyDoubleClicked -= OnJoinButtonClicked;

            Destroy(entry.gameObject);
        }
    }
}