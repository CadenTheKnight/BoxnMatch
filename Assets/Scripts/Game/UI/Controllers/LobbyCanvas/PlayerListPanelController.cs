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

        private void OnEnable()
        {
            LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
            LobbyEvents.OnPlayerTeamChanged += OnPlayerTeamChanged;
            LobbyEvents.OnPlayerStatusChanged += OnPlayerStatusChanged;
            LobbyEvents.OnPlayerConnecting += OnPlayerConnecting;
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
            LobbyEvents.OnPlayerTeamChanged -= OnPlayerTeamChanged;
            LobbyEvents.OnPlayerStatusChanged -= OnPlayerStatusChanged;
            LobbyEvents.OnPlayerConnecting -= OnPlayerConnecting;
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;
        }

        private void Start()
        {
            ReorganizePlayerList();
        }

        private void ReorganizePlayerList()
        {
            List<Player> players = LobbyManager.Instance.Lobby.Players;
            for (int i = 0; i < _playerListEntries.Count; i++)
            {
                _playerListEntries[i].gameObject.SetActive(true);
                if (i < players.Count)
                    _playerListEntries[i].SetPlayer(players[i]);
                else if (i < LobbyManager.Instance.Lobby.MaxPlayers)
                    _playerListEntries[i].SetEmpty();
                else
                    _playerListEntries[i].gameObject.SetActive(false);
            }
        }

        private void OnPlayerJoined(Player player)
        {
            _playerListEntries.Find(entry => entry.Player == null).SetPlayer(player);
        }

        private void OnPlayerLeft(Player player)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetEmpty();
            ReorganizePlayerList();
        }

        private void OnPlayerDataChanged(string playerId)
        {
            _playerListEntries.Find(entry => entry.Player.Id == playerId).SetButtons();
        }

        private void OnPlayerTeamChanged(Player player, Team team)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetTeam(team);
        }

        private void OnPlayerStatusChanged(Player player, PlayerStatus status)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetStatus(status);
        }

        private void OnPlayerConnecting(Player player)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetConnecting();
        }

        private void OnPlayerConnect(Player player)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetConnected();
        }

        private void OnPlayerDisconnect(Player player)
        {
            _playerListEntries.Find(entry => entry.Player.Id == player.Id).SetDisconnected();
        }
    }
}