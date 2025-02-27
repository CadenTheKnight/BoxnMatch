// using System;
// using System.Linq;
// using UnityEngine;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Data;
// using System.Collections.Generic;
// using Unity.Services.Authentication;
// using Unity.Services.Lobbies.Models;
// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Framework.Managers;
// using Assets.Scripts.Framework.Utilities;


// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Manages player data for the Box'n Match game, including creation, updates, caching,
//     /// and player selections such as characters, colors, and teams.
//     /// </summary>
//     public class PlayerDataManager : Singleton<PlayerDataManager>
//     {
//         #region Constants

//         /// <summary>
//         /// Constants related to team identification
//         /// </summary>
//         public static class TeamIds
//         {
//             public const int TEAM_A = 0;
//             public const int TEAM_B = 1;
//             public const string TEAM_A_NAME = "Team Red";
//             public const string TEAM_B_NAME = "Team Blue";
//         }

//         /// <summary>
//         /// Constants related to player configuration and limits
//         /// </summary>
//         public static class PlayerConstants
//         {
//             public const int MAX_PLAYERS = 4;
//             public const int MIN_PLAYERS_TO_START = 2;
//             public const string DEFAULT_CHARACTER_ID = "character_1";
//             public const string DEFAULT_COLOR_ID = "color_1";
//             public const int DEFAULT_TEAM_ID = TeamIds.TEAM_A;
//             public const bool DEFAULT_READY_STATUS = false;
//             public const string DEFAULT_PLAYER_NAME = "Player";
//         }

//         #endregion

//         private readonly Dictionary<string, LobbyPlayerData> playerDataCache = new();

//         #region Public Methods - Player Data Management

//         /// <summary>
//         /// Creates initial player data for a new player
//         /// </summary>
//         /// <param name="isHost">Whether the player is the host</param>
//         /// <returns>Player data object with default values</returns>
//         public LobbyPlayerData CreateInitialPlayerData(bool isHost = false)
//         {
//             string playerId = AuthenticationService.Instance.PlayerId;
//             string playerName = PlayerPrefs.GetString("username", PlayerConstants.DEFAULT_PLAYER_NAME);

//             var playerData = new LobbyPlayerData(
//                 playerId,
//                 playerName,
//                 isHost
//             );

//             // Add to cache
//             if (playerDataCache.ContainsKey(playerId))
//                 playerDataCache[playerId] = playerData;
//             else
//                 playerDataCache.Add(playerId, playerData);

//             return playerData;
//         }

//         /// <summary>
//         /// Updates the player data cache with data from a player object
//         /// </summary>
//         /// <param name="playerId">The ID of the player</param>
//         /// <param name="data">The player data dictionary</param>
//         public void UpdateCache(string playerId, Dictionary<string, PlayerDataObject> data)
//         {
//             var playerData = new LobbyPlayerData(playerId, data);

//             if (playerDataCache.ContainsKey(playerId))
//                 playerDataCache[playerId] = playerData;
//             else
//                 playerDataCache.Add(playerId, playerData);
//         }

//         /// <summary>
//         /// Gets player data from cache by player ID
//         /// </summary>
//         /// <param name="playerId">The ID of the player to retrieve</param>
//         /// <returns>The player data, or null if not found</returns>
//         public LobbyPlayerData GetPlayerData(string playerId)
//         {
//             if (playerDataCache.TryGetValue(playerId, out var playerData))
//                 return playerData;

//             return null;
//         }

//         /// <summary>
//         /// Gets the local player's data
//         /// </summary>
//         /// <returns>The local player's data, or null if not found</returns>
//         public LobbyPlayerData GetLocalPlayerData()
//         {
//             return GetPlayerData(AuthenticationService.Instance.PlayerId);
//         }

//         /// <summary>
//         /// Gets the team name based on team ID
//         /// </summary>
//         /// <param name="teamId">The team ID (0 or 1)</param>
//         /// <returns>The team name</returns>
//         public string GetTeamName(int teamId)
//         {
//             return teamId == TeamIds.TEAM_A ? TeamIds.TEAM_A_NAME : TeamIds.TEAM_B_NAME;
//         }

//         /// <summary>
//         /// Clears all cached player data
//         /// </summary>
//         public void ClearCache()
//         {
//             playerDataCache.Clear();
//         }

//         #endregion

//         #region Public Methods - Player Actions

