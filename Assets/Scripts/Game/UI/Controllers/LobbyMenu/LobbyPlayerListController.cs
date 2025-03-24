using UnityEngine;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components.ListEntries;


namespace Assets.Scripts.Game.UI.Controllers.LobbyMenu
{
    /// <summary>
    /// Handles the player list in the lobby.
    /// </summary>
    public class LobbyPlayerListController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> emptySpotPrefab;
        [SerializeField] private List<PlayerListEntry> _playerListEntries = new();

        private void OnEnable()
        {
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        private void OnLobbyUpdated()
        {
            List<LobbyPlayerData> playersData = GameLobbyManager.Instance.GetPlayers();

            for (int i = 0; i < 4; i++)
            {
                _playerListEntries[i].gameObject.SetActive(false);
                emptySpotPrefab[i].SetActive(false);
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                LobbyPlayerData playerData = playersData[i];
                _playerListEntries[i].gameObject.SetActive(true);
                _playerListEntries[i].SetData(playerData);
            }

            for (int i = playersData.Count; i < LobbyManager.Instance.MaxPlayers; i++)
                emptySpotPrefab[i].SetActive(true);
        }
    }
}