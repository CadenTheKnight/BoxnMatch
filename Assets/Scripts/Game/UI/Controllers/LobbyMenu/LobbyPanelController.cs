using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Testing;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.UI.Controllers.LobbyMenu
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private Button lobbySettingsButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private LobbySettingsPanelController lobbySettingsPanelController;

        [Header("Footer")]
        [SerializeField] private Button startButton;
        [SerializeField] protected Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private TextMeshProUGUI readyUnreadyButtonText;
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;

        [Header("Countdown Timer")]
        private Coroutine countdownCoroutine;
        private bool isCountdownActive = false;
        private const float COUNTDOWN_DURATION = 5f;

        [Header("Player List")]
        [SerializeField] private Transform playerListContent;
        [SerializeField] private GameObject playerListItemPrefab;

        [SerializeField] private ResultHandler resultHandler;

        private void OnEnable()
        {
            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            // LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            // LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
            Events.LobbyEvents.OnAllPlayersReady += OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady += OnLobbyNotReady;

            AddHostListeners();

            if (countdownPanel != null)
                countdownPanel.SetActive(false);
        }

        private void OnDisable()
        {
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            // LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            // LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
            Events.LobbyEvents.OnAllPlayersReady -= OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady -= OnLobbyNotReady;

            RemoveHostListeners();

            CancelCountdown();
        }

        private void OnPlayerJoined(Player player)
        {
            CancelCountdownAndUnreadyAll();

            UpdatePlayerList(LobbyManager.Instance.lobby);
        }

        private void OnPlayerLeft(Player player)
        {
            CancelCountdownAndUnreadyAll();

            UpdatePlayerList(LobbyManager.Instance.lobby);
        }

        void Start()
        {
            OnLobbyUpdated(LobbyManager.Instance.lobby);
            startButton.interactable = false;
            UpdateReadyButton();
        }

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;

            readyUnreadyLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            OperationResult result = await GameLobbyManager.Instance.ToggleReadyStatus();
            readyUnreadyLoadingBar.StopLoading();

            resultHandler.HandleResult(result);

            UpdateReadyButton();
            await Task.Delay(2000); // temporary delay to prevent spamming the ready button

            readyUnreadyButton.interactable = true;
        }

        private void UpdateReadyButton()
        {
            bool isReady = GameLobbyManager.Instance.IsPlayerReady();
            readyUnreadyButtonText.text = isReady ? "Unready" : "Ready";

            ColorBlock colors = readyUnreadyButton.colors;

            if (isReady)
            {
                colors.normalColor = UIColors.successDefaultColor;
                colors.highlightedColor = UIColors.successHoverColor;
                colors.pressedColor = UIColors.successDefaultColor;
                colors.selectedColor = UIColors.successHoverColor;
            }
            else
            {
                colors.normalColor = UIColors.errorDefaultColor;
                colors.highlightedColor = UIColors.errorHoverColor;
                colors.pressedColor = UIColors.errorDefaultColor;
                colors.selectedColor = UIColors.errorHoverColor;
            }

            readyUnreadyButton.colors = colors;
        }

        private void OnPlayerDataChanged(Player player)
        {
            UpdatePlayerList(LobbyManager.Instance.lobby);

            if (player.Id == AuthenticationManager.Instance.PlayerId)
            {
                UpdateReadyButton();
            }
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            leaveLoadingBar.StartLoading();
            await Tests.TestDelay(1000);
            OperationResult result = await LobbyManager.Instance.LeaveLobby();
            leaveLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            leaveButton.interactable = true;
        }

        public async void OnApplicationQuit()
        {
            await LobbyManager.Instance.LeaveLobby();
        }

        private void OnLobbySettingsClicked()
        {
            lobbySettingsPanelController.ShowPanel();
        }

        private void OnLobbyReady()
        {
            if (LobbyManager.Instance.IsLobbyHost)
                startButton.interactable = true;

            StartCountdown();
        }

        public void OnLobbyNotReady()
        {
            if (LobbyManager.Instance.IsLobbyHost)
                startButton.interactable = false;

            CancelCountdown();
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = LobbyManager.Instance.LobbyName;
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.LobbyCode}";

            UpdatePlayerList(lobby);
            UpdateHostUI();
            UpdateReadyButton();
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.LobbyCode;
            resultHandler.HandleResult(OperationResult.SuccessResult("LobbyCode", $"Lobby code: {LobbyManager.Instance.LobbyCode} copied to clipboard"));
        }

        private void UpdateHostUI()
        {
            if (LobbyManager.Instance.IsLobbyHost)
            {
                AddHostListeners();
                lobbySettingsButton.interactable = true;
                startButton.gameObject.SetActive(true);
            }
            else
            {
                RemoveHostListeners();
                lobbySettingsButton.interactable = false;
                startButton.gameObject.SetActive(false);
            }
        }

        private void AddHostListeners()
        {
            lobbySettingsButton.onClick.AddListener(OnLobbySettingsClicked);
        }

        private void RemoveHostListeners()
        {
            lobbySettingsButton.onClick.RemoveListener(OnLobbySettingsClicked);
        }

        #region Player List Management

        private void UpdatePlayerList(Lobby lobby)
        {
            foreach (Transform t in playerListContent)
                Destroy(t.gameObject);

            foreach (var player in lobby.Players)
            {
                GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
                PlayerListEntry playerListEntry = playerItem.GetComponent<PlayerListEntry>();
                playerListEntry.SetPlayer(player);
            }
        }

        #endregion

        #region Countdown Management
        private void StartCountdown()
        {
            if (isCountdownActive)
                return;

            CancelCountdown();

            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private void CancelCountdown()
        {
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            isCountdownActive = false;

            if (countdownPanel != null)
                countdownPanel.SetActive(false);
        }

        private async void CancelCountdownAndUnreadyAll()
        {
            CancelCountdown();

            if (LobbyManager.Instance.IsLobbyHost)
                await GameLobbyManager.Instance.SetAllPlayersUnready();
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountdownActive = true;

            if (countdownPanel != null)
                countdownPanel.SetActive(true);

            float remainingTime = COUNTDOWN_DURATION;

            while (remainingTime > 0)
            {
                if (countdownText != null)
                    countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

                yield return null;

                remainingTime -= Time.deltaTime;
            }

            isCountdownActive = false;

            if (countdownPanel != null)
                countdownPanel.SetActive(false);

            StartGame();
        }

        private async void StartGame()
        {
            Debug.Log("Game starting: (Countdown completed)");

            // temporary - reset the lobby ready status
            if (LobbyManager.Instance.IsLobbyHost)
                await GameLobbyManager.Instance.SetAllPlayersUnready();

            readyUnreadyButton.interactable = true;
            UpdateReadyButton();
        }
        #endregion
    }
}