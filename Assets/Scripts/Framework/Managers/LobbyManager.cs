using System;
using UnityEngine;
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
        /// Current lobby name
        /// </summary>
        public string LobbyName => IsInLobby ? lobby.Name : string.Empty;

        /// <summary>
        /// Current lobby join code
        /// </summary>
        public string LobbyCode => IsInLobby ? lobby.LobbyCode : string.Empty;

        /// <summary>
        /// Whether player is currently in a lobby
        /// </summary>
        public bool IsInLobby => lobby != null;

        /// <summary>
        /// Current lobby ID
        /// </summary>
        public string LobbyId => IsInLobby ? lobby.Id : string.Empty;

        /// <summary>
        /// Whether the current player is the host of the lobby
        /// </summary>
        public bool IsLobbyHost => lobby.HostId == AuthenticationService.Instance.PlayerId;

        /// <summary>
        /// Current number of players in the lobby
        /// </summary>
        public int PlayerCount => lobby.Players.Count;

        /// <summary>
        /// Maximum allowed players in the lobby
        /// </summary>
        public int MaxPlayers => lobby.MaxPlayers;

        /// <summary>
        /// Current game mode
        /// </summary>
        public string GameMode => lobby.Data["GameMode"].Value;

        /// <summary>
        /// List of players in the lobby
        /// </summary>
        public IReadOnlyList<Player> Players => IsInLobby ? lobby.Players : new List<Player>();

        #endregion

        #region Private Fields

        private Lobby lobby;
        private float heartbeatTimer;
        private float refreshTimer;
        private const float HEARTBEAT_INTERVAL = 15f;
        private const float REFRESH_INTERVAL = 1.1f;

        #endregion

        #region Lifecycle Methods

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyRefresh();
        }

        private void OnDestroy()
        {
            CleanupLobby();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (lobby == null || !IsInLobby || !IsLobbyHost) return;

            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = HEARTBEAT_INTERVAL;

                try
                {
                    string lobbyId = lobby.Id;
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                }
                catch (LobbyServiceException e)
                {
                    if (e.ErrorCode == 10004)
                    {
                        CleanupLobby();
                        LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                    }
                }
            }
        }

        private async void HandleLobbyRefresh()
        {
            if (lobby == null || !IsInLobby) return;

            refreshTimer -= Time.deltaTime;
            if (refreshTimer < 0f)
            {
                refreshTimer = REFRESH_INTERVAL;

                try
                {
                    string lobbyId = lobby.Id;
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
                    LobbyEvents.InvokeLobbyUpdated(lobby);
                }
                catch (LobbyServiceException e)
                {
                    if (e.ErrorCode == 10004)
                    {
                        CleanupLobby();
                        LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up lobby resources and stops heartbeat/refresh cycles
        /// </summary>
        public void CleanupLobby()
        {
            lobby = null;
            heartbeatTimer = 0;
            refreshTimer = 0;
        }

        #endregion

        #region Lobby Creation and Joining

        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers)
        {
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Standard") },
                    { "Map" , new DataObject(DataObject.VisibilityOptions.Public, "Default")}
                }
            };

            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

                heartbeatTimer = HEARTBEAT_INTERVAL;
                refreshTimer = REFRESH_INTERVAL;

                LobbyEvents.InvokeLobbyCreated(lobby);
                return OperationResult.SuccessResult("LobbyCreated", $"Lobby {lobby.Name} created with ID: {lobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new() { Player = GetPlayer() };
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

                refreshTimer = REFRESH_INTERVAL;

                LobbyEvents.InvokeLobbyJoined(lobby);
                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by code: {lobbyCode}");
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            try
            {
                JoinLobbyByIdOptions joinLobbyByIdOptions = new() { Player = GetPlayer() };
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

                refreshTimer = REFRESH_INTERVAL;

                LobbyEvents.InvokeLobbyJoined(lobby);
                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by Id: {lobbyId}");
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion

        #region Lobby Management

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        public async Task<OperationResult> LeaveLobby()
        {
            if (!IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");

            try
            {
                string lobbyId = lobby.Id;
                CleanupLobby();

                await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
                LobbyEvents.InvokeLobbyLeft();

                return OperationResult.SuccessResult("LeaveLobby", "Left lobby successfully");
            }
            catch (LobbyServiceException e)
            {
                CleanupLobby();
                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        public async Task<OperationResult> KickPlayer(string playerId)
        {
            if (!IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id,
                    lobby?.Players.Find(player => player.Id == playerId)?.Id ?? playerId);

                LobbyEvents.InvokePlayerKicked(playerId);
                return OperationResult.SuccessResult("KickPlayer", $"Player {playerId} kicked from lobby");
            }
            catch (LobbyServiceException e)
            {
                if (playerId == AuthenticationService.Instance.PlayerId)
                    CleanupLobby();

                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of active lobbies
        /// </summary>
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
                LobbyEvents.InvokeLobbyError(e.ErrorCode.ToString(), e.Message);
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion

        #region Lobby Data Updates

        /// <summary>
        /// Updates the lobby's game mode
        /// </summary>
        public async Task<OperationResult> UpdateGameMode(string gameMode)
        {
            if (!IsInLobby)
            {
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");
            }

            try
            {
                lobby = await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                    }
                });

                // LobbyEvents.InvokeGameModeChanged(gameMode);
                return OperationResult.SuccessResult("GameModeUpdated", $"Game mode updated to {gameMode}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update lobby game mode: {e.Message}");
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Updates the player's name in the lobby
        /// </summary>
        public async Task<OperationResult> UpdatePlayerName(string newPlayerName)
        {
            if (!IsInLobby)
            {
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");
            }

            try
            {
                lobby = await Lobbies.Instance.UpdatePlayerAsync(lobby.Id,
                    AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newPlayerName) }
                        }
                    });

                PlayerPrefs.SetString("PlayerName", newPlayerName);
                PlayerPrefs.Save();

                // LobbyEvents.InvokePlayerNameUpdated(newPlayerName);
                return OperationResult.SuccessResult("PlayerNameUpdated", $"Player name updated to {newPlayerName}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update player name: {e.Message}");
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion

        #region Private Helpers

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetString("PlayerName")) }
                }
            };
        }

        #endregion
    }
}