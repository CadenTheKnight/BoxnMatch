using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;

namespace Assets.Scripts.Framework.Manager
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        private Lobby lobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;

        public string GetLobbyName()
        {
            return lobby?.Name;
        }

        public string GetLobbyCode()
        {
            return lobby?.LobbyCode;
        }

        public string GetHostId()
        {
            return lobby?.HostId;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new();

            foreach (Player player in lobby.Players)
            {
                data.Add(player.Data);
            }

            return data;
        }

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);

            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);

                Lobby newLobby = task.Result;
                lobby = newLobby;
                LobbyEvents.InvokeLobbyUpdated(lobby);

                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new();
            foreach (KeyValuePair<string, string> kvp in data)
            {
                playerData.Add(kvp.Key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, kvp.Value));
            }

            return playerData;
        }

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new();
            foreach (KeyValuePair<string, string> kvp in data)
            {
                lobbyData.Add(kvp.Key, new DataObject(DataObject.VisibilityOptions.Member, kvp.Value));
            }

            return lobbyData;
        }

        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            CreateLobbyOptions options = new()
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new OperationResult(false, e.ErrorCode, e.Message);
            }

            _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 6f));
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(lobby.Id, 1f));

            Debug.Log($"Lobby {lobby.Name} created with lobby ID: {lobby.Id}");
            return new OperationResult(true);
        }

        public async Task<OperationResult> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new();
            Player player = new(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));
            options.Player = player;

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new OperationResult(false, e.ErrorCode, e.Message);
            }

            if (_refreshLobbyCoroutine != null)
            {
                StopCoroutine(_refreshLobbyCoroutine);
            }
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(lobby.Id, 1f));

            Debug.Log($"Lobby joined with lobby ID: {lobby.Id}");
            LobbyEvents.InvokePlayerJoined(AuthenticationService.Instance.PlayerId);
            return new OperationResult(true);
        }

        public async Task<OperationResult> LeaveLobby(string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new OperationResult(false, e.ErrorCode, e.Message);
            }

            if (lobby.Players.Count == 1 && lobby.Players[0].Id == playerId)
            {
                Debug.Log($"Lobby {lobby.Name} closed with lobby ID: {lobby.Id}");
                if (_heartbeatCoroutine != null)
                {
                    StopCoroutine(_heartbeatCoroutine);
                }
                if (_refreshLobbyCoroutine != null)
                {
                    StopCoroutine(_refreshLobbyCoroutine);
                }
                LobbyEvents.InvokeLobbyClosed();
            }
            else
            {
                Debug.Log($"Player {playerId} left the lobby");
                LobbyEvents.InvokePlayerLeft(playerId);
            }

            return new OperationResult(true);
        }

        public async Task<OperationResult> UpdatePlayerData(string playerId, Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions playerOptions = new()
            {
                Data = playerData
            };

            try
            {
                await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, playerOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new OperationResult(false, e.ErrorCode, e.Message);
            }

            LobbyEvents.InvokeLobbyUpdated(lobby);
            return new OperationResult(true);
        }

        public async Task<OperationResult> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);
            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = lobbyData
            };

            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, lobbyOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return new OperationResult(false, e.ErrorCode, e.Message);
            }

            LobbyEvents.InvokeLobbyUpdated(lobby);
            return new OperationResult(true);
        }
    }
}