using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Handles low-level interaction with Unity's Lobby service.
    /// </summary>
    public class LobbyManager : Singleton<LobbyManager>
    {
        #region Private Fields
        private Lobby lobby;
        private List<string> joinedLobbyIds;
        #endregion

        #region Public Properties
        public Lobby Lobby => lobby;
        public List<string> JoinedLobbyIds => joinedLobbyIds;
        public string LobbyName => lobby.Name;
        public bool IsPrivate => lobby.IsPrivate;
        public string LobbyCode => IsInLobby ? lobby.LobbyCode : string.Empty;
        public bool IsInLobby => lobby != null;
        public string LobbyId => IsInLobby ? lobby.Id : string.Empty;
        public bool IsHostId(string playerId) => lobby.HostId == playerId;
        public bool IsLobbyHost => lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id;
        public int PlayerCount => lobby.Players.Count;
        public int MaxPlayers => int.Parse(lobby.Data["MaxPlayers"].Value);
        public int RoundCount => int.Parse(lobby.Data["RoundCount"].Value);
        public string GameMode => lobby.Data["GameMode"].Value;
        public bool IsGameInProgress => lobby.Data["GameInProgress"].Value == "true";
        public string RelayJoinCode => lobby.Data["JoinCode"].Value ?? default;
        public IReadOnlyList<Player> Players => IsInLobby ? lobby.Players : new List<Player>();
        #endregion

        #region Lifecycle Methods
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshCoroutine;

        /// <summary>
        /// Handles the lobby heartbeat, sending regular updates to the lobby service.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to send heartbeats to.</param>
        /// <param name="waitTimeSeconds">The time to wait between heartbeats.</param>
        /// <returns>Coroutine for the heartbeat process.</returns>
        private IEnumerator HeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        /// <summary>
        /// Handles the lobby refresh, updating the lobby data at regular intervals.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to refresh.</param>
        /// <param name="refreshIntervalSeconds">The time to wait between refreshes.</param>
        /// <returns>Coroutine for the refresh process.</returns>
        private IEnumerator RefreshCoroutine(string lobbyId, float refreshIntervalSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);

                lobby = task.Result;
                // LobbyEvents.InvokeLobbyUpdated(lobby);

                yield return new WaitForSecondsRealtime(refreshIntervalSeconds);
            }
        }
        #endregion

        #region Lobby Management
        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyData">Dictionary of lobby data.</param>
        public async void CreateLobby(Dictionary<string, string> lobbyData)
        {

            Dictionary<string, DataObject> serializedLobbyData = SerializeLobbyData(lobbyData);

            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = serializedLobbyData["IsPrivate"].Value == "true",
                Player = AuthenticationManager.Instance.LocalPlayer,
                Data = serializedLobbyData
            };

            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(serializedLobbyData["LobbyName"].Value, int.Parse(serializedLobbyData["MaxPlayers"].Value), createLobbyOptions);

                _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(lobby.Id, 6f));
                _refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                LobbyEvents.InvokeLobbyCreated(OperationResult.SuccessResult("CreateLobby", $"Created lobby: {lobby.Name}"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("CreateLobbyError", e.Message));
            }
        }

        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        public async void JoinLobbyByCode(string lobbyCode)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new()
            {
                Player = AuthenticationManager.Instance.LocalPlayer
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

                _refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyByCode", $"Joined lobby: {lobby.Name} by code: {lobbyCode}"));
                LobbyEvents.InvokePlayerJoined(OperationResult.SuccessResult("PlayerJoinedByCode", $"{AuthenticationManager.Instance.LocalPlayer.Profile.Name} joined the lobby"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("JoinLobbyByCodeError", e.Message));
            }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        public async void JoinLobbyById(string lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new()
            {
                Player = AuthenticationManager.Instance.LocalPlayer
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

                _refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyById", $"Joined lobby: {lobby.Name} by ID: {lobbyId}"));
                LobbyEvents.InvokePlayerJoined(OperationResult.SuccessResult("PlayerJoinedById", $"{AuthenticationManager.Instance.LocalPlayer.Profile.Name} joined the lobby"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("JoinLobbyByIdError", e.Message));
            }
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        public async void RejoinLobby()
        {
            try
            {
                foreach (string lobbyId in joinedLobbyIds.GetRange(1, joinedLobbyIds.Count - 1))
                {
                    await Task.Delay(1500);
                    Debug.Log($"Removing player from lobby: {lobbyId}");
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationManager.Instance.LocalPlayer.Id);
                }

                lobby = await LobbyService.Instance.ReconnectToLobbyAsync(joinedLobbyIds[0]);

                _refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                AuthenticationEvents.InvokeLobbyRejoined(OperationResult.SuccessResult("RejoinLobby", $"Rejoined lobby: {lobby.Name}"));
            }
            catch (System.Exception e)
            {
                AuthenticationEvents.InvokeLobbyRejoinError(OperationResult.ErrorResult("RejoinLobbyError", e.Message));
            }
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        public async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationManager.Instance.LocalPlayer.Id);

                LobbyEvents.InvokeLobbyLeft(OperationResult.SuccessResult("LeaveLobby", $"Left lobby: {lobby.Name}"));
                ClearLobby();
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("LeaveLobbyError", e.Message));
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        /// <param name="playerId">The ID of the player being kicked.</param>
        public async void KickPlayer(string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, playerId);

                if (playerId == AuthenticationManager.Instance.LocalPlayer.Id)
                {
                    LobbyEvents.InvokeLobbyKicked(OperationResult.SuccessResult("KickPlayer", $"Kicked from lobby: {lobby.Name}"));
                    ClearLobby();
                }
                else
                    LobbyEvents.InvokePlayerKicked(OperationResult.SuccessResult("KickPlayer", $"Kicked {playerId} from the lobby"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("KickPlayerError", e.Message));
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a list of active lobbies.
        /// </summary>
        public async void GetLobbies()
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Order = new List<QueryOrder> { new(false, QueryOrder.FieldOptions.Created) }
            };

            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                LobbyEvents.InvokeLobbyQueryResponse(OperationResult.SuccessResult("GetLobbies", $"Found {queryResponse.Results.Count} " + (queryResponse.Results.Count == 1 ? "lobby" : "lobbies")), queryResponse.Results);
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("GetLobbiesError", e.Message));
            }
        }

        public async Task<bool> HasActiveLobbies()
        {
            joinedLobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();

            return joinedLobbyIds.Count > 0;
        }

        #region Data Management
        public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
        {
            List<Dictionary<string, PlayerDataObject>> playersData = new();

            foreach (Player player in lobby.Players)
                playersData.Add(player.Data);

            return playersData;
        }

        public async Task<OperationResult> UpdatePlayerData(string playerId, Dictionary<string, string> playerData, string allocationId = default, string connectionData = default)
        {
            UpdatePlayerOptions updatePlayerOptions = new()
            {
                Data = SerializePlayerData(playerData),
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, updatePlayerOptions);

                // LobbyEvents.InvokeLobbyUpdated(lobby);

                return OperationResult.SuccessResult("UpdatePlayerData", $"Player {playerId} data updated");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        public async Task<OperationResult> UpdateAllPlayerData(List<Dictionary<string, string>> playersData)
        {
            foreach (Dictionary<string, string> playerData in playersData)
            {
                UpdatePlayerOptions updatePlayerOptions = new()
                {
                    Data = SerializePlayerData(playerData)
                };

                try
                {
                    lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationManager.Instance.LocalPlayer.Id, updatePlayerOptions);

                    // LobbyEvents.InvokeLobbyUpdated(lobby);
                }
                catch (LobbyServiceException e)
                {
                    return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
                }
            }

            return OperationResult.SuccessResult("UpdateAllPlayerData", "All player data updated");
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string, string> lobbyData)
        {
            UpdateLobbyOptions updateLobbyOptions = new()
            {
                Data = SerializeLobbyData(lobbyData)
            };

            try
            {
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);

                LobbyEvents.InvokeLobbyDataUpdated(OperationResult.SuccessResult("UpdateLobbyData", $"Lobby data updated"));
                return true;
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("UpdateLobbyDataError", e.Message));
                return false;
            }
        }

        /// <summary>
        /// Serializes player data into a format that can be sent to the Unity Lobby service.
        /// </summary>
        /// <param name="playerData">The player data to serialize.</param>
        /// <returns>A dictionary of data objects.</returns>
        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> playerData)
        {
            Dictionary<string, PlayerDataObject> serializedPlayerData = new();

            foreach (KeyValuePair<string, string> data in playerData)
                serializedPlayerData.Add(data.Key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, data.Value));

            return serializedPlayerData;
        }

        /// <summary>
        /// Serializes lobby data into a format that can be sent to the Unity Lobby service.
        /// </summary>
        /// <param name="lobbyData">The lobby data to serialize.</param>
        /// <returns>A dictionary of data objects.</returns>
        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> lobbyData)
        {
            Dictionary<string, DataObject> serializedLobbyData = new();

            foreach (KeyValuePair<string, string> data in lobbyData)
                serializedLobbyData.Add(data.Key, new DataObject(DataObject.VisibilityOptions.Public, data.Value));

            return serializedLobbyData;
        }

        #endregion

        private void ClearLobby()
        {
            StopAllCoroutines();
            lobby = null;
            joinedLobbyIds = null;
        }
    }
}