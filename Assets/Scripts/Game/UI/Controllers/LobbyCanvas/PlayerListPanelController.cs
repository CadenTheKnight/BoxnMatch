using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
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
            LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            LobbyEvents.OnPlayerDataUpdated += OnPlayerDataUpdated;
            LobbyEvents.OnPlayerLeft += OnPlayerLeft;
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            LobbyEvents.OnPlayerDataUpdated -= OnPlayerDataUpdated;
            LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
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

        private void OnPlayerDataUpdated(OperationResult result)
        {
            Debug.Log($"Could update player entry here");
        }

        private void OnPlayerLeft(Player player)
        {
            PlayerListEntry entryToRemove = _playerListEntries.Find(entry => entry.Player.Id == player.Id);
            if (entryToRemove != null)
            {
                entryToRemove.SetEmpty();
                _playerListEntries.Remove(entryToRemove);
            }
        }
    }
}