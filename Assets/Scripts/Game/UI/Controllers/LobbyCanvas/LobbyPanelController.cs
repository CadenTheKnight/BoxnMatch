using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header Components")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        [Header("Main Components")]
        [SerializeField] private GameSettingsPanelController gameSettingsPanelController;


        [Header("Footer Components")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startReadyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startReadyUnreadyText;

        private void OnEnable()
        {
            ConfigureUIBasedOnHostStatus();

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            LobbyEvents.OnHostMigrated += OnHostMigrated;
        }

        private void OnDisable()
        {
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.OnHostMigrated -= OnHostMigrated;
        }

        void Start()
        {
            OnLobbyUpdated(LobbyManager.Instance.Lobby);

            // if (LobbyManager.Instance.IsLobbyHost)
            //     await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);
        }

        private void OnHostMigrated(string newHostId)
        {
            Debug.Log($"Host migrated to: {newHostId}. Local player ID: {AuthenticationManager.Instance.LocalPlayer.Id}");

            bool isNowHost = newHostId == AuthenticationManager.Instance.LocalPlayer.Id;
            ConfigureUIBasedOnHostStatus();

            // if (isNowHost)
            //     NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("HostMigration", "You are now the lobby host"));
            // else
            //     NotificationManager.Instance.HandleResult(OperationResult.WarningResult("HostMigration", "The lobby has a new host"));
        }

        private void ConfigureUIBasedOnHostStatus()
        {
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            bool isHost = LobbyManager.Instance.IsLobbyHost;
            if (isHost)
            {

            }
            else
            {

                bool isReady = GameLobbyManager.Instance.IsPlayerReady(AuthenticationManager.Instance.LocalPlayer.Id);
                // UpdateReadyButton(isReady);
            }

            leaveButton.gameObject.SetActive(true);
            lobbyCodeButton.gameObject.SetActive(true);
        }


        private void OnLobbyUpdated(Lobby lobby)
        {
            lobbyNameText.text = $"{LobbyManager.Instance.LobbyName}" + (LobbyManager.Instance.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"CODE: {LobbyManager.Instance.LobbyCode}";
            // roundCountText.text = $"{GameLobbyManager.Instance.GetRoundCount()}";

            // currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            // mapName.text = mapSelectionData.Maps[currentMapIndex].Name;
            // mapThumbnailImage.sprite = mapSelectionData.Maps[currentMapIndex].Thumbnail;
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.LobbyCode;
            // NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("LobbyCode", $"Lobby code: {LobbyManager.Instance.LobbyCode} copied to clipboard"));
        }
    }
}