using System;
using System.Linq;
using UnityEngine;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages player data including creation, updates, caching,
    /// and player selections such as characters, colors, and teams.
    /// </summary>
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        #region Constants

        /// <summary>
        /// Constants related to team identification.
        /// </summary>
        public static class TeamIds
        {
            public const int TEAM_A = 0;
            public const int TEAM_B = 1;
            public const string TEAM_A_NAME = "Team Red";
            public const string TEAM_B_NAME = "Team Blue";
        }

        /// <summary>
        /// Constants related to player configuration and limits.
        /// </summary>
        public static class PlayerConstants
        {
            public const int MAX_PLAYERS = 4;
            public const int MIN_PLAYERS_TO_START = 2;
            public const string DEFAULT_CHARACTER_ID = "character_1";
            public const string DEFAULT_COLOR_ID = "color_1";
            public const int DEFAULT_TEAM_ID = TeamIds.TEAM_A;
            public const bool DEFAULT_READY_STATUS = false;
            public const string DEFAULT_PLAYER_NAME = "Player";
        }

        private const string KEY_PLAYER_NAME = "PlayerName";
        private const string KEY_CHARACTER_ID = "CharacterId";
        private const string KEY_COLOR_ID = "ColorId";
        private const string KEY_TEAM_ID = "TeamId";
        private const string KEY_IS_READY = "IsReady";
        private const string KEY_IS_HOST = "IsHost";

        #endregion

        private readonly Dictionary<string, LobbyPlayerData> playerDataCache = new();

        #region Lifecycle Methods

        protected override void Awake()
        {
            base.Awake();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            LobbyEvents.OnLobbyUpdated += HandleLobbyUpdated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
        }

        private void UnregisterEvents()
        {
            LobbyEvents.OnLobbyUpdated -= HandleLobbyUpdated;
            LobbyEvents.OnLobbyJoined -= HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft -= HandleLobbyLeft;
            LobbyEvents.OnLobbyCreated -= HandleLobbyCreated;
        }

        #endregion

        #region Event Handlers

        private void HandleLobbyCreated(Lobby lobby)
        {
            SynchronizeWithLobby(lobby);
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            SynchronizeWithLobby(lobby);
        }

        private void HandleLobbyUpdated(Lobby lobby)
        {
            SynchronizeWithLobby(lobby);
        }

        private void HandleLobbyLeft()
        {
            ClearCache();
        }

        /// <summary>
        /// Synchronizes the player data cache with the lobby player list.
        /// </summary>
        /// <param name="lobby">The lobby to synchronize with.</param>
        private void SynchronizeWithLobby(Lobby lobby)
        {
            if (lobby == null) return;

            var existingPlayerIds = new HashSet<string>(playerDataCache.Keys);

            foreach (var player in lobby.Players)
            {
                UpdateCache(player);
                existingPlayerIds.Remove(player.Id);
            }

            foreach (var removedPlayerId in existingPlayerIds)
                playerDataCache.Remove(removedPlayerId);
        }

        #endregion

        #region Public Methods - Player Data Management

        /// <summary>
        /// Updates the player data cache with data from a player object.
        /// </summary>
        /// <param name="player">The Unity Player object.</param>
        public void UpdateCache(Player player)
        {
            if (player == null) return;

            var playerData = ConvertToLobbyPlayerData(player);

            if (playerDataCache.ContainsKey(player.Id))
                playerDataCache[player.Id] = playerData;
            else
                playerDataCache.Add(player.Id, playerData);
        }

        /// <summary>
        /// Gets player data from cache by player ID.
        /// </summary>
        /// <param name="playerId">The ID of the player to retrieve.</param>
        /// <returns>The player data, or null if not found.</returns>
        public LobbyPlayerData GetPlayerData(string playerId)
        {
            return playerDataCache.TryGetValue(playerId, out var playerData) ? playerData : null;
        }

        /// <summary>
        /// Gets the local player data.
        /// </summary>
        /// <returns>The local player's data, or null if not found.</returns>
        public LobbyPlayerData GetLocalPlayerData()
        {
            string playerId = AuthenticationService.Instance?.PlayerId;
            return !string.IsNullOrEmpty(playerId) ? GetPlayerData(playerId) : null;
        }

        /// <summary>
        /// Gets the team name based on team ID.
        /// </summary>
        /// <param name="teamId">The team ID (0 or 1).</param>
        /// <returns>The team name.</returns>
        public string GetTeamName(int teamId)
        {
            return teamId == TeamIds.TEAM_A ? TeamIds.TEAM_A_NAME : TeamIds.TEAM_B_NAME;
        }

        /// <summary>
        /// Clears all cached player data.
        /// </summary>
        public void ClearCache()
        {
            playerDataCache.Clear();
        }

        /// <summary>
        /// Gets all players in a specific team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <returns>List of players in the team.</returns>
        public List<LobbyPlayerData> GetPlayersInTeam(int teamId)
        {
            return playerDataCache.Values.Where(p => p.TeamId == teamId).ToList();
        }

        /// <summary>
        /// Gets count of players in each team.
        /// </summary>
        /// <returns>Tuple with counts (TeamA, TeamB).</returns>
        public (int teamACount, int teamBCount) GetTeamCounts()
        {
            int teamA = 0;
            int teamB = 0;

            foreach (var player in playerDataCache.Values)
            {
                if (player.TeamId == TeamIds.TEAM_A)
                    teamA++;
                else
                    teamB++;
            }

            return (teamA, teamB);
        }

        #endregion

        #region Public Methods - Player Actions

        /// <summary>
        /// Updates a player ready status.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="isReady">Whether the player is ready.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> SetPlayerReady(string playerId, bool isReady)
        {
            if (!playerDataCache.TryGetValue(playerId, out _))
                return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

            return await UpdatePlayerAttribute(playerId, KEY_IS_READY, isReady.ToString().ToLower());
        }

        /// <summary>
        /// Sets the local player ready status.
        /// </summary>
        /// <param name="isReady">Whether the player is ready.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public Task<OperationResult> SetLocalPlayerReady(bool isReady)
        {
            string localPlayerId = AuthenticationService.Instance.PlayerId;
            return SetPlayerReady(localPlayerId, isReady);
        }

        /// <summary>
        /// Updates a player character selection.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="characterId">The ID of the selected character.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> SelectCharacter(string playerId, string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return OperationResult.FailureResult("InvalidCharacterId", "Character ID cannot be empty.");

            return await UpdatePlayerAttribute(playerId, KEY_CHARACTER_ID, characterId);
        }

        /// <summary>
        /// Updates a player color selection.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="colorId">The ID of the selected color.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> SelectColor(string playerId, string colorId)
        {
            if (string.IsNullOrEmpty(colorId))
                return OperationResult.FailureResult("InvalidColorId", "Color ID cannot be empty.");

            return await UpdatePlayerAttribute(playerId, KEY_COLOR_ID, colorId);
        }

        /// <summary>
        /// Updates a player team selection.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="teamId">The ID of the selected team.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> SelectTeam(string playerId, int teamId)
        {
            teamId = Mathf.Clamp(teamId, TeamIds.TEAM_A, TeamIds.TEAM_B);

            return await UpdatePlayerAttribute(playerId, KEY_TEAM_ID, teamId.ToString());
        }

        /// <summary>
        /// Toggles the local player team between Team A and Team B.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> ToggleLocalPlayerTeam()
        {
            var localPlayerData = GetLocalPlayerData();
            if (localPlayerData == null)
                return OperationResult.FailureResult("PlayerNotFound", "Local player data not found.");

            int newTeamId = localPlayerData.TeamId == TeamIds.TEAM_A ? TeamIds.TEAM_B : TeamIds.TEAM_A;
            return await SelectTeam(localPlayerData.PlayerId, newTeamId);
        }

        /// <summary>
        /// Balances teams by moving players between them.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> BalanceTeams()
        {
            if (!LobbyManager.Instance.IsLobbyHost)
                return OperationResult.FailureResult("NotHost", "Only the host can balance teams.");

            var (teamACount, teamBCount) = GetTeamCounts();

            if (Math.Abs(teamACount - teamBCount) <= 1)
                return OperationResult.SuccessResult("AlreadyBalanced", "Teams are already balanced.");

            int fromTeam = teamACount > teamBCount ? TeamIds.TEAM_A : TeamIds.TEAM_B;
            int toTeam = fromTeam == TeamIds.TEAM_A ? TeamIds.TEAM_B : TeamIds.TEAM_A;
            int playersToMove = Math.Abs(teamACount - teamBCount) / 2;

            var localPlayerId = AuthenticationService.Instance.PlayerId;
            var candidatePlayers = playerDataCache.Values.Where(p => p.TeamId == fromTeam && p.PlayerId != localPlayerId).ToList();

            if (candidatePlayers.Count < playersToMove)
                candidatePlayers = playerDataCache.Values.Where(p => p.TeamId == fromTeam).ToList();

            int moved = 0;
            foreach (var player in candidatePlayers)
            {
                if (moved >= playersToMove) break;

                await SelectTeam(player.PlayerId, toTeam);
                moved++;
            }

            return OperationResult.SuccessResult("TeamsBalanced", $"Moved {moved} players to balance teams.");
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Updates a single player attribute.
        /// </summary>
        /// <param name="playerId">The player ID.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        private async Task<OperationResult> UpdatePlayerAttribute(string playerId, string key, string value)
        {
            if (string.IsNullOrEmpty(playerId))
                return OperationResult.FailureResult("InvalidPlayerId", "Player ID cannot be empty.");

            if (!LobbyManager.Instance.IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

            if (!playerDataCache.TryGetValue(playerId, out var playerData))
                return OperationResult.FailureResult("PlayerNotFound", "Player data not found.");

            try
            {
                await Lobbies.Instance.UpdatePlayerAsync(
                    LobbyManager.Instance.LobbyId,
                    playerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value) }
                        }
                    }
                );

                switch (key)
                {
                    case KEY_PLAYER_NAME:
                        playerData.PlayerName = value;
                        break;
                    case KEY_CHARACTER_ID:
                        playerData.CharacterId = value;
                        break;
                    case KEY_COLOR_ID:
                        playerData.ColorId = value;
                        break;
                    case KEY_TEAM_ID:
                        playerData.TeamId = int.Parse(value);
                        break;
                    case KEY_IS_READY:
                        playerData.IsReady = value.ToLower() == "true";
                        break;
                    case KEY_IS_HOST:
                        playerData.IsHost = value.ToLower() == "true";
                        break;
                }

                playerDataCache[playerId] = playerData;
                return OperationResult.SuccessResult("AttributeUpdated", $"Updated {key} to {value}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update player attribute: {ex.Message}");
                return OperationResult.FailureResult("UpdateFailed", ex.Message);
            }
        }

        /// <summary>
        /// Converts a Unity Player object to a LobbyPlayerData model.
        /// </summary>
        private LobbyPlayerData ConvertToLobbyPlayerData(Player player)
        {
            if (player == null || player.Data == null)
                return null;

            string playerName = GetPlayerValue(player, KEY_PLAYER_NAME, PlayerConstants.DEFAULT_PLAYER_NAME);
            string characterId = GetPlayerValue(player, KEY_CHARACTER_ID, PlayerConstants.DEFAULT_CHARACTER_ID);
            string colorId = GetPlayerValue(player, KEY_COLOR_ID, PlayerConstants.DEFAULT_COLOR_ID);

            bool isReady = bool.TryParse(GetPlayerValue(player, KEY_IS_READY, "false"), out bool readyResult) && readyResult;
            int.TryParse(GetPlayerValue(player, KEY_TEAM_ID, PlayerConstants.DEFAULT_TEAM_ID.ToString()), out int teamId);
            bool isHost = LobbyManager.Instance != null && LobbyManager.Instance.IsInLobby && LobbyManager.Instance.IsLobbyHost &&
                          player.Id == AuthenticationService.Instance.PlayerId;

            return new LobbyPlayerData(
                player.Id,
                playerName,
                characterId,
                colorId,
                teamId,
                isReady,
                isHost
            );
        }

        /// <summary>
        /// Gets a player data value with a default fallback.
        /// </summary>
        private string GetPlayerValue(Player player, string key, string defaultValue)
        {
            return player.Data != null && player.Data.TryGetValue(key, out var dataObject) ?
                dataObject.Value : defaultValue;
        }

        #endregion
    }
}