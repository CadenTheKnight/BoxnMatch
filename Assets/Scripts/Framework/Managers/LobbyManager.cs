using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.Managers;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Assets.Scripts.Game.Data;
using Steamworks;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Handles low-level interaction with Unity's Lobby service.
    /// </summary>
    public class LobbyManager : Singleton<LobbyManager>
    {
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = false;

        public Lobby Lobby { get; private set; } = null;
        private Coroutine _heartbeatCoroutine = null;
        private ILobbyEvents lobbyEvents = null;
        private bool isVoluntarilyLeaving = false;
        private List<Player> cachedPlayersList = new();

        /// <summary>
        /// Retrieves list of active lobbies matching the filters.
        /// </summary>
        /// <param name="count">The maximum number of lobbies to retrieve, default 25.</param>
        /// <param name="order">Optional order to apply to the query.</param>
        /// <param name="filters">Optional filters to apply to the query.</param>
        /// <returns>A list of lobbies matching the query.</returns>
        public async Task<List<Lobby>> QueryLobbies(int count = 25, List<QueryOrder> order = null, List<QueryFilter> filters = null)
        {
            QueryLobbiesOptions queryLobbiesOptions = new() { Count = count, Order = order, Filters = filters };

            try
            {
                QueryResponse queryResponse = await WithTimeout(LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions), 5000, "QueryLobbies");
                return queryResponse.Results;
            }
            catch (Exception) { return new List<Lobby>(); }
        }

        /// <summary>
        /// Retrieves a list of all lobbies the player is in.
        /// </summary>
        /// <returns>A list of lobby IDs the player is currently in.</returns>
        public async Task<List<string>> GetJoinedLobbies()
        {
            return await LobbyService.Instance.GetJoinedLobbiesAsync();
        }

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

        #region Lobby Management
        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or public.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="lobbyData">A dictionary of additional data to associate with the lobby.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, bool isPrivate, int maxPlayers, Dictionary<string, DataObject> lobbyData)
        {
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = isPrivate,
                Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() },
                Data = lobbyData
            };

            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions), 5000, "CreateLobby");
                await SubscribeToLobbyEvents(Lobby.Id);
                _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(Lobby.Id, 6f));
                return OperationResult.SuccessResult("CreateLobby", $"Created lobby: {Lobby.Name}");
            }
            catch (Exception e)
            {
                await ClearLobby();
                return OperationResult.ErrorResult("CreateLobbyError", e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new() { Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() } };

            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions), 5000, "JoinLobbyByCode");
                await SubscribeToLobbyEvents(Lobby.Id);
                return OperationResult.SuccessResult("JoinLobbyByCode", $"Joined lobby: {Lobby.Name}");
            }
            catch (Exception e)
            {
                await ClearLobby();
                return OperationResult.ErrorResult("JoinLobbyByCodeError", e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new() { Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() } };

            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions), 5000, "JoinLobbyById");
                await SubscribeToLobbyEvents(Lobby.Id);
                return OperationResult.SuccessResult("JoinLobbyById", $"Joined lobby: {Lobby.Name}");
            }
            catch (Exception e)
            {
                await ClearLobby();
                return OperationResult.ErrorResult("JoinLobbyByIdError", e.Message);
            }
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        /// <param name="joinedLobbyIds">A list of lobby IDs the player is currently in.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> RejoinLobby(List<string> joinedLobbyIds)
        {
            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.GetLobbyAsync(joinedLobbyIds[0]), 5000, "RejoinLobby");
                await SubscribeToLobbyEvents(Lobby.Id);
                return OperationResult.SuccessResult("RejoinLobby", $"Rejoined lobby: {Lobby.Name}");
            }
            catch (Exception e)
            {
                await ClearLobby();
                return OperationResult.ErrorResult("RejoinLobbyError", e.Message);
            }
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <returns>Bool indicating success or failure.</returns>
        public async Task<bool> LeaveLobby()
        {
            try
            {
                isVoluntarilyLeaving = true;
                await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, AuthenticationService.Instance.PlayerId);
                return true;
            }
            catch (Exception)
            {
                isVoluntarilyLeaving = false;
                return false;
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> KickPlayer(Player player)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, player.Id);
                return OperationResult.SuccessResult("KickPlayer", $"Kicked player: {player.Id} from lobby: {Lobby.Name}");
            }
            catch (Exception e) { return OperationResult.ErrorResult("KickPlayerError", e.Message); }
        }
        #endregion

        #region Data Management
        /// <summary>
        /// Updates the player data with the provided dictionary.
        /// </summary>
        /// <param name="playerId">The ID of the player whose data is being updated.</param>
        /// <param name="playerData">The dictionary containing the player data to update.</param>
        /// <param name="allocationId">The optional allocation ID for the player.</param>
        /// <param name="connectionData">The optional connection data for the player.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> UpdatePlayerData(string playerId, Dictionary<string, PlayerDataObject> playerData, string allocationId = default, string connectionData = default)
        {
            UpdatePlayerOptions updatePlayerOptions = new() { ConnectionInfo = connectionData, Data = playerData, AllocationId = allocationId };

            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.UpdatePlayerAsync(Lobby.Id, playerId, updatePlayerOptions), 5000, "UpdatePlayerData");
                return OperationResult.SuccessResult("UpdatePlayerData", $"Updated player data for {playerId} in lobby: {Lobby.Name}");
            }
            catch (Exception e) { return OperationResult.ErrorResult("UpdatePlayerDataError", e.Message); }
        }

        /// <summary>
        /// Updates the lobby data with the provided dictionary.
        /// </summary>
        /// <param name="lobbyData">The dictionary containing the lobby data to update.</param>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> UpdateLobbyData(Dictionary<string, DataObject> lobbyData)
        {
            UpdateLobbyOptions updateLobbyOptions = new() { Data = lobbyData };

            try
            {
                Lobby = await WithTimeout(LobbyService.Instance.UpdateLobbyAsync(Lobby.Id, updateLobbyOptions), 5000, "UpdateLobbyData");
                return OperationResult.SuccessResult("UpdateLobbyData", $"Updated lobby data in lobby: {Lobby.Name}");
            }
            catch (Exception e) { return OperationResult.ErrorResult("UpdateLobbyDataError", e.Message); }
        }

        /// <summary>
        /// Handles the timeout for a given task.
        /// </summary>
        /// <param name="task">The task to wait for.</param>
        /// <param name="timeoutMs">The timeout in milliseconds.</param>
        /// <param name="operationName">The name of the operation for logging.</param>
        /// <returns>The result of the task if completed within the timeout.</returns>
        /// <exception cref="TimeoutException">Thrown if the task times out.</exception>
        private async Task<T> WithTimeout<T>(Task<T> task, int timeoutMs = 5000, string operationName = "Operation")
        {
            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task) return await task;
            else throw new TimeoutException($"{operationName} timed out");
        }
        #endregion

        #region Lobby Event Callbacks
        /// <summary>
        /// Sets up and subscribes to Unity's built-in lobby events.
        /// </summary>
        private async Task SubscribeToLobbyEvents(string lobbyId)
        {
            await UnsubscribeFromLobbyEvents();

            LobbyEventCallbacks callbacks = new();

            callbacks.LobbyChanged += OnLobbyChanged;
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;
            callbacks.DataChanged += OnDataChanged;
            callbacks.PlayerDataChanged += OnPlayerDataChanged;
            callbacks.KickedFromLobby += OnKickedFromLobby;
            callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

            lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, callbacks);
        }

        /// <summary>
        /// Unsubscribes from all lobby events.
        /// </summary>
        private async Task UnsubscribeFromLobbyEvents()
        {
            if (lobbyEvents == null) return;

            await lobbyEvents.UnsubscribeAsync();

            lobbyEvents = null;
        }

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            if (showDebugMessages) Debug.Log("Lobby changed event received");

            if (lobbyChanges.HostId.Changed)
            {
                if (showDebugMessages) Debug.Log($"Host changed to {lobbyChanges.HostId.Value}");
                if (AuthenticationService.Instance.PlayerId == lobbyChanges.HostId.Value)
                {
                    if (showDebugMessages) Debug.Log("Local player is now host - setting to Ready");
                    if (_heartbeatCoroutine == null)
                    {
                        _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(Lobby.Id, 6f));
                        if (showDebugMessages) Debug.Log("Started heartbeat coroutine as new host");
                    }
                }
                LobbyEvents.InvokeLobbyHostMigrated(lobbyChanges.HostId.Value);
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

            cachedPlayersList = new List<Player>(Lobby.Players);
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> playersJoined)
        {
            foreach (LobbyPlayerJoined playerJoined in playersJoined)
            {
                cachedPlayersList.Add(playerJoined.Player);
                if (showDebugMessages) Debug.Log($"Player {playerJoined.Player.Id} joined the lobby");
                LobbyEvents.InvokePlayerJoined(playerJoined.Player.Id);
            }
        }

        private void OnPlayerLeft(List<int> playerIndices)
        {
            foreach (int playerIndex in playerIndices)
            {
                if (showDebugMessages) Debug.Log($"Player {cachedPlayersList[playerIndex].Id} left the lobby");
                LobbyEvents.InvokePlayerLeft(cachedPlayersList[playerIndex].Id);
                cachedPlayersList.RemoveAt(playerIndex);
            }
        }

        private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dataChanges)
        {
            if (showDebugMessages) Debug.Log($"Lobby data changed: {dataChanges.Count} " + (dataChanges.Count == 1 ? "field" : "fields"));
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
            if (showDebugMessages) Debug.Log($"Player data changed: {changes.Count} " + (changes.Count == 1 ? "player" : "players"));
            foreach (var kvp in changes)
                foreach (var dataChange in kvp.Value)
                    if (showDebugMessages) Debug.Log($"{Lobby.Players[kvp.Key].Id} - {dataChange.Key}: {dataChange.Value.Value.Value}");
        }

        private async void OnKickedFromLobby()
        {
            if (!isVoluntarilyLeaving) NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("KickedFromLobby", "Kicked from lobby"));
            else if (Lobby.Players.Count > 1) NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("LeftLobby", "Left lobby"));
            else NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("LeftLobby", "Left and deleted lobby"));

            SceneManager.LoadSceneAsync("Main");

            await ClearLobby();
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            Debug.Log($"Lobby connection state changed to: {state}");

            if (state == LobbyEventConnectionState.Subscribing)
                LobbyEvents.InvokePlayerConnecting(AuthenticationService.Instance.PlayerId);
            else if (state == LobbyEventConnectionState.Subscribed)
                LobbyEvents.InvokePlayerConnected(AuthenticationService.Instance.PlayerId);
            else if (state == LobbyEventConnectionState.Unsynced)
                LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
            // else if (state == LobbyEventConnectionState.Unsubscribed)
            //     LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
            // else if (state == LobbyEventConnectionState.Error)
            //     LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
        }

        private async void OnApplicationQuit()
        {
            if (Lobby != null)
            {
                if (showDebugMessages) Debug.Log("Application quitting - clearing lobby");
                await ClearLobby();
            }
        }

        private async Task ClearLobby()
        {
            await UnsubscribeFromLobbyEvents();
            if (_heartbeatCoroutine != null)
            {
                StopCoroutine(_heartbeatCoroutine);
                _heartbeatCoroutine = null;
            }
            Lobby = null;
        }
        #endregion
    }
}