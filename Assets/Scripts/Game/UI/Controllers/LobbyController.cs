
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Game.Managers;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class LobbyPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private Button lobbySettingsButton;
        // [SerializeField] private Image mapImage;
        // [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] protected Button leaveButton;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private Button startButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button unreadyButton;
        // [SerializeField] private LoadingBarAnimator readyUnreadyLoadingBar;
        [SerializeField] private LobbySettingsPanelController lobbySettingsPanelController;
        // private Loading loading;

        private void OnEnable()
        {
            // readyButton.onClick.AddListener(OnReadyClicked);
            // unreadyButton.onClick.AddListener(OnUnreadyClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            // LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            // LobbyEvents.OnAllPlayersReady += OnLobbyReady;
            // LobbyEvents.OnNotAllPlayersReady += OnLobbyNotReady;

            AddHostListeners();
        }

        private void OnDisable()
        {
            // readyButton.onClick.RemoveListener(OnReadyClicked);
            // unreadyButton.onClick.RemoveListener(OnUnreadyClicked);
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            // LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            // LobbyEvents.OnAllPlayersReady -= OnLobbyReady;
            // LobbyEvents.OnNotAllPlayersReady -= OnLobbyNotReady;

            RemoveHostListeners();
        }

        void Start()
        {

            lobbyNameText.text = LobbyManager.Instance.LobbyName;
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.LobbyCode}";

            // UpdateHostUI();

            startButton.interactable = false;
        }

        // private async void OnReadyClicked()
        // {
        //     readyButton.interactable = false;

        //     loading.StartLoading(readyButton, readyUnreadyLoadingBar);
        //     await GameLobbyManager.Instance.SetPlayerReady(true);
        //     loading.StopLoading(readyButton, readyUnreadyLoadingBar);

        //     if (result.Success)
        //     {
        //         readyButton.gameObject.SetActive(false);
        //         unreadyButton.gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         readyButton.interactable = true;
        //     }
        // }

        // private async void OnUnreadyClicked()
        // {
        //     unreadyButton.interactable = false;

        //     loading.StartLoading(unreadyButton, readyUnreadyLoadingBar);
        //     await GameLobbyManager.Instance.SetPlayerReady(false);
        //     loading.StopLoading(unreadyButton, readyUnreadyLoadingBar);

        //     if (result.Success)
        //     {
        //         unreadyButton.gameObject.SetActive(false);
        //         readyButton.gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         unreadyButton.interactable = true;
        //     }
        // }

        private async void OnLeaveClicked()
        {
            var result = await GameLobbyManager.Instance.LeaveLobby();

            if (result.Success)
                Debug.Log($"{result.Code} - {result.Message}");
            else
                Debug.LogError($"{result.Code} - {result.Message}");
        }

        private void OnLobbySettingsClicked()
        {
            lobbySettingsPanelController.ShowPanel();
        }

        private void OnStartClicked()
        {
            Debug.Log("Start game");
        }

        // private void OnLobbyReady()
        // {
        //     if (GameLobbyManager.Instance.IsHost)
        //     {
        //         startButton.interactable = true;
        //     }
        // }

        // public void OnLobbyNotReady()
        // {
        //     if (GameLobbyManager.Instance.IsHost)
        //     {
        //         startButton.interactable = false;
        //     }
        // }

        // private void OnLobbyUpdated()
        // {
        //     lobbyNameText.text = GameLobbyManager.Instance.LobbyName;
        //     lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.LobbyCode}";

        //     UpdateHostUI();
        // }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.LobbyCode;
            Debug.Log($"Lobby code {LobbyManager.Instance.LobbyCode} copied to clipboard");
        }

        // private void UpdateHostUI()
        // {
        //     if (GameLobbyManager.Instance.IsHost)
        //     {
        //         AddHostListeners();
        //         lobbySettingsButton.interactable = true;
        //     }
        //     else
        //     {
        //         RemoveHostListeners();
        //         lobbySettingsButton.interactable = false;
        //     }
        // }

        private void AddHostListeners()
        {
            lobbySettingsButton.onClick.AddListener(OnLobbySettingsClicked);
            startButton.onClick.AddListener(OnStartClicked);
        }

        private void RemoveHostListeners()
        {
            lobbySettingsButton.onClick.RemoveListener(OnLobbySettingsClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
        }

        // private void UpdateMapImage()
        // {
        //     mapImage.sprite = mapSelectionData.MapThumbnail(GameLobbyManager.Instance.GetMapIndex());
        //     mapNameText.text = mapSelectionData.Color(GameLobbyManager.Instance.GetMapIndex());
        // }
    }
}