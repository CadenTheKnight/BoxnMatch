using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Framework;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.UI.Controllers
{
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

        private void UpdateJoinButtonState()
        {
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || selectedLobby != null;
        }

        private async void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                await JoinByCode(lobbyCodeInput.text);
            else if (selectedLobby != null)
                await JoinSelectedLobby();

            joinButton.interactable = true;
        }

        private async Task JoinByCode(string code)
        {
            joinLoadingBar.StartLoading();
            await Task.Delay(1000); // Simulate loading
            var result = await GameLobbyManager.Instance.JoinLobbyByCode(code);
            joinLoadingBar.StopLoading();

            resultHandler.HandleResult(result);
        }

        private async Task JoinSelectedLobby()
        {
            if (selectedLobby == null) return;

            joinLoadingBar.StartLoading();
            await Task.Delay(1000); // Simulate loading
            var result = await GameLobbyManager.Instance.JoinLobbyById(selectedLobby.Id);
            joinLoadingBar.StopLoading();

            resultHandler.HandleResult(result);
        }

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

        private void OnLobbySelected(Lobby lobby, LobbyListEntry lobbyListEntry)
        {
            foreach (var item in lobbyList)
                if (item != lobbyListEntry)
                    item.isSelected = false;

            selectedLobby = lobby;
            UpdateJoinButtonState();
        }

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
            await Task.Delay(1000); // Simulate loading
            var result = await GameLobbyManager.Instance.RefreshLobbyList();
            refreshLoadingBar.StopLoading();

            resultHandler.HandleResult(result);
            refreshButton.interactable = true;
        }
    }
}