using System;
using System.Linq;
using UnityEngine;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Assets.Scripts.Framework;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// Acts as a bridge between the framework's LobbyManager and the game's specific needs.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers = 4, bool isPrivate = false)
        // maxplayers and isprivate hardcoded until lobby creation is updated, will probably pass a whole object with all the new data eventually
        {
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = isPrivate,
                Player = PlayerDataManager.Instance.GetNewPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Standard") },
                    { "MapName", new DataObject(DataObject.VisibilityOptions.Public, "Default")},
                    { "RoundCount", new DataObject(DataObject.VisibilityOptions.Public, "5")},
                    { "RoundTimeMinutes", new DataObject(DataObject.VisibilityOptions.Public, "2")},
                    { "GameInProgress", new DataObject(DataObject.VisibilityOptions.Public, "false")},
                }
            };

            return await LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, createLobbyOptions);
        }

        /// <summary>
        /// Checks if the given player is ready.
        /// </summary>
        /// <param name="playerId">The ID of the player to check.</param>
        /// <returns>True if the player is ready, false otherwise.</returns>
        public bool IsPlayerReady(string playerId)
        {
            return LobbyManager.Instance.Players.FirstOrDefault(p => p.Id == playerId)?.Data["IsReady"].Value == "true";
        }


        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> ToggleReadyStatus()
        {
            try
            {
                bool newReadyStatus = !IsPlayerReady(AuthenticationManager.Instance.PlayerId);

                UpdatePlayerOptions options = new()
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newReadyStatus.ToString().ToLower()) }
                    }
                };

                await LobbyManager.Instance.UpdatePlayer(AuthenticationManager.Instance.PlayerId, options);

                LobbyEvents.InvokePlayerReadyChanged(AuthenticationManager.Instance.PlayerId, newReadyStatus);

                return OperationResult.SuccessResult("ReadyStatusToggled", $"{(newReadyStatus ? "Ready" : "Not Ready")}");
            }
            catch (Exception ex)
            {
                return OperationResult.FailureResult("ToggleReadyError", ex.Message);
            }
        }



        /// <summar>
        /// Invokes the appropriate event based on the readiness of all players in the lobby.
        /// </summary>
        public void IsLobbyReady()
        {
            if (!LobbyManager.Instance.IsInLobby)
                return;

            bool allReady = true;

            foreach (var player in LobbyManager.Instance.Players)
                if (!IsPlayerReady(player.Id))
                {
                    allReady = false;
                    break;
                }

            if (allReady)
                LobbyEvents.InvokeAllPlayersReady();
            else
                LobbyEvents.InvokeNotAllPlayersReady();
        }

        /// <summary>
        /// Sets all players to unready.
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> SetAllPlayersUnready()
        {
            try
            {
                foreach (var player in LobbyManager.Instance.Players)
                {
                    UpdatePlayerOptions options = new()
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                            {
                                { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
                            }
                    };

                    await LobbyManager.Instance.UpdatePlayer(player.Id, options);
                }

                return OperationResult.SuccessResult("AllPlayersUnreadied", "All players have been set to unready");
            }
            catch (Exception ex)
            {
                return OperationResult.FailureResult("UnreadyError", ex.Message);
            }
        }

        public async Task<OperationResult> SetGameInProgress(bool inProgress, string joinCode = "")
        {
            try
            {
                UpdateLobbyOptions options = new()
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameInProgress", new DataObject(DataObject.VisibilityOptions.Public, inProgress.ToString().ToLower()) }
                    }
                };

                if (!string.IsNullOrEmpty(joinCode))
                    options.Data.Add("JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode));

                await LobbyManager.Instance.UpdateLobby(LobbyManager.Instance.LobbyId, options);

                return OperationResult.SuccessResult("GameStateUpdated", $"Game is now {(inProgress ? "in progress" : "inactive")}");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }
    }
}