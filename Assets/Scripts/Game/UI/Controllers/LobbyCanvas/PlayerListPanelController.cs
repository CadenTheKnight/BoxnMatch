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
            Debug.Log($"Player {player.Id} getting a list entry.");
            PlayerListEntry emptyEntry = _playerListEntries.Find(entry => entry.Player == null);
            emptyEntry.SetPlayer(player);
            _playerListEntries.Add(emptyEntry);
        }

        private void OnPlayerLeft(Player player)
        {
            Debug.Log($"Player {player.Id} leaving the list.");
            PlayerListEntry entryToRemove = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToRemove.SetEmpty();
            _playerListEntries.Remove(entryToRemove);
        }

        private void OnPlayerDataChanged(string playerId)
        {
            Debug.Log($"Player {playerId} list data changed.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == playerId);
            entryToUpdate.SetButtons();
        }

        private void OnPlayerTeamChanged(Player player, Team team)
        {
            Debug.Log($"Player {player.Id} list team changed to {team}.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetTeam(team);
        }

        private void OnPlayerStatusChanged(Player player, PlayerStatus status)
        {
            Debug.Log($"Player {player.Id} list status changed to {status}.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetStatus(status);
        }

        private void OnPlayerConnecting(Player player)
        {
            Debug.Log($"Player {player.Id} is connecting.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetConnecting();
        }

        private void OnPlayerConnect(Player player)
        {
            Debug.Log($"Player {player.Id} connected.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetConnected();
        }

        private void OnPlayerDisconnect(Player player)
        {
            Debug.Log($"Player {player.Id} disconnected.");
            PlayerListEntry entryToUpdate = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            entryToUpdate.SetDisconnected();
        }
    }
}