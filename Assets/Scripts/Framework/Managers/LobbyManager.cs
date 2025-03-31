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
        private Lobby lobby;
        public Lobby Lobby => lobby;

        /// <summary>
        /// Retrieves list of all active lobbies.
        /// </summary>
        /// <param name="filters">The filters to apply to the query.</param>
        /// <param name="maxResults">The maximum number of results to return.</param>
        /// <returns>A list of lobbies matching the query.</returns>
        public async Task<List<Lobby>> GetLobbies(List<QueryFilter> filters, int maxResults)
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Count = maxResults,
                Order = new List<QueryOrder> { new(false, QueryOrder.FieldOptions.Created) },
                Filters = filters,
            };

            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                LobbyEvents.InvokeLobbyQueryResponse(OperationResult.SuccessResult("GetLobbies", $"Found {queryResponse.Results.Count} " + (queryResponse.Results.Count == 1 ? "lobby" : "lobbies")));
                return queryResponse.Results;
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("GetLobbiesError", e.Message));
                return new List<Lobby>();
            }
        }

        /// <summary>
        /// Retrieves a list of all lobbies the player is in.
        /// </summary>
        /// <returns>A list of lobby IDs the player is currently in.</returns>
        public async Task<List<string>> GetJoinedLobbies()
        {
            return await LobbyService.Instance.GetJoinedLobbiesAsync();
        }

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
                Task<Lobby> task = null;
                try
                {
                    task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                }
                catch (LobbyServiceException e)
                {
                    LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("RefreshLobbyError", e.Message));
                    yield break;
                }

                yield return new WaitUntil(() => task.IsCompleted);

                lobby = task.Result;
                LobbyEvents.InvokeLobbyRefreshed();

                yield return new WaitForSecondsRealtime(refreshIntervalSeconds);
            }
        }
        #endregion

        #region Lobby Management
        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or public.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="lobbyData">Serialized lobby data.</param>
        public async void CreateLobby(string lobbyName, bool isPrivate, int maxPlayers, Dictionary<string, DataObject> lobbyData)
        {
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = isPrivate,
                Player = AuthenticationManager.Instance.LocalPlayer,
                Data = lobbyData
            };

            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

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
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("JoinLobbyByIdError", e.Message));
            }
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        public async void RejoinLobby(List<string> joinedLobbyIds)
        {
            try
            {
                foreach (string lobbyId in joinedLobbyIds.GetRange(1, joinedLobbyIds.Count - 1))
                {
                    await Task.Delay(1500);
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

        #region Data Management
        /// <summary>
        /// Updates the player data with the provided dictionary.
        /// </summary>
        /// <param name="playerId">The ID of the player whose data is being updated.</param>
        /// <param name="playerData">The dictionary containing the player data to update.</param>
        /// <param name="allocationId">The allocation ID for the player.</param>
        /// <param name="connectionData">The connection data for the player.</param>
        public async void UpdatePlayerData(string playerId, Dictionary<string, PlayerDataObject> playerData, string allocationId = default, string connectionData = default)
        {
            UpdatePlayerOptions updatePlayerOptions = new()
            {
                ConnectionInfo = connectionData,
                Data = playerData,
                AllocationId = allocationId

            };

            try
            {
                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, updatePlayerOptions);

                LobbyEvents.InvokePlayerDataUpdated(OperationResult.SuccessResult("UpdatePlayerData", $"Player {playerId} data updated"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("UpdatePlayerDataError", e.Message));
            }
        }

        /// <summary>
        /// Updates the lobby data with the provided dictionary.
        /// </summary>
        /// <param name="lobbyData">The dictionary containing the lobby data to update.</param>
        public async void UpdateLobbyData(Dictionary<string, DataObject> lobbyData)
        {
            UpdateLobbyOptions updateLobbyOptions = new()
            {
                Data = lobbyData
            };

            try
            {
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);

                LobbyEvents.InvokeLobbyDataUpdated(OperationResult.SuccessResult("UpdateLobbyData", $"Lobby {lobby.Id} data updated"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("UpdateLobbyDataError", e.Message));
            }
        }
        #endregion

        private void ClearLobby()
        {
            StopAllCoroutines();
            lobby = null;
        }
    }
}