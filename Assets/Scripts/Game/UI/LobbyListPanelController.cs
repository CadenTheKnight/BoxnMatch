using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game.UI
{
    public class LobbyListPanelController : BasePanelController
    {
        [SerializeField] private Transform lobbyListContent;
        [SerializeField] private GameObject lobbyListItemPrefab;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private LoadingBarAnimator joinLoadingBar;
        [SerializeField] private Button refreshButton;
        [SerializeField] private LoadingBarAnimator refreshLoadingBar;

        private List<LobbyListLobbyData> activeLobbies = new();
        private LobbyListLobbyData selectedLobby;
        private Loading loading;

        protected override void OnEnable()
        {
            base.OnEnable();
            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);
            // RefreshLobbyList();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);
        }

        private void Awake()
        {
            loading = gameObject.AddComponent<Loading>();
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            joinButton.interactable = !string.IsNullOrEmpty(lobbyCodeInput.text) || selectedLobby != null;
        }

        private void OnJoinButtonClicked()
        {
            string code = lobbyCodeInput.text;
            if (!string.IsNullOrEmpty(code))
            {
                JoinThroughCode(code);
            }

            // if (selectedLobby != null)
            // {
            //     JoinThroughList(selectedLobby);
            // }
        }

        private void OnRefreshButtonClicked()
        {
            Debug.Log("Refresh button clicked.");
            // RefreshLobbyList();
        }

        // private async void RefreshLobbyList()
        // {
        //     ClearLobbyList();
        //     activeLobbies = await GameLobbyManager.Instance.GetActiveLobbies();
        //     foreach (var lobby in activeLobbies)
        //     {
        //         GameObject lobbyItem = Instantiate(lobbyListItemPrefab, lobbyListContent);
        //         LobbyListItemController itemController = lobbyItem.GetComponent<LobbyListItemController>();
        //         itemController.SetLobbyData(lobby);
        //         itemController.OnLobbySelected += OnLobbySelected;
        //         itemController.OnLobbyDoubleClicked += OnLobbyDoubleClicked;
        //     }
        // }

        private void ClearLobbyList()
        {
            foreach (Transform child in lobbyListContent)
            {
                Destroy(child.gameObject);
            }
        }

        // private void OnLobbySelected(LobbyListLobbyData lobby)
        // {
        //     selectedLobby = lobby;
        //     UpdateJoinButtonState();
        // }

        // private void OnLobbyDoubleClicked(LobbyListLobbyData lobby)
        // {
        //     selectedLobby = lobby;
        //     OnJoinButtonClicked();
        // }

        private async void JoinThroughCode(string code)
        {
            loading.StartLoading(joinButton, joinLoadingBar);
            var result = await GameLobbyManager.Instance.JoinLobbyByCode(code);
            loading.StopLoading(joinButton, joinLoadingBar);

            if (result.Success)
            {
                Debug.Log($"Joined lobby using code {code}.");
                SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                Debug.LogError($"Failed to join lobby using code {code}.");
            }
        }

        // private async void JoinThroughList(LobbyData lobby)
        // {
        //     loading.StartLoading(joinButton, joinLoadingBar);
        //     var result = await GameLobbyManager.Instance.JoinLobbyById(lobby.Id);
        //     loading.StopLoading(joinButton, joinLoadingBar);

        //     if (result.Success)
        //     {
        //         Debug.Log($"Joined lobby using id {lobby.Id}.");
        //         SceneManager.LoadSceneAsync("Lobby");
        //     }
        //     else
        //     {
        //         Debug.LogError($"Failed to join lobby using id {lobby.Id}.");
        //     }
        // }
    }
}
