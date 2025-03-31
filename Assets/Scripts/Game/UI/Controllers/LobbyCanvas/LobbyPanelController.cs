using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header Components")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        [Header("Main Components")]
        [SerializeField] private PlayerListPanelController playerListPanelController;
        [SerializeField] private GameSettingsPanelController gameSettingsPanelController;


        [Header("Footer Components")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startReadyUnreadyButton;
        [SerializeField] private TextMeshProUGUI startReadyUnreadyText;

        private void OnEnable()
        {
            ConfigureUIBasedOnHostStatus();

            LobbyEvents.OnLobbyRefreshed += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyRefreshed -= OnLobbyUpdated;
        }

        // private void OnHostMigrated(string newHostId)
        // {
        //     Debug.Log($"Host migrated to: {newHostId}. Local player ID: {AuthenticationManager.Instance.LocalPlayer.Id}");

        //     bool isNowHost = newHostId == AuthenticationManager.Instance.LocalPlayer.Id;
        //     ConfigureUIBasedOnHostStatus();

        //     // if (isNowHost)
        //     //     NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("HostMigration", "You are now the lobby host"));
        //     // else
        //     //     NotificationManager.Instance.HandleResult(OperationResult.WarningResult("HostMigration", "The lobby has a new host"));
        // }

        private void ConfigureUIBasedOnHostStatus()
        {
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            bool isHost = LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;
            if (isHost)
            {

            }
            else
            {

                bool isReady = AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.Ready.ToString();
                // UpdateReadyButton(isReady);
            }

            leaveButton.gameObject.SetActive(true);
            lobbyCodeButton.gameObject.SetActive(true);
        }


        private void OnLobbyUpdated()
        {
            lobbyNameText.text = $"{LobbyManager.Instance.Lobby.Name}" + (LobbyManager.Instance.Lobby.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"CODE: {LobbyManager.Instance.Lobby.LobbyCode}";
            // roundCountText.text = $"{GameLobbyManager.Instance.GetRoundCount()}";

            // currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            // mapName.text = mapSelectionData.Maps[currentMapIndex].Name;
            // mapThumbnailImage.sprite = mapSelectionData.Maps[currentMapIndex].Thumbnail;
        }

        private void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.Lobby.LobbyCode;
            // NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("LobbyCode", $"Lobby code: {LobbyManager.Instance.LobbyCode} copied to clipboard"));
        }
    }
}