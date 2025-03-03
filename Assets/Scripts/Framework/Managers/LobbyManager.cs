using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
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
        #region Public Properties

        /// <summary>
        /// Current lobby object.
        /// </summary>
        public Lobby lobby;

        /// <summary>
        /// Current lobby name.
        /// </summary>
        public string LobbyName => IsInLobby ? lobby.Name : string.Empty;

        /// <summary>
        /// Current lobby join code.
        /// </summary>
        public string LobbyCode => IsInLobby ? lobby.LobbyCode : string.Empty;

        /// <summary>
        /// Whether player is currently in a lobby.
        /// </summary>
        public bool IsInLobby => lobby != null;

        /// <summary>
        /// Current lobby ID.
        /// </summary>
        public string LobbyId => IsInLobby ? lobby.Id : string.Empty;

        /// <summary>
        /// Whether the current player is the host of the lobby.
        /// </summary>
        public bool IsLobbyHost => lobby.HostId == AuthenticationService.Instance.PlayerId;

        /// <summary>
        /// Current number of players in the lobby.
        /// </summary>
        public int PlayerCount => lobby.Players.Count;

        /// <summary>
        /// Maximum allowed players in the lobby.
        /// </summary>
        public int MaxPlayers => lobby.MaxPlayers;

        /// <summary>
        /// Current game mode.
        /// </summary>
        public string GameMode => lobby.Data["GameMode"].Value;

        /// <summary>
        /// List of players in the lobby.
        /// </summary>
        public IReadOnlyList<Player> Players => IsInLobby ? lobby.Players : new List<Player>();

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Handles the lobby heartbeat, sending regular updates to the lobby service.
        /// </summary>
        private IEnumerator HeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true)
            {
                var task = LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.IsFaulted)
                    Debug.LogError($"Heartbeat failed: {task.Exception}");
                yield return delay;
            }
        }

        /// <summary>
        /// Handles the lobby refresh, updating the lobby data at regular intervals.
        /// </summary>
        private IEnumerator RefreshCoroutine(string lobbyId, float refreshIntervalSeconds)
        {
            var delay = new WaitForSecondsRealtime(refreshIntervalSeconds);
            while (true)
            {
                var task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.IsFaulted)
                    Debug.LogError($"Refresh failed: {task.Exception}");
                else
                {
                    lobby = task.Result;
                    LobbyEvents.InvokeLobbyUpdated(lobby);
                }
                yield return delay;
            }
        }

        #endregion

        #region Lobby Creation and Joining

        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="createLobbyOptions">Additional options for creating the lobby.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions createLobbyOptions)
        {
            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

                LobbyEvents.InvokeLobbyCreated(lobby);
                StartCoroutine(HeartbeatCoroutine(lobby.Id, 15f));
                StartCoroutine(RefreshCoroutine(lobby.Id, 2f));

                return OperationResult.SuccessResult("LobbyCreated", $"Lobby: {lobby.Name} created");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        /// <param name="joinLobbyByCodeOptions">Additional options for joining the lobby.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode, JoinLobbyByCodeOptions joinLobbyByCodeOptions)
        {
            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                StartCoroutine(RefreshCoroutine(lobby.Id, 2f));

                LobbyEvents.InvokeLobbyJoined(lobby);

                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by code: {lobbyCode}");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        /// <param name="joinLobbyByIdOptions">Additional options for joining the lobby.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId, JoinLobbyByIdOptions joinLobbyByIdOptions)
        {
            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
                StartCoroutine(RefreshCoroutine(lobby.Id, 2f));

                LobbyEvents.InvokeLobbyJoined(lobby);

                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by Id: {lobbyId}");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion

        #region Lobby Management

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
                StopAllCoroutines();

                LobbyEvents.InvokeLobbyLeft();

                return OperationResult.SuccessResult("LeaveLobby", "Left lobby successfully");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        /// <param name="playerId">The ID of the player being kicked.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> KickPlayer(string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id,
                    lobby?.Players.Find(player => player.Id == playerId)?.Id ?? playerId);

                if (AuthenticationService.Instance.PlayerId == playerId)
                    StopAllCoroutines();

                LobbyEvents.InvokePlayerKicked(playerId);

                return OperationResult.SuccessResult("KickPlayer", $"Player {playerId} kicked from lobby");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of active lobbies.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> GetLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new()
                {
                    Order = new List<QueryOrder> { new(false, QueryOrder.FieldOptions.Created) }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                LobbyEvents.InvokeLobbyListChanged(queryResponse.Results);

                return OperationResult.SuccessResult("GetLobbies", $"Retrieved {queryResponse.Results.Count} lobbies");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion
    }
}