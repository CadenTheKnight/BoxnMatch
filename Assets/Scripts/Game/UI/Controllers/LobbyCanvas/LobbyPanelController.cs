using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI startText;
        [SerializeField] private LoadingBar startLoadingBar;
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationManager.Instance.LocalPlayer);
        }

        private void OnPlayerConnect(Player player)
        {
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
            LobbyEvents.OnPlayerStatusChanged += OnPlayerStatusChanged;

            OnPlayerStatusChanged(player);
            lobbyNameText.text = $"{LobbyManager.Instance.Lobby.Name}" + (LobbyManager.Instance.Lobby.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
        }

        private void OnPlayerDisconnect(Player player)
        {
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
            LobbyEvents.OnPlayerStatusChanged -= OnPlayerStatusChanged;
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;

            startLoadingBar.StopLoading();
        }

        private void OnLobbyHostMigrated(Player player)
        {
            if (player.Id == AuthenticationManager.Instance.LocalPlayer.Id) UpdateStartButtonInteractable();
        }

        private void OnPlayerStatusChanged(Player player)
        {
            int playersReady = GameLobbyManager.Instance.GetPlayersReady();
            int maxPlayers = LobbyManager.Instance.Lobby.MaxPlayers;
            if (playersReady < maxPlayers) startText.text = $"{playersReady} / {maxPlayers} Ready";
            else
            {
                if (AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId) UpdateStartButtonInteractable();
                startText.text = "Start Game";
            }
        }

        private void UpdateStartButtonInteractable()
        {
            startButton.interactable = GameLobbyManager.Instance.GetPlayersReady() == LobbyManager.Instance.Lobby.MaxPlayers;
        }

        private async void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.Lobby.LobbyCode;

            lobbyCodeText.text = $"Copied!";
            lobbyCodeText.color = UIColors.Green.One;

            await Task.Delay(1000);

            lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
            lobbyCodeText.color = Color.white;
        }
    }
}