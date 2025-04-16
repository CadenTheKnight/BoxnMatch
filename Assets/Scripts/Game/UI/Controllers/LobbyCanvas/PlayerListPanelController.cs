using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    /// <summary>
    /// Handles the player list in the lobby.
    /// </summary>
    public class PlayerListPanelController : MonoBehaviour
    {
        [SerializeField] private List<PlayerListEntry> _playerListEntries = new();

        private void OnEnable()
        {
            LobbyEvents.OnHostMigrated += OnHostMigrated;
            LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            GameLobbyEvents.OnPlayerTeamChanged += OnPlayerTeamChanged;
            GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;
            GameLobbyEvents.OnPlayerConnectionStatusChanged += OnPlayerConnectionStatusChanged;

            ResetPlayerList();
        }

        private void OnDisable()
        {
            LobbyEvents.OnHostMigrated -= OnHostMigrated;
            LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            GameLobbyEvents.OnPlayerTeamChanged -= OnPlayerTeamChanged;
            GameLobbyEvents.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;
            GameLobbyEvents.OnPlayerConnectionStatusChanged -= OnPlayerConnectionStatusChanged;
        }

        private void ResetPlayerList()
        {
            List<Player> players = GameLobbyManager.Instance.Lobby.Players;
            for (int i = 0; i < _playerListEntries.Count; i++)
            {
                _playerListEntries[i].gameObject.SetActive(true);
                if (i < players.Count) _playerListEntries[i].SetPlayer(players[i].Id);
                else if (i < GameLobbyManager.Instance.Lobby.MaxPlayers) _playerListEntries[i].SetEmpty();
                else _playerListEntries[i].gameObject.SetActive(false);
            }
        }

        private void OnHostMigrated(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId) ResetPlayerList();
        }

        private void OnPlayerJoined(string playerId)
        {
            if (playerId != AuthenticationService.Instance.PlayerId) ResetPlayerList();
        }

        private void OnPlayerLeft(string playerId)
        {
            if (playerId != AuthenticationService.Instance.PlayerId) ResetPlayerList();
        }

        private void OnPlayerTeamChanged(bool success, string playerId, Team team)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetTeam(success, team);
        }

        private void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetReadyStatus(success, readyStatus);
        }

        private void OnPlayerConnectionStatusChanged(bool success, string playerId, ConnectionStatus connectionStatus)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetConnectionStatus(success, connectionStatus);
        }
    }
}