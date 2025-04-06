using UnityEngine;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    /// <summary>
    /// Handles the player list in the lobby.
    /// </summary>
    public class PlayerListPanelController : MonoBehaviour
    {
        [SerializeField] private List<PlayerListEntry> _playerListEntries = new();

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
                LobbyEvents.OnPlayerJoined += OnPlayerJoined;
                LobbyEvents.OnPlayerLeft += OnPlayerLeft;
                LobbyEvents.OnPlayerKicked += OnPlayerLeft;
                LobbyEvents.OnPlayerConnecting += OnPlayerConnecting;

                ResetPlayerList();
                _playerListEntries.Find(entry => entry.PlayerId == playerId).SetConnected();
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
                LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
                LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
                LobbyEvents.OnPlayerKicked -= OnPlayerLeft;
                LobbyEvents.OnPlayerConnecting -= OnPlayerConnecting;

                _playerListEntries.Find(entry => entry.PlayerId == playerId).SetDisconnected();
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        private void ResetPlayerList()
        {
            List<Player> players = LobbyManager.Instance.Lobby.Players;
            for (int i = 0; i < _playerListEntries.Count; i++)
            {
                _playerListEntries[i].gameObject.SetActive(true);
                if (i < players.Count) _playerListEntries[i].SetPlayer(players[i].Id);
                else if (i < LobbyManager.Instance.Lobby.MaxPlayers) _playerListEntries[i].SetEmpty();
                else _playerListEntries[i].gameObject.SetActive(false);
            }
        }

        private void OnLobbyHostMigrated(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId) ResetPlayerList();
        }

        private void OnPlayerJoined(string playerId)
        {
            _playerListEntries.Find(e => e.PlayerId == null).SetPlayer(playerId);
        }

        private void OnPlayerLeft(string playerId)
        {
            _playerListEntries.Find(e => e.PlayerId == playerId).SetEmpty();
            ResetPlayerList();
        }

        public void SetPlayerTeam(string playerId, Team team)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetTeam(team);
        }

        public void SetPlayerStatus(string playerId, PlayerStatus status)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetStatus(status);
        }

        private void OnPlayerConnecting(string playerId)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetConnecting();
        }
    }
}