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
            LobbyEvents.OnLobbyRefreshed += OnLobbyRefreshed;
            LobbyEvents.OnPlayerDataUpdated += OnPlayerDataUpdated;
        }

        private void OnDisable()
        {
            LobbyEvents.OnLobbyRefreshed -= OnLobbyRefreshed;
            LobbyEvents.OnPlayerDataUpdated -= OnPlayerDataUpdated;
        }

        private void OnLobbyRefreshed()
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

        private void OnPlayerDataUpdated(OperationResult result)
        {

        }
    }
}