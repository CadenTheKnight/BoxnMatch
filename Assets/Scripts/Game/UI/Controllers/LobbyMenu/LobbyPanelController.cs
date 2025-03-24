using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Colors;

namespace Assets.Scripts.Game.UI.Controllers.LobbyMenu
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header Components")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        [Header("Player List Components")]
        [SerializeField] private Transform playerList;
        [SerializeField] private GameObject playerListEntry;

        [Header("Map Display Components")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Image mapThumbnailImage;
        [SerializeField] private TextMeshProUGUI mapName;
        [SerializeField] private MapSelectionData mapSelectionData;

        [Header("Footer Components")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startButtonText;
        [SerializeField] private LoadingBar leaveButtonLoadingBar;
        [SerializeField] private LoadingBar startButtonLoadingBar;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;
        [SerializeField] private TextMeshProUGUI lobbyGameModeText;
        [SerializeField] private TextMeshProUGUI lobbyRoundCountText;
        [SerializeField] private TextMeshProUGUI readyUnreadyButtonText;

        private int currentMapIndex = 0;

        private void OnEnable()
        {
            ConfigureUIBasedOnHostStatus();

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            LobbyEvents.OnHostMigrated += OnHostMigrated;
            Events.LobbyEvents.OnAllPlayersReady += OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady += OnLobbyNotReady;
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(OnLeftClicked);
            rightButton.onClick.RemoveListener(OnRightClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.OnHostMigrated -= OnHostMigrated;
            Events.LobbyEvents.OnAllPlayersReady -= OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady -= OnLobbyNotReady;

            startButtonLoadingBar.StopLoading();
            leaveButtonLoadingBar.StopLoading();
            readyUnreadyLoadingBar.StopLoading();
        }

        async void Start()
        {
            OnLobbyUpdated(LobbyManager.Instance.lobby);

            if (LobbyManager.Instance.IsLobbyHost)
                await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);
        }

        private void OnHostMigrated(string newHostId)
        {
            Debug.Log($"Host migrated to: {newHostId}. Local player ID: {AuthenticationManager.Instance.PlayerId}");

            bool isNowHost = newHostId == AuthenticationManager.Instance.PlayerId;
            ConfigureUIBasedOnHostStatus();

            if (isNowHost)
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("HostMigration", "You are now the lobby host"));
            else
                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("HostMigration", "The lobby has a new host"));
        }

        private void ConfigureUIBasedOnHostStatus()
        {
            leftButton.onClick.RemoveListener(OnLeftClicked);
            rightButton.onClick.RemoveListener(OnRightClicked);
            startButton.onClick.RemoveListener(OnStartClicked);
            readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            bool isHost = LobbyManager.Instance.IsLobbyHost;
            Debug.Log($"Configuring UI. Is Host: {isHost}");

            if (isHost)
            {
                leftButton.gameObject.SetActive(true);
                rightButton.gameObject.SetActive(true);
                startButton.gameObject.SetActive(true);
                readyUnreadyButton.gameObject.SetActive(false);

                leftButton.onClick.AddListener(OnLeftClicked);
                rightButton.onClick.AddListener(OnRightClicked);
                startButton.onClick.AddListener(OnStartClicked);

                startButton.interactable = false;
            }
            else
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
                startButton.gameObject.SetActive(false);
                readyUnreadyButton.gameObject.SetActive(true);

                readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

                bool isReady = GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.PlayerId);
                UpdateReadyButton(isReady);
            }

            leaveButton.gameObject.SetActive(true);
            lobbyCodeButton.gameObject.SetActive(true);
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            leaveButtonLoadingBar.StartLoading();
            OperationResult result = await LobbyManager.Instance.LeaveLobby();
            leaveButtonLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Error)
                NotificationManager.Instance.HandleResult(result);

            leaveButton.interactable = true;
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = $"{LobbyManager.Instance.LobbyName}" + (LobbyManager.Instance.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"CODE: {LobbyManager.Instance.LobbyCode}";
            lobbyRoundCountText.text = $"{LobbyManager.Instance.RoundCount}" + (LobbyManager.Instance.RoundCount > 1 ? " ROUNDS" : " ROUND");
            lobbyGameModeText.text = $"{LobbyManager.Instance.GameMode} MODE";

            currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            UpdateMap();
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.LobbyCode;
            NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("LobbyCode", $"Lobby code: {LobbyManager.Instance.LobbyCode} copied to clipboard"));
        }

        #region Ready Status
        private async void OnReadyUnreadyClicked()
        {
            readyUnreadyButton.interactable = false;
            readyUnreadyLoadingBar.StartLoading();

            await GameLobbyManager.Instance.TogglePlayerReady();
            UpdateReadyButton(GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.PlayerId));

            await Task.Delay(2000);
            readyUnreadyButton.interactable = true;
            readyUnreadyLoadingBar.StopLoading();
        }

        private void UpdateReadyButton(bool isReady)
        {
            readyUnreadyButtonText.text = isReady ? "READY" : "NOT READY";

            ColorBlock colors = readyUnreadyButton.colors;

            if (isReady)
            {
                colors.normalColor = UIColors.greenDefaultColor;
                colors.highlightedColor = UIColors.greenHoverColor;
                colors.pressedColor = UIColors.greenDefaultColor;
                colors.selectedColor = UIColors.greenHoverColor;
            }
            else
            {
                colors.normalColor = UIColors.redDefaultColor;
                colors.highlightedColor = UIColors.redHoverColor;
                colors.pressedColor = UIColors.redDefaultColor;
                colors.selectedColor = UIColors.redHoverColor;
            }

            readyUnreadyButton.colors = colors;
        }

        private void OnLobbyReady()
        {
            // send system chat message

            startButtonText.text = "START";

            if (LobbyManager.Instance.IsLobbyHost)
                startButton.interactable = true;
        }

        private void OnLobbyNotReady(int playersReady, int maxPlayerCount)
        {
            // send system chat message

            startButtonText.text = $"{playersReady + 1}/{maxPlayerCount} READY";

            if (LobbyManager.Instance.IsLobbyHost)
                startButton.interactable = false;
        }

        private async void OnStartClicked()
        {
            startButton.interactable = false;

            startButtonLoadingBar.StartLoading();
            Debug.Log("Starting...");
            await Task.Delay(2000);
            startButtonLoadingBar.StopLoading();

            await GameLobbyManager.Instance.SetAllPlayersUnready();
            // await GameLobbyManager.Instance.StartGame(mapSelectionData.Maps[currentMapIndex].MapName, mapSelectionData.Maps[currentMapIndex].MapThumbnail, mapSelectionData.Maps[currentMapIndex].MapSceneName);
        }
        #endregion

        #region Map Selection
        private async void OnLeftClicked()
        {
            leftButton.interactable = false;
            if (currentMapIndex > 0)
                currentMapIndex--;
            else
                currentMapIndex = mapSelectionData.Maps.Count - 1;

            UpdateMap();
            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);

            await Task.Delay(1200);
            leftButton.interactable = true;
        }

        private async void OnRightClicked()
        {
            rightButton.interactable = false;
            if (currentMapIndex < mapSelectionData.Maps.Count - 1)
                currentMapIndex++;
            else
                currentMapIndex = 0;

            UpdateMap();
            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);

            await Task.Delay(1200);
            rightButton.interactable = true;
        }

        private void UpdateMap()
        {
            // send system chat message
            mapName.text = mapSelectionData.Maps[currentMapIndex].Name;
            mapThumbnailImage.sprite = mapSelectionData.Maps[currentMapIndex].Thumbnail;
        }
        #endregion
    }
}