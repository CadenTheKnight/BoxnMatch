
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework;
using Assets.Scripts.Game.Events;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

namespace Assets.Scripts.Game.UI
{
    public class LobbyPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private Button lobbySettingsButton;
        // [SerializeField] private Image mapImage;
        // [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] protected Button leaveLobbyButton;
        [SerializeField] private LoadingBarAnimator leaveLoadingBar;
        [SerializeField] private Button startButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button unreadyButton;
        [SerializeField] private LoadingBarAnimator readyUnreadyLoadingBar;
        [SerializeField] private LobbySettingsPanelController lobbySettingsPanelController;
        private Loading loading;

        private void OnEnable()
        {
            readyButton.onClick.AddListener(OnReadyClicked);
            unreadyButton.onClick.AddListener(OnUnreadyClicked);
            leaveLobbyButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            LobbyEvents.OnLobbyReady += OnLobbyReady;
            LobbyEvents.OnLobbyNotReady += OnLobbyNotReady;

            AddHostListeners();
        }

        private void OnDisable()
        {
            readyButton.onClick.RemoveListener(OnReadyClicked);
            unreadyButton.onClick.RemoveListener(OnUnreadyClicked);
            leaveLobbyButton.onClick.RemoveListener(OnLeaveClicked);
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.OnLobbyReady -= OnLobbyReady;
            LobbyEvents.OnLobbyNotReady -= OnLobbyNotReady;

            RemoveHostListeners();
        }

        void Start()
        {
            loading = gameObject.AddComponent<Loading>();

            lobbyNameText.text = GameLobbyManager.Instance.GetLobbyName();
            lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.GetLobbyCode()}";

            UpdateHostUI();

            startButton.interactable = false;
        }

        private async void OnReadyClicked()
        {
            readyButton.interactable = false;

            loading.StartLoading(readyButton, readyUnreadyLoadingBar);
            var result = await GameLobbyManager.Instance.SetPlayerReady();
            loading.StopLoading(readyButton, readyUnreadyLoadingBar);

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

            loading.StartLoading(unreadyButton, readyUnreadyLoadingBar);
            var result = await GameLobbyManager.Instance.SetPlayerUnready();
            loading.StopLoading(unreadyButton, readyUnreadyLoadingBar);

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
            leaveLobbyButton.interactable = false;

            loading.StartLoading(leaveLobbyButton, leaveLoadingBar);
            var result = await GameLobbyManager.Instance.LeaveLobby(AuthenticationService.Instance.PlayerId);
            loading.StopLoading(leaveLobbyButton, leaveLoadingBar);

            if (result.Success)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                leaveLobbyButton.interactable = true;
                // TODO: Show error message
            }
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
            {
                startButton.interactable = true;
            }
        }

        public void OnLobbyNotReady()
        {
            if (GameLobbyManager.Instance.IsHost)
            {
                startButton.interactable = false;
            }
        }

        private void OnLobbyUpdated()
        {
            lobbyNameText.text = GameLobbyManager.Instance.GetLobbyName();
            lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.GetLobbyCode()}";

            UpdateHostUI();
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = GameLobbyManager.Instance.GetLobbyCode();
            Debug.Log("Lobby code copied to clipboard: " + GameLobbyManager.Instance.GetLobbyCode());
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