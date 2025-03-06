// using System;
// using System.Linq;
// using UnityEngine;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Data;
// using System.Collections.Generic;
// using Unity.Services.Authentication;
// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Manages team-related functionality including team assignments,
//     /// colors, balancing, and team statistics.
//     /// </summary>
//     public class LobbyTeamManager : Singleton<LobbyTeamManager>
//     {
//         #region Team Constants

//         /// <summary>
//         /// Constants related to team identification and properties
//         /// </summary>
//         public static class TeamConstants
//         {
//             public const int TEAM_A = 0;
//             public const int TEAM_B = 1;
//             public const string TEAM_A_NAME = "Team Red";
//             public const string TEAM_B_NAME = "Team Blue";
//             public const int MIN_PLAYERS_PER_TEAM = 1;
//             public const int MAX_PLAYERS_PER_TEAM = 2;
//         }

//         #endregion

//         #region Team Colors

//         [System.Serializable]
//         public class TeamColorScheme
//         {
//             public Color primaryColor = Color.red;
//             public Color secondaryColor = Color.white;
//             public Color uiColor = new(0.9f, 0.3f, 0.3f);
//             public Material primaryMaterial;
//             public Material secondaryMaterial;
//         }

//         [SerializeField]
//         private TeamColorScheme teamAColors = new()
//         {
//             primaryColor = Color.red,
//             uiColor = new Color(0.9f, 0.3f, 0.3f)
//         };

//         [SerializeField]
//         private TeamColorScheme teamBColors = new()
//         {
//             primaryColor = Color.blue,
//             uiColor = new Color(0.3f, 0.3f, 0.9f)
//         };

//         #endregion

//         #region Team Player Management

//         /// <summary>
//         /// Gets the number of players in the specified team
//         /// </summary>
//         /// <param name="teamId">The team ID to check</param>
//         /// <returns>The number of players in the team</returns>
//         public int GetTeamPlayerCount(int teamId)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return 0;

//             return GameLobbyManager.Instance.Players.Count(p => p.TeamId == teamId);
//         }

//         /// <summary>
//         /// Gets players in the specified team
//         /// </summary>
//         /// <param name="teamId">The team ID to get players for</param>
//         /// <returns>List of players in the specified team</returns>
//         public List<LobbyPlayerData> GetTeamPlayers(int teamId)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return new List<LobbyPlayerData>();

//             return GameLobbyManager.Instance.Players
//                 .Where(p => p.TeamId == teamId)
//                 .ToList();
//         }

//         /// <summary>
//         /// Checks if the specified team has room for more players
//         /// </summary>
//         /// <param name="teamId">The team ID to check</param>
//         /// <returns>True if the team has room, false if full</returns>
//         public bool HasTeamSpace(int teamId)
//         {
//             return GetTeamPlayerCount(teamId) < TeamConstants.MAX_PLAYERS_PER_TEAM;
//         }

//         /// <summary>
//         /// Gets the suggested team for a new player to join (the team with fewer players)
//         /// </summary>
//         /// <returns>The team ID that has fewer players</returns>
//         public int GetSuggestedTeam()
//         {
//             int teamACount = GetTeamPlayerCount(TeamConstants.TEAM_A);
//             int teamBCount = GetTeamPlayerCount(TeamConstants.TEAM_B);

//             return teamACount <= teamBCount ? TeamConstants.TEAM_A : TeamConstants.TEAM_B;
//         }

//         #endregion

//         #region Team Properties

//         /// <summary>
//         /// Gets the team name for a team ID
//         /// </summary>
//         /// <param name="teamId">The team ID</param>
//         /// <returns>The team name</returns>
//         public string GetTeamName(int teamId)
//         {
//             return teamId == TeamConstants.TEAM_A ?
//                 TeamConstants.TEAM_A_NAME :
//                 TeamConstants.TEAM_B_NAME;
//         }

//         /// <summary>
//         /// Gets the primary color for a team
//         /// </summary>
//         /// <param name="teamId">The team ID</param>
//         /// <returns>The team's primary color</returns>
//         public Color GetTeamColor(int teamId)
//         {
//             return teamId == TeamConstants.TEAM_A ?
//                 teamAColors.primaryColor :
//                 teamBColors.primaryColor;
//         }

