using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Manager;

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
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            // LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft += OnPlayerLeft;
        }

        private void OnDisable()
        {
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            // LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            // LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
        }

        public string GetLobbyName()
        {
            return LobbyManager.Instance.GetLobbyName();
        }

        public string GetLobbyCode()
        {
            return LobbyManager.Instance.GetLobbyCode();
        }

        public int GetMapIndex()
        {
            return lobbyData?.MapIndex ?? 0;
        }

        public List<LobbyPlayerData> GetLobbyPlayers()
        {
            return lobbyPlayerData;
        }

        public async Task<OperationResult> SetPlayerReady()
        {
            localLobbyPlayerData.IsReady = true;
            var result = await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());

            if (result.Success)
            {
                Debug.Log($"Player: {localLobbyPlayerData.PlayerId} is ready");
                Events.LobbyEvents.InvokePlayerReady(localLobbyPlayerData.PlayerId);
                return new OperationResult(true);
            }
            else
            {
                Debug.LogError($"Failed to set player ready: {result.ErrorCode} - {result.ErrorMessage}");
                return new OperationResult(false, result.ErrorCode, result.ErrorMessage);
            }
        }

        public async Task<OperationResult> SetPlayerUnready()
        {
            localLobbyPlayerData.IsReady = false;
            var result = await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());

            if (result.Success)
            {
                Debug.Log($"Player: {localLobbyPlayerData.PlayerId} is not ready");
                Events.LobbyEvents.InvokePlayerNotReady(localLobbyPlayerData.PlayerId);
                return new OperationResult(true);
            }
            else
            {
                Debug.LogError($"Failed to set player unready: {result.ErrorCode} - {result.ErrorMessage}");
                return new OperationResult(false, result.ErrorCode, result.ErrorMessage);
            }
        }

        public async Task<OperationResult> SetSelectedMap(int currentMapIndex)
        {
            lobbyData.MapIndex = currentMapIndex;
            return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        }

        public async Task<OperationResult> CreateLobby(string lobbyName)
        {
            localLobbyPlayerData = new LobbyPlayerData();
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, PlayerPrefs.GetString("username"));
            lobbyData = new LobbyData();
            lobbyData.Initialize(mapIndex: 0);

            return await LobbyManager.Instance.CreateLobby(lobbyName: lobbyName, maxPlayers: 4, isPrivate: true, data: localLobbyPlayerData.Serialize(), lobbyData.Serialize());
        }

        public async Task<OperationResult> JoinLobby(string code)
        {
            localLobbyPlayerData = new LobbyPlayerData();
            localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, playerName: PlayerPrefs.GetString("username"));

            return await LobbyManager.Instance.JoinLobby(code, localLobbyPlayerData.Serialize());
        }

        public async Task<OperationResult> LeaveLobby(string playerId)
        {
            return await LobbyManager.Instance.LeaveLobby(playerId);
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

            UpdateNumberOfPlayersReady();

            Events.LobbyEvents.InvokeLobbyUpdated();
        }

        private void UpdateNumberOfPlayersReady()
        {
            int numberOfPlayersReady = 0;
            foreach (var player in lobbyPlayerData)
            {
                if (player.IsReady)
                {
                    numberOfPlayersReady++;
                }
            }

            if (numberOfPlayersReady == lobbyPlayerData.Count && numberOfPlayersReady > 0)
            {
                Events.LobbyEvents.InvokeLobbyReady();
            }
            else
            {
                Events.LobbyEvents.InvokeLobbyNotReady();
            }
        }

        // private void OnPlayerJoined(string playerId)
        // {
        //     // notify of join in lobby chat
        // }

        // private void OnPlayerLeft(string playerId)
        // {
        //     // notify of leave in lobby chat
        // }
    }
}
