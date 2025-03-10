using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Colors;
using Assets.Scripts.Game.Data;

namespace Assets.Scripts.Game.UI.Controllers.LobbyMenu
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        [Header("Player List")]
        [SerializeField] private Transform playerList;
        [SerializeField] private GameObject playerListEntry;

        [Header("Map Display")]
        [SerializeField] private Image mapImage;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TextMeshProUGUI mapName;
        [SerializeField] private MapSelectionData mapSelectionData;

        [Header("Footer")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button readyUnreadyButton;
        [SerializeField] private LoadingBar leaveLoadingBar;
        [SerializeField] private TextMeshProUGUI startButtonText;
        [SerializeField] private LoadingBar readyUnreadyLoadingBar;
        [SerializeField] private TextMeshProUGUI readyUnreadyButtonText;

        private int currentMapIndex = 0;
        [SerializeField] private ResultHandler resultHandler;

        private void OnEnable()
        {
            leftButton.onClick.AddListener(OnLeftClicked);
            rightButton.onClick.AddListener(OnRightClicked);
            startButton.onClick.AddListener(OnStartClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
            readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            // LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            // LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;

            Events.LobbyEvents.OnAllPlayersReady += OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady += OnLobbyNotReady;

            startButton.interactable = false;
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
            // LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            // LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;

            Events.LobbyEvents.OnAllPlayersReady -= OnLobbyReady;
            Events.LobbyEvents.OnNotAllPlayersReady -= OnLobbyNotReady;
        }

        private void OnPlayerJoined(Player player)
        {
        }

        private void OnPlayerLeft(Player player)
        {
        }

        async void Start()
        {
            lobbyNameText.text = $"{LobbyManager.Instance.LobbyName}" + (LobbyManager.Instance.IsPrivate ? " (Private)" : "");
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.LobbyCode}";

            if (!LobbyManager.Instance.IsLobbyHost)
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
            }
            else
                await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex, mapSelectionData.Maps[currentMapIndex].MapSceneName);

            UpdateReadyButton(false);
        }

        private async void OnLeaveClicked()
        {
            leaveButton.interactable = false;

            leaveLoadingBar.StartLoading();
            OperationResult result = await LobbyManager.Instance.LeaveLobby();
            leaveLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            leaveButton.interactable = true;
        }
        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = $"{LobbyManager.Instance.LobbyName}" + (LobbyManager.Instance.IsPrivate ? " (Private)" : "");
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.LobbyCode}";

            currentMapIndex = GameLobbyManager.Instance.GetMapIndex();

            UpdateMap();
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.LobbyCode;
            resultHandler.HandleResult(OperationResult.SuccessResult("LobbyCode", $"Lobby code: {LobbyManager.Instance.LobbyCode} copied to clipboard"));
        }

        #region Ready Status Management

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

            startButtonText.text = $"{playersReady}/{maxPlayerCount} READY";

            if (LobbyManager.Instance.IsLobbyHost)
                startButton.interactable = false;
        }

        private async void OnStartClicked()
        {
            startButton.interactable = false;

            await GameLobbyManager.Instance.StartGame();

            await Task.Delay(2000);
            startButton.interactable = true;
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
            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex, mapSelectionData.Maps[currentMapIndex].MapSceneName);

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
            await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex, mapSelectionData.Maps[currentMapIndex].MapSceneName);

            await Task.Delay(1200);
            rightButton.interactable = true;
        }

        private void UpdateMap()
        {
            // send system chat message

            mapImage.color = mapSelectionData.Maps[currentMapIndex].MapThumbnail;
            mapName.text = mapSelectionData.Maps[currentMapIndex].MapName;
        }

        #endregion
    }
}