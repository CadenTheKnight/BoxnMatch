using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
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
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = false;

        private Lobby lobby;
        public Lobby Lobby => lobby;
        private LobbyEventCallbacks lobbyEventCallbacks;
        private bool isVoluntarilyLeaving = false;
        private readonly List<Player> cachedPlayersList = new();

        /// <summary>
        /// Retrieves list of all active lobbies.
        /// </summary>
        /// <param name="filters">Optional filters to apply to the query.</param>
        /// <returns>A list of lobbies matching the query.</returns>
        public async Task<List<Lobby>> GetLobbies(List<QueryFilter> filters = null)
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                // Count = maxResults,
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
        #endregion

        #region Local Lobby Management
        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or public.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="lobbyData">A dictionary of additional data to associate with the lobby.</param>
        public async Task CreateLobby(string lobbyName, bool isPrivate, int maxPlayers, Dictionary<string, DataObject> lobbyData)
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
                SubscribeToLobbyEvents(lobby.Id);
                await GameLobbyManager.Instance.TogglePlayerReady(AuthenticationManager.Instance.LocalPlayer, setReady: true);
                _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(lobby.Id, 6f));
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
        public async Task JoinLobbyByCode(string lobbyCode)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new()
            {
                Player = AuthenticationManager.Instance.LocalPlayer
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                SubscribeToLobbyEvents(lobby.Id);
                await GameLobbyManager.Instance.TogglePlayerReady(AuthenticationManager.Instance.LocalPlayer, setUnready: true);
                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyByCode", $"Joined lobby: {lobby.Name}"));
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
        public async Task JoinLobbyById(string lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new()
            {
                Player = AuthenticationManager.Instance.LocalPlayer
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
                SubscribeToLobbyEvents(lobby.Id);
                await GameLobbyManager.Instance.TogglePlayerReady(AuthenticationManager.Instance.LocalPlayer, setUnready: true);
                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyById", $"Joined lobby: {lobby.Name}"));
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("JoinLobbyByIdError", e.Message));
            }
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        public async Task RejoinLobby(List<string> joinedLobbyIds)
        {
            try
            {
                foreach (string lobbyId in joinedLobbyIds.GetRange(1, joinedLobbyIds.Count - 1))
                {
                    await Task.Delay(1500);
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationManager.Instance.LocalPlayer.Id);
                }

                lobby = await LobbyService.Instance.ReconnectToLobbyAsync(joinedLobbyIds[0]);
                SubscribeToLobbyEvents(lobby.Id);
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
        public async Task LeaveLobby()
        {
            try
            {
                isVoluntarilyLeaving = true;
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationManager.Instance.LocalPlayer.Id);
            }
            catch (LobbyServiceException e)
            {
                isVoluntarilyLeaving = false;
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("LeaveLobbyError", e.Message));
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        public async Task KickPlayer(Player player)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, player.Id);
                LobbyEvents.InvokePlayerKicked(player);
            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("KickPlayerError", e.Message));
            }
        }
        #endregion

        #region Local Data Management
        /// <summary>
        /// Updates the player data with the provided dictionary.
        /// </summary>
        /// <param name="playerId">The ID of the player whose data is being updated.</param>
        /// <param name="playerData">The dictionary containing the player data to update.</param>
        /// <param name="allocationId">The allocation ID for the player.</param>
        /// <param name="connectionData">The connection data for the player.</param>
        public async Task UpdatePlayerData(string playerId, Dictionary<string, PlayerDataObject> playerData, string allocationId = default, string connectionData = default)
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
                LobbyEvents.InvokePlayerDataChanged(playerId);
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
        public async Task UpdateLobbyData(Dictionary<string, DataObject> lobbyData)
        {
            UpdateLobbyOptions updateLobbyOptions = new()
            {
                Data = lobbyData
            };

            try
            {
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
                LobbyEvents.InvokeLobbyDataChanged(OperationResult.SuccessResult("UpdateLobbyData", $"Updated lobby data for {lobby.Name}"));

            }
            catch (LobbyServiceException e)
            {
                LobbyEvents.InvokeLobbyError(OperationResult.ErrorResult("UpdateLobbyDataError", e.Message));
            }
        }
        #endregion

        #region Lobby Event Callbacks
        /// <summary>
        /// Sets up and subscribes to Unity's built-in lobby events.
        /// </summary>
        private async void SubscribeToLobbyEvents(string lobbyId)
        {
            UnsubscribeFromLobbyEvents();

            lobbyEventCallbacks = new LobbyEventCallbacks();

            lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            lobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            lobbyEventCallbacks.PlayerLeft += OnPlayerLeft;
            lobbyEventCallbacks.DataChanged += OnDataChanged;
            lobbyEventCallbacks.PlayerDataChanged += OnPlayerDataChanged;
            lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

            try
            {
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, lobbyEventCallbacks);
                if (showDebugMessages) Debug.Log($"Successfully subscribed to events for lobby {lobbyId}");

                UpdateCachedPlayers();
            }
            catch (LobbyServiceException ex)
            {
                if (showDebugMessages) Debug.LogError($"Error subscribing to lobby events: {ex.Message}");
            }
        }

        /// <summary>
        /// Unsubscribes from all lobby events.
        /// </summary>
        private void UnsubscribeFromLobbyEvents()
        {
            if (lobbyEventCallbacks == null || lobby == null) return;

            lobbyEventCallbacks.LobbyChanged -= OnLobbyChanged;
            lobbyEventCallbacks.PlayerJoined -= OnPlayerJoined;
            lobbyEventCallbacks.PlayerLeft -= OnPlayerLeft;
            lobbyEventCallbacks.DataChanged -= OnDataChanged;
            lobbyEventCallbacks.PlayerDataChanged -= OnPlayerDataChanged;
            lobbyEventCallbacks.KickedFromLobby -= OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged -= OnLobbyEventConnectionStateChanged;

            if (showDebugMessages) Debug.Log($"Unsubscribed from events for lobby {lobby.Id}");

            lobbyEventCallbacks = null;
            cachedPlayersList.Clear();
        }

        private void UpdateCachedPlayers()
        {
            cachedPlayersList.Clear();
            foreach (Player player in lobby.Players)
                cachedPlayersList.Add(player);
        }

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            if (showDebugMessages) Debug.Log("Lobby changed event received");

            if (lobbyChanges.HostId.Changed)
            {
                if (showDebugMessages) Debug.Log($"Host changed to {lobbyChanges.HostId.Value}");
                if (AuthenticationManager.Instance.LocalPlayer.Id == lobbyChanges.HostId.Value)
                {
                    if (showDebugMessages) Debug.Log("Local player is now host - setting to Ready");
                    LobbyEvents.InvokeNewLobbyHost(AuthenticationManager.Instance.LocalPlayer);

                    if (_heartbeatCoroutine == null)
                    {
                        _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(lobby.Id, 6f));
                        if (showDebugMessages) Debug.Log("Started heartbeat coroutine as new host");
                    }
                }
            }

            if (lobbyChanges.MaxPlayers.Changed)
            {
                if (showDebugMessages) Debug.Log($"Max players changed to {lobbyChanges.MaxPlayers.Value}");
            }

            if (lobbyChanges.IsPrivate.Changed)
            {
                if (showDebugMessages) Debug.Log($"Lobby privacy changed to {(lobbyChanges.IsPrivate.Value ? "Private" : "Public")}");
            }

            if (lobbyChanges.Name.Changed)
            {
                if (showDebugMessages) Debug.Log($"Lobby name changed to {lobbyChanges.Name.Value}");
            }

            LobbyEvents.InvokeLobbyChanged();
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> playersJoined)
        {
            foreach (LobbyPlayerJoined playerJoined in playersJoined)
            {
                cachedPlayersList.Add(playerJoined.Player);
                if (showDebugMessages) Debug.Log($"Player {playerJoined.Player.Id} joined the lobby");
                LobbyEvents.InvokePlayerJoined(playerJoined.Player);
            }
        }

        private void OnPlayerLeft(List<int> playerIndices)
        {
            foreach (int playerIndex in playerIndices)
            {
                if (showDebugMessages) Debug.Log($"Player {cachedPlayersList[playerIndex].Id} left the lobby");
                LobbyEvents.InvokePlayerLeft(cachedPlayersList[playerIndex]);
                cachedPlayersList.Remove(cachedPlayersList[playerIndex]);
            }
        }

        private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dataChanges)
        {
            if (showDebugMessages) Debug.Log($"Lobby data changed: {dataChanges.Count} fields");
            foreach (var kvp in dataChanges)
            {
                if (showDebugMessages) Debug.Log($"- {kvp.Key}: {kvp.Value.Value.Value}");
                if (kvp.Key == "MapIndex")
                    LobbyEvents.InvokeLobbyMapIndexChanged(int.Parse(kvp.Value.Value.Value));
                else if (kvp.Key == "RoundCount")
                    LobbyEvents.InvokeLobbyRoundCountChanged(int.Parse(kvp.Value.Value.Value));
                else if (kvp.Key == "RoundTime")
                    LobbyEvents.InvokeLobbyRoundTimeChanged(int.Parse(kvp.Value.Value.Value));
                else if (kvp.Key == "GameMode")
                    LobbyEvents.InvokeLobbyGameModeChanged((GameMode)int.Parse(kvp.Value.Value.Value));
                else if (kvp.Key == "Status")
                    LobbyEvents.InvokeLobbyStatusChanged((LobbyStatus)int.Parse(kvp.Value.Value.Value));
            }
        }

        private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> changes)
        {
            List<Player> currentPlayers = lobby.Players;
            List<Player> tempPlayersList = new(cachedPlayersList);

            UpdateCachedPlayers();

            if (showDebugMessages) Debug.Log($"Player data changed: {changes.Count} players");
            foreach (var kvp in changes)
            {
                string playerId = tempPlayersList[kvp.Key].Id;
                Player changedPlayer = currentPlayers.Find(player => player.Id == playerId);

                if (showDebugMessages) Debug.Log($"- Player {playerId} data changed: {kvp.Value.Count} fields");
                foreach (var dataChange in kvp.Value)
                {
                    if (showDebugMessages) Debug.Log($"-- {dataChange.Key}: {dataChange.Value.Value.Value}");
                    if (dataChange.Key == "Status")
                        LobbyEvents.InvokePlayerStatusChanged(changedPlayer, (PlayerStatus)int.Parse(dataChange.Value.Value.Value));
                    else if (dataChange.Key == "Team")
                        LobbyEvents.InvokePlayerTeamChanged(changedPlayer, (Team)int.Parse(dataChange.Value.Value.Value));
                }
            }
        }

        private void OnKickedFromLobby()
        {
            if (isVoluntarilyLeaving)
            {
                if (lobby.Players.Count > 1)
                {
                    if (showDebugMessages) Debug.Log($"Left lobby: {lobby.Name}");
                    LobbyEvents.InvokeLobbyLeft(OperationResult.SuccessResult("LeaveLobby", $"Left lobby: {lobby.Name}"));
                }
                else
                {
                    if (showDebugMessages) Debug.Log($"Left and deleted lobby: {lobby.Name}");
                    LobbyEvents.InvokeLobbyLeft(OperationResult.SuccessResult("LeaveLobby", $"Left and deleted lobby: {lobby.Name}"));
                }
            }
            else
            {
                if (showDebugMessages) Debug.Log($"Kicked from lobby: {lobby.Name}");
                LobbyEvents.InvokeLobbyKicked(OperationResult.ErrorResult("KickedFromLobby", $"Kicked from lobby: {lobby.Name}"));
            }

            ClearLobby();
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            Debug.Log($"Lobby connection state changed to: {state}");

            if (state == LobbyEventConnectionState.Subscribing)
                LobbyEvents.InvokePlayerConnecting(AuthenticationManager.Instance.LocalPlayer);
            else if (state == LobbyEventConnectionState.Subscribed)
                LobbyEvents.InvokePlayerConnected(AuthenticationManager.Instance.LocalPlayer);
            else if (state == LobbyEventConnectionState.Unsynced)
                LobbyEvents.InvokePlayerDisconnected(AuthenticationManager.Instance.LocalPlayer);
        }

        private void ClearLobby()
        {
            UnsubscribeFromLobbyEvents();
            if (_heartbeatCoroutine != null)
            {
                StopCoroutine(_heartbeatCoroutine);
                _heartbeatCoroutine = null;
            }
            lobby = null;
            cachedPlayersList.Clear();
        }
        #endregion
    }
}