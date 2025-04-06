using System;
using UnityEngine;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
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

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationManager.Instance.LocalPlayer);
        }

        private void OnPlayerConnect(Player player)
        {
            LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
            LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            LobbyEvents.OnPlayerKicked += OnPlayerLeft;
            LobbyEvents.OnPlayerTeamChanged += OnPlayerTeamChanged;
            LobbyEvents.OnPlayerStatusChanged += OnPlayerStatusChanged;
            LobbyEvents.OnPlayerConnecting += OnPlayerConnecting;

            ResetPlayerList();
            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetConnected();
        }

        private void OnPlayerDisconnect(Player player)
        {
            LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
            LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            LobbyEvents.OnPlayerKicked -= OnPlayerLeft;
            LobbyEvents.OnPlayerTeamChanged -= OnPlayerTeamChanged;
            LobbyEvents.OnPlayerStatusChanged -= OnPlayerStatusChanged;
            LobbyEvents.OnPlayerConnecting -= OnPlayerConnecting;

            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetDisconnected();
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        private void ResetPlayerList()
        {
            if (LobbyManager.Instance.Lobby == null) return;
            List<Player> players = LobbyManager.Instance.Lobby.Players;
            for (int i = 0; i < _playerListEntries.Count; i++)
            {
                _playerListEntries[i].gameObject.SetActive(true);
                if (i < players.Count) _playerListEntries[i].SetPlayer(players[i]);
                else if (i < LobbyManager.Instance.Lobby.MaxPlayers) _playerListEntries[i].SetEmpty();
                else _playerListEntries[i].gameObject.SetActive(false);
            }
        }

        private void OnLobbyHostMigrated(Player player)
        {
            if (player.Id == AuthenticationManager.Instance.LocalPlayer.Id) ResetPlayerList();
        }

        private void OnPlayerJoined(Player player)
        {
            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetPlayer(player);
        }

        private void OnPlayerLeft(Player player)
        {
            ResetPlayerList();
        }

        private void OnPlayerTeamChanged(Player player)
        {
            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetTeam(Enum.Parse<Team>(player.Data["Team"].Value));
        }

        private void OnPlayerStatusChanged(Player player)
        {
            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetStatus(Enum.Parse<PlayerStatus>(player.Data["Status"].Value));
        }

        private void OnPlayerConnecting(Player player)
        {
            _playerListEntries.Find(entry => entry.PlayerId == player.Id).SetConnecting();
        }
    }
}