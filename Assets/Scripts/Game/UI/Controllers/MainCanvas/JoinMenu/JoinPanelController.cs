using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
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

            OnRefreshButtonClicked();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

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
            lobbyCodeInput.interactable = false;
            refreshButton.interactable = false;
            joinButton.interactable = false;
            joinText.text = "Joining...";
            joinLoadingBar.StartLoading();
            refreshingPanel.SetActive(true);
            refreshingLoadingBar.StartLoading();

            await Task.Delay(1000);

            OperationResult result;
            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                result = await LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                result = await LobbyManager.Instance.JoinLobbyById(currentSelectedId);

            joinLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshingLoadingBar.StopLoading();
            NotificationManager.Instance.ShowNotification(result);
            if (result.Status == ResultStatus.Success) SceneManager.LoadSceneAsync("Lobby");
            else
            {
                joinText.text = "Join";
                refreshButton.interactable = true;
                lobbyCodeInput.interactable = true;
                base.UpdateInteractable(true);

                UpdateJoinButtonState();
            }
        }

        private async void OnRefreshButtonClicked()
        {
            ClearSelection();

            base.UpdateInteractable(false);
            lobbyCodeInput.interactable = false;
            refreshButton.interactable = false;
            joinButton.interactable = false;
            refreshText.text = "Refreshing...";
            refreshLoadingBar.StartLoading();
            refreshingPanel.SetActive(true);
            refreshingLoadingBar.StartLoading();

            await Task.Delay(1000);

            List<Lobby> lobbies = await LobbyManager.Instance.QueryLobbies();
            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>()) DeleteLobbyListEntry(entry);
            foreach (Lobby lobby in lobbies) CreateLobbyListEntry(lobby);

            NotificationManager.Instance.ShowNotification(OperationResult.SuccessResult("Lobbies refreshed", lobbies.Count + lobbies.Count == 1 ? " lobby" : " lobbies" + " found"));

            refreshingLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshLoadingBar.StopLoading();
            refreshText.text = "Refresh";
            refreshButton.interactable = true;
            lobbyCodeInput.interactable = true;
            base.UpdateInteractable(true);

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
            item.preferredHeight = item.minHeight = Screen.height * 0.07f;
            lobbyEntry.SetLobby(lobby);

            lobbyEntry.lobbySingleClicked += SelectLobby;
            lobbyEntry.lobbyDoubleClicked += OnJoinButtonClicked;
        }
    }
}