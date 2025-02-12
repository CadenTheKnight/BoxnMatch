using UnityEngine;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;


namespace Assets.Scripts.Game
{
    public class LobbySpawner : MonoBehaviour
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

            for (int i = 0; i < lobbyPlayersData.Count; i++)
            {
                LobbyPlayerData data = lobbyPlayersData[i];
                lobbyPlayers[i].SetData(data);
            }
        }
    }
}
