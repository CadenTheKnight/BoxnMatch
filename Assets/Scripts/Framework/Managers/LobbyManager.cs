using System;
using Steamworks;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Handles low-level interaction with Unity's Lobby service.
    /// </summary>
    public static class LobbyManager
    {
        /// <summary>
        /// Retrieves list of active lobbies matching the filters.
        /// </summary>
        /// <param name="count">The maximum number of lobbies to retrieve, default 25.</param>
        /// <param name="order">Optional order to apply to the query.</param>
        /// <param name="filters">Optional filters to apply to the query.</param>
        public static async Task QueryLobbies(int count = 25, List<QueryOrder> order = null, List<QueryFilter> filters = null)
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(new() { Count = count, Order = order, Filters = filters });
                LobbyEvents.InvokeLobbiesQueried(OperationResult.SuccessResult("QueryLobbies", $"Found {queryResponse.Results.Count} " + (queryResponse.Results.Count == 1 ? "lobby" : "lobbies"), queryResponse.Results));
            }
            catch (Exception e) { LobbyEvents.InvokeLobbiesQueried(OperationResult.ErrorResult("QueryLobbies", e.Message)); }
        }

        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or public.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="lobbyData">A dictionary of additional data to associate with the lobby.</param>
        public static async Task CreateLobby(string lobbyName, bool isPrivate, int maxPlayers, Dictionary<string, DataObject> lobbyData)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new() { IsPrivate = isPrivate, Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() }, Data = lobbyData });
                LobbyEvents.InvokeLobbyCreated(OperationResult.SuccessResult("CreateLobby", $"Created lobby {lobby.Name} with ID {lobby.Id}", lobby));
            }
            catch (Exception e) { LobbyEvents.InvokeLobbyCreated(OperationResult.ErrorResult("CreateLobby", e.Message)); }
        }

        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        public static async Task JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new() { Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() } });
                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyByCode", $"Joined lobby {lobby.Name} with ID {lobby.Id}", lobby));
            }
            catch (Exception e) { LobbyEvents.InvokeLobbyJoined(OperationResult.ErrorResult("JoinLobbyByCode", e.Message)); }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        public static async Task JoinLobbyById(string lobbyId)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new() { Player = new Player(AuthenticationService.Instance.PlayerId) { Data = new PlayerData(SteamUser.GetSteamID()).Serialize() } });
                LobbyEvents.InvokeLobbyJoined(OperationResult.SuccessResult("JoinLobbyById", $"Joined lobby {lobby.Name} with ID {lobby.Id}", lobby));
            }
            catch (Exception e) { LobbyEvents.InvokeLobbyJoined(OperationResult.ErrorResult("JoinLobbyById", e.Message)); }
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        public static async Task RejoinLobby()
        {
            try
            {
                List<string> joinedLobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();
                if (joinedLobbyIds.Count == 0) LobbyEvents.InvokeLobbyRejoined(OperationResult.WarningResult("RejoinLobby", "No joined lobbies found"));
                else
                {
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyIds[0]);
                    LobbyEvents.InvokeLobbyRejoined(OperationResult.SuccessResult("RejoinLobby", $"Rejoined lobby {lobby.Name} with ID {lobby.Id}", lobby));
                }
            }
            catch (Exception e) { LobbyEvents.InvokeLobbyRejoined(OperationResult.ErrorResult("RejoinLobby", e.Message)); }
        }

        /// <summary>
        /// Leaves the current lobby with the specified ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to leave.</param>
        /// <returns>An OperationResult.</returns>
        public static async Task<OperationResult> LeaveLobby(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
                return OperationResult.SuccessResult("LeaveLobby", $"Left lobby with ID {lobbyId}");
            }
            catch (Exception e) { return OperationResult.ErrorResult("LeaveLobby", e.Message); }
        }

        /// <summary>
        /// Kicks a player from the specified lobby using their ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby from which to kick the player.</param>
        /// <param name="playerId">The ID of the player to kick.</param>
        public static async Task KickPlayer(string lobbyId, string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                LobbyEvents.InvokePlayerKicked(OperationResult.SuccessResult("KickPlayer", $"Kicked player {playerId} from lobby with ID {lobbyId}"));
            }
            catch (Exception e) { LobbyEvents.InvokePlayerKicked(OperationResult.ErrorResult("KickPlayer", e.Message)); }
        }

        /// <summary>
        /// Updates the player data with the provided dictionary.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby where the player is located.</param>
        /// <param name="playerId">The ID of the player whose data is being updated.</param>
        /// <param name="playerData">The dictionary containing the player data to update.</param>
        /// <param name="connectionInfo">The optional connection info for the player.</param>
        /// <param name="allocationId">The optional allocation ID for the player.</param>
        /// <returns>An OperationResult.</returns>
        public static async Task<OperationResult> UpdatePlayerData(string lobbyId, string playerId, Dictionary<string, PlayerDataObject> playerData, string connectionInfo = null, string allocationId = null)
        {
            try
            {
                await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, new() { ConnectionInfo = connectionInfo, Data = playerData, AllocationId = allocationId });
                return OperationResult.SuccessResult("UpdatePlayerData", $"Updated player data for {playerId} in lobby with ID {lobbyId}", playerData);
            }
            catch (Exception e) { return OperationResult.ErrorResult("UpdatePlayerData", e.Message); }
        }

        /// <summary>
        /// Updates the lobby data with the provided dictionary.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to update.</param>
        /// <param name="lobbyData">The dictionary containing the lobby data to update.</param>
        /// <returns>An OperationResult.</returns>
        public static async Task<OperationResult> UpdateLobbyData(string lobbyId, Dictionary<string, DataObject> lobbyData)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyId, new() { Data = lobbyData });
                return OperationResult.SuccessResult("UpdateLobbyData", $"Updated lobby data for {lobby.Name} with ID {lobby.Id}", lobby);
            }
            catch (Exception e) { return OperationResult.ErrorResult("UpdateLobbyData", e.Message); }
        }
    }
}