//         /// <summary>
//         /// Gets the UI color for a team
//         /// </summary>
//         /// <param name="teamId">The team ID</param>
//         /// <returns>The team's UI color</returns>
//         public Color GetTeamUIColor(int teamId)
//         {
//             return teamId == TeamConstants.TEAM_A ?
//                 teamAColors.uiColor :
//                 teamBColors.uiColor;
//         }

//         /// <summary>
//         /// Gets the primary material for a team
//         /// </summary>
//         /// <param name="teamId">The team ID</param>
//         /// <returns>The team's primary material</returns>
//         public Material GetTeamMaterial(int teamId)
//         {
//             return teamId == TeamConstants.TEAM_A ?
//                 teamAColors.primaryMaterial :
//                 teamBColors.primaryMaterial;
//         }

//         #endregion

//         #region Team Assignments

//         /// <summary>
//         /// Updates a player's team assignment
//         /// </summary>
//         /// <param name="playerId">The player ID to update</param>
//         /// <param name="teamId">The team to assign the player to</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> AssignPlayerToTeam(string playerId, int teamId)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             if (teamId != TeamConstants.TEAM_A && teamId != TeamConstants.TEAM_B)
//                 return OperationResult.FailureResult("InvalidTeam", "Invalid team ID.");

//             if (!HasTeamSpace(teamId))
//                 return OperationResult.FailureResult("TeamFull", $"{GetTeamName(teamId)} is full.");

//             var playerData = PlayerDataManager.Instance.GetPlayerData(playerId);
//             if (playerData == null)
//                 return OperationResult.FailureResult("NoPlayerData", "Player data not found.");

//             if (playerData.TeamId == teamId)
//                 return OperationResult.SuccessResult("AlreadyOnTeam", "Player is already on this team.");

//             playerData.TeamId = teamId;

//             var result = await PlayerDataManager.Instance.UpdatePlayerData(playerId, playerData.Serialize());

//             if (result.Success)
//             {
//                 string teamName = GetTeamName(teamId);
//                 Debug.Log($"Player {playerData.PlayerName} assigned to {teamName}");

//                 if (playerId == AuthenticationService.Instance.PlayerId)
//                     LobbyChatManager.Instance.SendSystemMessage($"{playerData.PlayerName} joined {teamName}.");
//             }

//             return result;
//         }

//         /// <summary>
//         /// Switches the local player to the specified team
//         /// </summary>
//         /// <param name="teamId">The team to join</param>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> JoinTeam(int teamId)
//         {
//             string localPlayerId = AuthenticationService.Instance.PlayerId;
//             return await AssignPlayerToTeam(localPlayerId, teamId);
//         }

//         /// <summary>
//         /// Switches the local player to the opposite team
//         /// </summary>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> SwitchTeam()
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             var localPlayer = GameLobbyManager.Instance.LocalPlayer;
//             if (localPlayer == null)
//                 return OperationResult.FailureResult("NoLocalPlayer", "Local player data not found.");

//             int newTeam = localPlayer.TeamId == TeamConstants.TEAM_A ? TeamConstants.TEAM_B : TeamConstants.TEAM_A;

//             return await JoinTeam(newTeam);
//         }

//         #endregion

//         #region Team Balance

