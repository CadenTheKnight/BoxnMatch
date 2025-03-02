
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Testing;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.LobbyMenu
{
    public class LobbyPanelController : MonoBehaviour
    {
        [SerializeField] private ResultHandler resultHandler;
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
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;
        [SerializeField] private LobbySettingsPanelController lobbySettingsPanelController;

        private void OnEnable()
        {
            readyButton.onClick.AddListener(OnReadyClicked);
            unreadyButton.onClick.AddListener(OnUnreadyClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            Events.LobbyEvents.OnAllPlayersReady += OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady += OnLobbyNotReady;

            AddHostListeners();
        }

        private void OnDisable()
        {
            readyButton.onClick.RemoveListener(OnReadyClicked);
            unreadyButton.onClick.RemoveListener(OnUnreadyClicked);
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            Events.LobbyEvents.OnAllPlayersReady -= OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady -= OnLobbyNotReady;

            RemoveHostListeners();
        }

        void Start()
        {
            OnLobbyUpdated(LobbyManager.Instance.lobby);

            startButton.interactable = false;
        }

        private async void OnReadyClicked()
        {
            readyButton.interactable = false;

            readyUnreadyLoadingBar.StartLoading();
            var result = await GameLobbyManager.Instance.SetPlayerReady(true);
            readyUnreadyLoadingBar.StopLoading();

            if (result.Success)
            {
                readyButton.gameObject.SetActive(false);
                unreadyButton.gameObject.SetActive(true);
            }
            else
            {
                readyButton.interactable = true;
            }
        }

        private async void OnUnreadyClicked()
        {
            unreadyButton.interactable = false;

            readyUnreadyLoadingBar.StartLoading();
            var result = await GameLobbyManager.Instance.SetPlayerReady(false);
            readyUnreadyLoadingBar.StopLoading();

            if (result.Success)
            {
                unreadyButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
            }
            else
            {
                unreadyButton.interactable = true;
            }
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            leaveLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            OperationResult result = await GameLobbyManager.Instance.LeaveLobby();
            leaveLoadingBar.StopLoading();

            if (result.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            leaveButton.interactable = true;
        }

        private void OnLobbySettingsClicked()
        {
            lobbySettingsPanelController.ShowPanel();
        }

        private void OnStartClicked()
        {
            Debug.Log("Start game");
        }

        private void OnLobbyReady()
        {
            if (GameLobbyManager.Instance.IsHost)
                startButton.interactable = true;
        }

        public void OnLobbyNotReady()
        {
            if (GameLobbyManager.Instance.IsHost)
                startButton.interactable = false;
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = GameLobbyManager.Instance.LobbyName;
            lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.LobbyCode}";

            UpdateHostUI();
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = GameLobbyManager.Instance.LobbyCode;
            resultHandler.HandleResult(new OperationResult(true, "LobbyCode", $"Lobby code: {GameLobbyManager.Instance.LobbyCode} copied to clipboard"));
        }

        private void UpdateHostUI()
        {
            if (GameLobbyManager.Instance.IsHost)
            {
                AddHostListeners();
                lobbySettingsButton.interactable = true;
            }
            else
            {
                RemoveHostListeners();
                lobbySettingsButton.interactable = false;
            }
        }

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