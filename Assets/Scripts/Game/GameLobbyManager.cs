using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;


namespace Assets.Scripts.Game
{
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private readonly List<LobbyPlayerData> lobbyPlayerData = new();
        private LobbyPlayerData localLobbyPlayerData;
        private LobbyData lobbyData;

        public bool IsHost => localLobbyPlayerData.PlayerId == LobbyManager.Instance.GetHostId();

        private void OnEnable()
        {
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        public string GetLobbyCode()
        {
            return LobbyManager.Instance.GetLobbyCode();
        }

        public async Task<bool> CreateLobby()
        {
            localLobbyPlayerData = new LobbyPlayerData();
            string playerName = PlayerPrefs.GetString("username");
            if (IsHost)
            {
                playerName += " (Host)";
            }
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, playerName);
            lobbyData = new LobbyData();
            lobbyData.Initialize(mapIndex: 0);

            bool succeeded = await LobbyManager.Instance.CreateLobby(maxPlayers: 4, isPrivate: true, data: localLobbyPlayerData.Serialize(), lobbyData.Serialize());
            return succeeded;
        }

        public async Task<bool> JoinLobby(string code)
        {
            localLobbyPlayerData = new LobbyPlayerData();
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, playerName: PlayerPrefs.GetString("username"));

            bool succeeded = await LobbyManager.Instance.JoinLobby(code, localLobbyPlayerData.Serialize());
            return succeeded;
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayerData();
            lobbyPlayerData.Clear();

            foreach (Dictionary<string, PlayerDataObject> data in playerData)
            {
                LobbyPlayerData lobbyPlayerData = new();
                lobbyPlayerData.Initialize(data);

                if (lobbyPlayerData.PlayerId == AuthenticationService.Instance.PlayerId)
                {
                    localLobbyPlayerData = lobbyPlayerData;
                }

                this.lobbyPlayerData.Add(lobbyPlayerData);
            }

            lobbyData = new LobbyData();
            lobbyData.Initialize(lobby.Data);

            Events.LobbyEvents.OnLobbyUpdated?.Invoke();
        }

        public List<LobbyPlayerData> GetLobbyPlayers()
        {
            return lobbyPlayerData;
        }

        public async Task<bool> SetPlayerReady()
        {
            localLobbyPlayerData.IsReady = true;
            bool succeeded = await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());
            return succeeded;
        }

        public int GetMapIndex()
        {
            return lobbyData.MapIndex;
        }

        public async Task<bool> SetSelectedMap(int currentMapIndex)
        {
            lobbyData.MapIndex = currentMapIndex;
            return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        }
    }
}
