// using System;
// using UnityEngine;
// using System.Threading;
// using System.Threading.Tasks;
// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Framework.Utilities;
// using Assets.Scripts.Framework.Managers;
// using Assets.Scripts.Game.Data;
// using Assets.Scripts.Game.Events;

// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Manages match configuration settings like map selection, round count, and match time.
//     /// Handles starting and tracking match state.
//     /// </summary>
//     public class MatchConfigManager : Singleton<MatchConfigManager>
//     {
//         #region Constants

//         /// <summary>
//         /// Constants related to match settings and configuration
//         /// </summary>
//         public static class MatchSettings
//         {
//             public const int MIN_ROUNDS = 1;
//             public const int MAX_ROUNDS = 12;
//             public const int MIN_MATCH_TIME = 1;
//             public const int MAX_MATCH_TIME = 10;
//             public const int DEFAULT_ROUND_COUNT = 5;
//             public const int DEFAULT_MATCH_TIME = 2;
//             public const int DEFAULT_MAP_INDEX = 0;
//         }


//         #endregion

//         #region Public Methods - Match Configuration

//         /// <summary>
//         /// Creates a default match configuration
//         /// </summary>
//         /// <returns>A new LobbyData object with default settings</returns>
//         public LobbyData CreateDefaultMatchConfig()
//         {
//             var lobbyData = new LobbyData();
//             lobbyData.Initialize();

//             // Set default values
//             lobbyData.RoundCount = MatchSettings.DEFAULT_ROUND_COUNT;
//             lobbyData.MatchTimeMinutes = MatchSettings.DEFAULT_MATCH_TIME;
//             lobbyData.MapIndex = MatchSettings.DEFAULT_MAP_INDEX;

//             return lobbyData;
//         }

//         /// <summary>
//         /// Updates the selected map/arena (host only)
//         /// </summary>
//         /// <param name="lobbyData">The current lobby data</param>
//         /// <param name="mapIndex">The index of the selected map</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SelectMap(LobbyData lobbyData, int mapIndex)
//         {
//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can change the map.");

//             lobbyData.MapIndex = mapIndex;
//             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
//         }

//         /// <summary>
//         /// Updates the round count setting (host only)
//         /// </summary>
//         /// <param name="lobbyData">The current lobby data</param>
//         /// <param name="roundCount">The number of rounds per game</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SetRoundCount(LobbyData lobbyData, int roundCount)
//         {
//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can change match settings.");

//             lobbyData.RoundCount = Mathf.Clamp(roundCount, MatchSettings.MIN_ROUNDS, MatchSettings.MAX_ROUNDS);
//             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
//         }

//         /// <summary>
//         /// Updates the match time setting (host only)
//         /// </summary>
//         /// <param name="lobbyData">The current lobby data</param>
//         /// <param name="minutes">The match time in minutes</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SetMatchTime(LobbyData lobbyData, int minutes)
//         {
//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can change match settings.");

//             lobbyData.MatchTimeMinutes = Mathf.Clamp(minutes, MatchSettings.MIN_MATCH_TIME, MatchSettings.MAX_MATCH_TIME);
//             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
//         }

//         #endregion

//         #region Public Methods - Match Flow

//         /// <summary>
//         /// Starts the match when all players are ready (host only)
//         /// </summary>
//         /// <param name="cancellationToken">Optional cancellation token to cancel the countdown</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> StartMatch(CancellationToken cancellationToken = default)
//         {
//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can start the match.");

//             var players = GameLobbyManager.Instance.Players;

//             // Check if any players are not ready
//             if (players.Any(p => !p.IsReady))
//                 return OperationResult.FailureResult("NotAllReady", "Not all players are ready.");

//             // Check minimum players
//             if (players.Count < PlayerDataManager.PlayerConstants.MIN_PLAYERS_TO_START)
//                 return OperationResult.FailureResult("NotEnoughPlayers",
//                     $"Need at least {PlayerDataManager.PlayerConstants.MIN_PLAYERS_TO_START} players to start.");

//             // Start countdown
//             try
//             {
//                 LobbyEvents.InvokeMatchStarting(MatchSettings.COUNTDOWN_SECONDS);
//                 await Task.Delay(MatchSettings.COUNTDOWN_SECONDS * 1000, cancellationToken);
//                 LobbyEvents.InvokeMatchStarted();

//                 return OperationResult.SuccessResult("MatchStarted", "Match started successfully.");
//             }
//             catch (OperationCanceledException)
//             {
//                 // Handle countdown cancellation (e.g., someone left the lobby during countdown)
//                 LobbyEvents.InvokeMatchCancelled("Countdown was cancelled");
//                 return OperationResult.FailureResult("Cancelled", "Match start was cancelled.");
//             }
//         }

//         /// <summary>
//         /// Reports match results in the lobby chat
//         /// </summary>
//         /// <param name="winningTeam">The team that won (0 or 1)</param>
//         /// <param name="teamAScore">Score for Team A</param>
//         /// <param name="teamBScore">Score for Team B</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public OperationResult ReportMatchResults(int winningTeam, int teamAScore, int teamBScore)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             string teamAName = PlayerDataManager.TeamIds.TEAM_A_NAME;
//             string teamBName = PlayerDataManager.TeamIds.TEAM_B_NAME;

//             string resultMessage;
//             if (winningTeam == PlayerDataManager.TeamIds.TEAM_A)
//                 resultMessage = $"<color=#FF5555>{teamAName} wins {teamAScore} - {teamBScore}!</color>";
//             else
//                 resultMessage = $"<color=#5555FF>{teamBName} wins {teamBScore} - {teamAScore}!</color>";

//             return LobbyChatManager.Instance.SendSystemMessage(resultMessage);
//         }

//         #endregion
//     }
// }