//         /// <summary>
//         /// Automatically balances teams by moving players to ensure teams have equal numbers
//         /// </summary>
//         /// <returns>Operation result indicating success or failure</returns>
//         public async Task<OperationResult> BalanceTeams()
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             if (!GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Only the host can balance teams.");

//             int teamACount = GetTeamPlayerCount(TeamConstants.TEAM_A);
//             int teamBCount = GetTeamPlayerCount(TeamConstants.TEAM_B);

//             if (Math.Abs(teamACount - teamBCount) <= 1)
//                 return OperationResult.SuccessResult("AlreadyBalanced", "Teams are already balanced.");

//             int fromTeam = teamACount > teamBCount ? TeamConstants.TEAM_A : TeamConstants.TEAM_B;
//             int toTeam = fromTeam == TeamConstants.TEAM_A ? TeamConstants.TEAM_B : TeamConstants.TEAM_A;
//             int playersToMove = Math.Abs(teamACount - teamBCount) / 2;

//             var localPlayerId = AuthenticationService.Instance.PlayerId;
//             var candidatePlayers = GetTeamPlayers(fromTeam)
//                 .Where(p => p.PlayerId != localPlayerId)
//                 .ToList();

//             if (candidatePlayers.Count < playersToMove)
//             {
//                 candidatePlayers = GetTeamPlayers(fromTeam);
//             }

//             int moved = 0;
//             foreach (var player in candidatePlayers)
//             {
//                 if (moved >= playersToMove) break;

//                 var result = await AssignPlayerToTeam(player.PlayerId, toTeam);
//                 if (result.Success)
//                     moved++;
//             }

//             if (moved > 0)
//             {
//                 LobbyChatManager.Instance.SendSystemMessage($"Balanced teams by moving {moved} players.");
//                 return OperationResult.SuccessResult("TeamsBalanced", $"Moved {moved} players to balance teams.");
//             }
//             else
//             {
//                 return OperationResult.FailureResult("BalanceFailed", "Failed to move players between teams.");
//             }
//         }

//         /// <summary>
//         /// Checks if teams are evenly balanced (player counts differ by at most 1)
//         /// </summary>
//         /// <returns>True if teams are balanced, false otherwise</returns>
//         public bool AreTeamsBalanced()
//         {
//             int teamACount = GetTeamPlayerCount(TeamConstants.TEAM_A);
//             int teamBCount = GetTeamPlayerCount(TeamConstants.TEAM_B);

//             return Math.Abs(teamACount - teamBCount) <= 1;
//         }

//         /// <summary>
//         /// Checks if teams are valid for starting a match
//         /// </summary>
//         /// <param name="errorMessage">Output error message if teams are invalid</param>
//         /// <returns>True if teams are valid for starting a match, false otherwise</returns>
//         public bool AreTeamsValidForMatch(out string errorMessage)
//         {
//             errorMessage = string.Empty;

//             int teamACount = GetTeamPlayerCount(TeamConstants.TEAM_A);
//             int teamBCount = GetTeamPlayerCount(TeamConstants.TEAM_B);

//             if (teamACount < TeamConstants.MIN_PLAYERS_PER_TEAM)
//             {
//                 errorMessage = $"{TeamConstants.TEAM_A_NAME} needs at least {TeamConstants.MIN_PLAYERS_PER_TEAM} player(s)";
//                 return false;
//             }

//             if (teamBCount < TeamConstants.MIN_PLAYERS_PER_TEAM)
//             {
//                 errorMessage = $"{TeamConstants.TEAM_B_NAME} needs at least {TeamConstants.MIN_PLAYERS_PER_TEAM} player(s)";
//                 return false;
//             }

//             if (Math.Abs(teamACount - teamBCount) > 1)
//             {
//                 errorMessage = "Teams are not balanced";
//                 return false;
//             }

//             return true;
//         }

//         #endregion

//         #region Team Visuals

//         /// <summary>
//         /// Applies team coloring to a character GameObject
//         /// </summary>
//         /// <param name="character">The character GameObject to color</param>
//         /// <param name="teamId">The team ID for coloring</param>
//         /// <param name="colorSlot">The material slot name to color</param>
//         public void ApplyTeamColor(GameObject character, int teamId, string colorSlot = "Body")
//         {
//             if (character == null) return;

//             Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
//             Material teamMaterial = GetTeamMaterial(teamId);

//             foreach (var renderer in renderers)
//             {
//                 if (renderer.gameObject.name.Contains(colorSlot))
//                 {
//                     Material[] materials = renderer.materials;
//                     for (int i = 0; i < materials.Length; i++)
//                     {
//                         materials[i] = teamMaterial;
//                     }
//                     renderer.materials = materials;
//                 }
//             }
//         }

//         #endregion
//     }
// }