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
            PlayerListEntry newEntry = Instantiate(_playerListEntries[LobbyManager.Instance.Lobby.Players.Count], transform);
            newEntry.SetPlayer(player);
            _playerListEntries.Add(newEntry);
        }

        private void OnPlayerLeft(Player player)
        {
            PlayerListEntry entryToRemove = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToRemove.SetEmpty();
            _playerListEntries.Remove(entryToRemove);
        }

        private void OnPlayerDataChanged(Player player, string key, string value)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetButtons();
        }

        private void OnPlayerTeamChanged(Player player, Team team)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetTeam(team);
        }

        private void OnPlayerStatusChanged(Player player, PlayerStatus status)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetStatus(status);
        }

        private void OnPlayerConnecting(Player player)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetConnecting();
        }

        private void OnPlayerConnect(Player player)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetConnected();
        }

        private void OnPlayerDisconnect(Player player)
        {
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetDisconnected();
        }
    }
}