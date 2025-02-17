using UnityEngine;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Framework.Manager;

namespace Assets.Scripts.Game
{
    public class LobbyPlayerSpawner : MonoBehaviour
    {
        [SerializeField] private List<LobbyPlayer> lobbyPlayers;

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
            List<LobbyPlayerData> lobbyPlayersData = GameLobbyManager.Instance.GetLobbyPlayers();
            string hostId = LobbyManager.Instance.GetHostId();

            foreach (var lobbyPlayer in lobbyPlayers)
            {
                lobbyPlayer.ClearData();
            }

            for (int i = 0; i < lobbyPlayersData.Count; i++)
            {
                LobbyPlayerData data = lobbyPlayersData[i];

                LobbyPlayerData modifiedData = new(
                    data.PlayerId,
                    data.PlayerId == hostId ? data.PlayerName + " (Host)" : data.PlayerName,
                    data.IsReady
                );

                lobbyPlayers[i].SetData(modifiedData);
            }
        }
    }
}