//         /// <summary>
//         /// Updates player data on the server
//         /// </summary>
//         /// <param name="playerId">The ID of the player to update</param>
//         /// <param name="data">Dictionary of player data values</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> UpdatePlayerData(string playerId, Dictionary<string, string> data)
//         {
//             if (string.IsNullOrEmpty(playerId))
//                 return OperationResult.FailureResult("InvalidPlayerId", "Player ID cannot be empty.");

//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             return await LobbyManager.Instance.UpdatePlayerData(playerId, data);
//         }

//         /// <summary>
//         /// Updates a player's ready status
//         /// </summary>
//         /// <param name="playerId">The ID of the player</param>
//         /// <param name="isReady">Whether the player is ready</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SetPlayerReady(string playerId, bool isReady)
//         {
//             if (!playerDataCache.TryGetValue(playerId, out var playerData))
//                 return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

//             playerData.IsReady = isReady;

//             var result = await UpdatePlayerData(playerId, playerData.Serialize());
//             return result;
//         }

//         /// <summary>
//         /// Updates a player's character selection
//         /// </summary>
//         /// <param name="playerId">The ID of the player</param>
//         /// <param name="characterId">The ID of the selected character</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SelectCharacter(string playerId, string characterId)
//         {
//             if (!playerDataCache.TryGetValue(playerId, out var playerData))
//                 return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

//             playerData.CharacterId = characterId;

//             var result = await UpdatePlayerData(playerId, playerData.Serialize());
//             return result;
//         }

//         /// <summary>
//         /// Updates a player's color selection
//         /// </summary>
//         /// <param name="playerId">The ID of the player</param>
//         /// <param name="colorId">The ID of the selected color</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SelectColor(string playerId, string colorId)
//         {
//             if (!playerDataCache.TryGetValue(playerId, out var playerData))
//                 return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

//             playerData.ColorId = colorId;

//             var result = await UpdatePlayerData(playerId, playerData.Serialize());
//             return result;
//         }

//         /// <summary>
//         /// Updates a player's team selection
//         /// </summary>
//         /// <param name="playerId">The ID of the player</param>
//         /// <param name="teamId">The ID of the selected team (0 or 1)</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SelectTeam(string playerId, int teamId)
//         {
//             if (!playerDataCache.TryGetValue(playerId, out var playerData))
//                 return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

//             // Ensure team ID is valid (0 or 1 for 2-team system)
//             teamId = Mathf.Clamp(teamId, TeamIds.TEAM_A, TeamIds.TEAM_B);

//             playerData.TeamId = teamId;

//             var result = await UpdatePlayerData(playerId, playerData.Serialize());
//             return result;
//         }

//         /// <summary>
//         /// Automatically balances teams by moving players
//         /// </summary>
//         /// <returns>Operation result with details about the balancing operation</returns>
//         public async Task<OperationResult> BalanceTeams()
//         {
//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can balance teams.");

//             var players = GameLobbyManager.Instance.Players;
//             int teamACount = players.Count(p => p.TeamId == TeamIds.TEAM_A);
//             int teamBCount = players.Count(p => p.TeamId == TeamIds.TEAM_B);

//             if (Math.Abs(teamACount - teamBCount) <= 1)
//                 return OperationResult.SuccessResult("AlreadyBalanced", "Teams are already balanced.");

//             int fromTeam = teamACount > teamBCount ? TeamIds.TEAM_A : TeamIds.TEAM_B;
//             int toTeam = fromTeam == TeamIds.TEAM_A ? TeamIds.TEAM_B : TeamIds.TEAM_A;
//             int playersToMove = Math.Abs(teamACount - teamBCount) / 2;

//             // Get players from the team with more players, prioritizing non-local players
//             var localPlayerId = AuthenticationService.Instance.PlayerId;
//             var candidatePlayers = players
//                 .Where(p => p.TeamId == fromTeam && p.PlayerId != localPlayerId)
//                 .ToList();

//             if (candidatePlayers.Count < playersToMove)
//             {
//                 candidatePlayers = players
//                     .Where(p => p.TeamId == fromTeam)
//                     .ToList();
//             }

//             int moved = 0;
//             foreach (var player in candidatePlayers)
//             {
//                 if (moved >= playersToMove) break;

//                 await SelectTeam(player.PlayerId, toTeam);
//                 moved++;
//             }

//             return OperationResult.SuccessResult("TeamsBalanced", $"Moved {moved} players to balance teams.");
//         }

//         #endregion
//     }
// }