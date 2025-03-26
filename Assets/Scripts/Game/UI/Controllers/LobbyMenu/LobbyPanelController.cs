using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

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

        [Header("Game Settings Components")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Image mapThumbnailImage;
        [SerializeField] private TextMeshProUGUI mapName;
        [SerializeField] private MapSelectionData mapSelectionData;
        // [SerializeField] private Incrementer roundCountIncrementer;
        // [SerializeField] private TextMeshProUGUI roundCountText;

        // [SerializeField] private Selector gameModeSelector;

        [Header("Lobby Settings Components")]

        [Header("Footer Components")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startButtonText;
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
        }

        async void Start()
        {
            OnLobbyUpdated(LobbyManager.Instance.Lobby);

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
            // roundCountIncrementer.incrementButton.onClick.RemoveListener(OnRoundCountChanged);
            // roundCountIncrementer.decrementButton.onClick.RemoveListener(OnRoundCountChanged);

            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            bool isHost = LobbyManager.Instance.IsLobbyHost;
            if (isHost)
            {
                leftButton.gameObject.SetActive(true);
                rightButton.gameObject.SetActive(true);
                startButton.gameObject.SetActive(true);
                readyUnreadyButton.gameObject.SetActive(false);
                // roundCountIncrementer.incrementButton.interactable = true;
                // roundCountIncrementer.decrementButton.interactable = true;

                leftButton.onClick.AddListener(OnLeftClicked);
                rightButton.onClick.AddListener(OnRightClicked);
                startButton.onClick.AddListener(OnStartClicked);
                // roundCountIncrementer.incrementButton.onClick.AddListener(OnRoundCountChanged);
                // roundCountIncrementer.decrementButton.onClick.AddListener(OnRoundCountChanged);
            }
            else
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
                startButton.gameObject.SetActive(false);
                readyUnreadyButton.gameObject.SetActive(true);
                readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);
                // roundCountIncrementer.incrementButton.interactable = false;
                // roundCountIncrementer.decrementButton.interactable = false;

                bool isReady = GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.PlayerId);
                UpdateReadyButton(isReady);
            }

            leaveButton.gameObject.SetActive(true);
            lobbyCodeButton.gameObject.SetActive(true);
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            OperationResult result = await LobbyManager.Instance.LeaveLobby();

            if (result.Status == ResultStatus.Error)
                NotificationManager.Instance.HandleResult(result);

            leaveButton.interactable = true;
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = $"{LobbyManager.Instance.LobbyName}" + (LobbyManager.Instance.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"CODE: {LobbyManager.Instance.LobbyCode}";
            // roundCountText.text = $"{GameLobbyManager.Instance.GetRoundCount()}";

            currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            mapName.text = mapSelectionData.Maps[currentMapIndex].Name;
            mapThumbnailImage.sprite = mapSelectionData.Maps[currentMapIndex].Thumbnail;
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

            await GameLobbyManager.Instance.TogglePlayerReady();
            UpdateReadyButton(GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.PlayerId));

            await Task.Delay(2000);
            readyUnreadyButton.interactable = true;
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

            Debug.Log("Starting...");
            await Task.Delay(2000);
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

            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);

            await Task.Delay(1500);
            leftButton.interactable = true;
        }

        private async void OnRightClicked()
        {
            rightButton.interactable = false;
            if (currentMapIndex < mapSelectionData.Maps.Count - 1) currentMapIndex++;
            else currentMapIndex = 0;

            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);

            await Task.Delay(1500);
            rightButton.interactable = true;
        }
        #endregion

        #region Round Count
        // private async void OnRoundCountChanged()
        // {
        //     roundCountIncrementer.incrementButton.interactable = false;
        //     roundCountIncrementer.decrementButton.interactable = false;

        //     await GameLobbyManager.Instance.SetRoundCount(roundCountIncrementer.Value);

        //     await Task.Delay(1500);
        //     roundCountIncrementer.incrementButton.interactable = roundCountIncrementer.Value < roundCountIncrementer.maxValue;
        //     roundCountIncrementer.decrementButton.interactable = roundCountIncrementer.Value > roundCountIncrementer.minValue;
        // }
        #endregion
    }
}