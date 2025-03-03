using System;
using System.Linq;
using UnityEngine;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers = 4)
        {
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = false,
                Player = PlayerDataManager.Instance.GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Standard") },
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, "Default")},
                    { "RoundCount", new DataObject(DataObject.VisibilityOptions.Public, "3")},
                    { "MatchTimeMinutes", new DataObject(DataObject.VisibilityOptions.Public, "5")},
                    { "MapIndex", new DataObject(DataObject.VisibilityOptions.Public, "0")}
                }
            };

            return await LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, createLobbyOptions);
        }


        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> ToggleReadyStatus()
        {
            try
            {
                var currentPlayer = LobbyManager.Instance.lobby.Players
                    .FirstOrDefault(p => p.Id == AuthenticationManager.Instance.PlayerId);

                if (currentPlayer == null)
                    return OperationResult.FailureResult("PlayerNotFound", "Player not found in lobby");

                bool isCurrentlyReady = currentPlayer.Data["IsReady"].Value == "true";
                bool newReadyStatus = !isCurrentlyReady;

                await Lobbies.Instance.UpdatePlayerAsync(
                    LobbyManager.Instance.LobbyId,
                    AuthenticationManager.Instance.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                    { "IsReady", new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Member,
                        newReadyStatus.ToString().ToLower()) }
                        }
                    });

                return OperationResult.SuccessResult(
                    "ReadyStatusToggled",
                    $"You are now {(newReadyStatus ? "ready" : "unready")}"
                );
            }
            catch (Exception ex)
            {
                return OperationResult.FailureResult("ToggleReadyError", ex.Message);
            }
        }

        /// <summary>
        /// Checks if the current player is ready.
        /// </summary>
        /// <returns>True if the player is ready, false otherwise.</returns>
        public bool IsPlayerReady()
        {
            if (!LobbyManager.Instance.IsInLobby)
                return false;

            var localPlayer = LobbyManager.Instance.lobby.Players
                .FirstOrDefault(p => p.Id == AuthenticationManager.Instance.PlayerId);

            if (localPlayer == null)
                return false;

            return localPlayer.Data["IsReady"].Value == "true";
        }


        /// <summary>
        /// Sets all players to unready.
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> SetAllPlayersUnready()
        {
            try
            {
                var updateTasks = new List<Task>();

                foreach (var player in LobbyManager.Instance.Players)
                {
                    updateTasks.Add(Lobbies.Instance.UpdatePlayerAsync(
                        LobbyManager.Instance.LobbyId,
                        player.Id,
                        new UpdatePlayerOptions
                        {
                            Data = new Dictionary<string, PlayerDataObject>
                            {
                        { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
                            }
                        }
                    ));
                }

                await Task.WhenAll(updateTasks);

                return OperationResult.SuccessResult("AllPlayersUnreadied", "All players have been set to unready");
            }
            catch (Exception ex)
            {
                return OperationResult.FailureResult("UnreadyError", ex.Message);
            }
        }
    }
}