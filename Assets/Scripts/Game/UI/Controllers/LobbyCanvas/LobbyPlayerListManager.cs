using UnityEngine;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles the player list in the lobby.
    /// </summary>
    public class LobbyPlayerListManager : MonoBehaviour
    {
        // [SerializeField] private List<PlayerListEntry> _playerListEntries = new();

        // private void OnEnable()
        // {
        //     LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        // }

        // private void OnDisable()
        // {
        //     LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        // }

        // private void OnLobbyUpdated()
        // {
        //     List<LobbyPlayerData> playersData = GameLobbyManager.Instance.GetPlayers();

        //     for (int i = 0; i < _playerListEntries.Count; i++)
        //     {
        //         _playerListEntries[i].gameObject.SetActive(true);
        //         if (i < playersData.Count)
        //         {
        //             bool isConnected = playersData[i].IsConnected;
        //             _playerListEntries[i].SetData(playersData[i], isConnected ? LobbyPlayerSpotState.Active : LobbyPlayerSpotState.Disconnected);
        //         }
        //         else if (i < LobbyManager.Instance.MaxPlayers)
        //             _playerListEntries[i].SetState(LobbyPlayerSpotState.Empty);
        //         else
        //             _playerListEntries[i].gameObject.SetActive(false);
        //     }
        // }
    }
}