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
using System.Linq;
using Unity.Netcode;
using UnityEngine.SceneManagement;

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
        [SerializeField] protected Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private TextMeshProUGUI readyUnreadyButtonText;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;
        [SerializeField] private Countdown countdown;

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

            countdown.OnCountdownComplete += OnCountdownComplete;

            AddHostListeners();

            countdown.ShowNotReadyMessage();
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

            countdown.OnCountdownComplete -= OnCountdownComplete;

            RemoveHostListeners();
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
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.LobbyCode}";

            OnLobbyUpdated(LobbyManager.Instance.lobby);
            UpdateReadyButton(false);
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            leaveLoadingBar.StartLoading();
            // await Tests.TestDelay(1000);
            OperationResult result = await LobbyManager.Instance.LeaveLobby();
            leaveLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            leaveButton.interactable = true;
        }

        private void OnLobbySettingsClicked()
        {
            lobbySettingsPanelController.ShowPanel();
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = LobbyManager.Instance.LobbyName;

            UpdatePlayerList(lobby);
            UpdateHostUI();

            if (lobby.Data != null && lobby.Data.ContainsKey("CountdownStartTime") && !countdown.IsCountdownActive)
            {
                Debug.Log("Received countdown start from lobby data");
                countdown.StartCountdown();
            }
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

        #region Ready/Countdown Management

        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;

            readyUnreadyLoadingBar.StartLoading();
            OperationResult result = await GameLobbyManager.Instance.ToggleReadyStatus();

            await Task.Delay(500);


            OnLobbyUpdated(LobbyManager.Instance.lobby);
            UpdateReadyButton(GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.PlayerId));
            GameLobbyManager.Instance.IsLobbyReady();

            readyUnreadyLoadingBar.StopLoading();

            await Task.Delay(2000);
            readyUnreadyButton.interactable = true;
        }

        private void UpdateReadyButton(bool isReady)
        {
            readyUnreadyButtonText.text = isReady ? "READY" : "NOT READY";

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

        private void OnLobbyReady()
        {
            Debug.Log("All players are now ready - starting countdown");

            // send system chat message

            if (LobbyManager.Instance.IsLobbyHost)
            {
                countdown.StartCountdown();
            }
        }

        private void OnLobbyNotReady()
        {
            Debug.Log("Not all players are ready - cancelling countdown");

            // send system chat message

            countdown.CancelCountdown();
            countdown.ShowNotReadyMessage();
        }

        private async void CancelCountdownAndUnreadyAll()
        {
            countdown.CancelCountdown();

            // send system chat message

            if (LobbyManager.Instance.IsLobbyHost)
                await GameLobbyManager.Instance.SetAllPlayersUnready();
        }

        private async void OnCountdownComplete()
        {
            Debug.Log("Countdown complete - starting game");

            if (LobbyManager.Instance.IsLobbyHost)
            {
                try
                {
                    // Create relay session
                    string joinCode = await RelayManager.Instance.CreateRelaySession(LobbyManager.Instance.MaxPlayers);

                    // Check if we got a valid join code
                    if (string.IsNullOrEmpty(joinCode))
                    {
                        Debug.LogError("Failed to get relay join code");
                        countdown.ShowNotReadyMessage();
                        return; // Exit early if we don't have a join code
                    }

                    // Update lobby with join code
                    await GameLobbyManager.Instance.SetGameInProgress(true, joinCode);

                    // Start networking as host
                    NetworkManager.Singleton.StartHost();

                    // Load game scene
                    await LoadGameSceneAdditively();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error starting host: {ex.Message}");
                    countdown.ShowNotReadyMessage();
                }
            }
            else
            {
                try
                {
                    await WaitForRelayJoinCode();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error joining as client: {ex.Message}");
                    countdown.ShowNotReadyMessage();
                }
            }

            readyUnreadyButton.interactable = false;
        }

        private async Task WaitForRelayJoinCode()
        {
            float waitTime = 0;
            int refreshAttempts = 0;
            const float refreshInterval = 1.0f;
            const int maxRefreshAttempts = 10;

            Debug.Log("Client waiting for relay join code...");

            while (string.IsNullOrEmpty(LobbyManager.Instance.RelayJoinCode) && refreshAttempts < maxRefreshAttempts)
            {
                if (!string.IsNullOrEmpty(LobbyManager.Instance.RelayJoinCode))
                {
                    Debug.Log($"Received relay join code: {LobbyManager.Instance.RelayJoinCode}");
                    break;
                }

                await Task.Delay((int)(refreshInterval * 1000));
                waitTime += refreshInterval;
                refreshAttempts++;
            }

            if (string.IsNullOrEmpty(LobbyManager.Instance.RelayJoinCode))
            {
                Debug.LogError($"Failed to get relay join code after {maxRefreshAttempts} attempts");
                countdown.ShowNotReadyMessage();
                return;
            }

            Debug.Log("Attempting to join relay session...");
            bool success = await RelayManager.Instance.JoinRelaySession(LobbyManager.Instance.RelayJoinCode);

            if (success)
            {
                Debug.Log("Successfully joined relay session, starting client");
                NetworkManager.Singleton.StartClient();
                await LoadGameSceneAdditively();
            }
            else
            {
                Debug.LogError("Failed to join Relay session");
                countdown.ShowNotReadyMessage();
            }
        }

        private async Task LoadGameSceneAdditively()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
                await Task.Yield();

            Scene gameScene = SceneManager.GetSceneByName("Game");
            SceneManager.SetActiveScene(gameScene);

            gameObject.SetActive(false);
        }


        public async void ReturnToLobby()
        {
            gameObject.SetActive(true);

            SceneManager.UnloadSceneAsync("Game");

            readyUnreadyButton.interactable = true;
            UpdateReadyButton(false);
            countdown.ShowNotReadyMessage();

            if (LobbyManager.Instance.IsLobbyHost)
                await GameLobbyManager.Instance.SetGameInProgress(false);
        }

        #endregion
    }
}