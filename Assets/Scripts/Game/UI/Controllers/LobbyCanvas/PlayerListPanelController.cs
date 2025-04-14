using UnityEngine;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
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

            if (GameLobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
                LobbyEvents.OnPlayerJoined += OnPlayerJoined;
                LobbyEvents.OnPlayerLeft += OnPlayerLeft;
                LobbyEvents.OnPlayerConnecting += OnPlayerConnecting;

                GameLobbyEvents.OnPlayerStatusChanged += SetPlayerStatus;
                GameLobbyEvents.OnPlayerTeamChanged += SetPlayerTeam;

                ResetPlayerList();
                // _playerListEntries.Find(entry => entry.PlayerId == playerId).SetConnected();
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
                LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
                LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
                LobbyEvents.OnPlayerConnecting -= OnPlayerConnecting;

                GameLobbyEvents.OnPlayerStatusChanged -= SetPlayerStatus;
                GameLobbyEvents.OnPlayerTeamChanged -= SetPlayerTeam;

                // _playerListEntries.Find(entry => entry.PlayerId == playerId).SetDisconnected();
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;
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

        private void OnLobbyHostMigrated(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId) ResetPlayerList();
        }

        private void OnPlayerJoined(string playerId)
        {
            _playerListEntries.Find(entry => entry.PlayerId == null).SetPlayer(playerId);
        }

        private void OnPlayerLeft(int playerIndex)
        {
            _playerListEntries.Find(entry => entry.PlayerId == GameLobbyManager.Instance.Lobby.Players[playerIndex].Id).SetEmpty();
            ResetPlayerList();
        }

        private void SetPlayerTeam(string playerId)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetTeam((Team)int.Parse(GameLobbyManager.Instance.GetPlayerById(playerId).Data["Team"].Value));
        }

        private void SetPlayerStatus(string playerId)
        {
            _playerListEntries.Find(entry => entry.PlayerId == playerId).SetStatus((PlayerStatus)int.Parse(GameLobbyManager.Instance.GetPlayerById(playerId).Data["Status"].Value));
        }

        private void OnPlayerConnecting(string playerId)
        {
            // _playerListEntries.Find(entry => entry.PlayerId == playerId).SetConnecting();
        }
    }
}