using System;
using UnityEngine;
using System.Linq;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Framework.Extensions;
using FrameworkEvents = Assets.Scripts.Framework.Events;


namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// Acts as a bridge between the framework's LobbyManager and the game's specific needs.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private readonly List<string> lobbyPlayerIds = new();
        private LobbyData lobbyData;

        #region Public Properties

        public bool IsHost => LobbyManager.Instance != null && LobbyManager.Instance.IsLobbyHost;
        public string LobbyName => LobbyManager.Instance != null ? LobbyManager.Instance.LobbyName : string.Empty;
        public string LobbyCode => LobbyManager.Instance != null ? LobbyManager.Instance.LobbyCode : string.Empty;
        public bool IsInLobby => LobbyManager.Instance != null && LobbyManager.Instance.IsInLobby;
        public int MaxPlayers => PlayerDataManager.PlayerConstants.MAX_PLAYERS;
        public int CurrentPlayerCount => lobbyPlayerIds?.Count ?? 0;
        public LobbyData CurrentLobbyData => lobbyData;

        public IReadOnlyList<LobbyPlayerData> Players
        {
            get
            {
                List<LobbyPlayerData> players = new();
                foreach (var playerId in lobbyPlayerIds)
                {
                    var player = PlayerDataManager.Instance.GetPlayerData(playerId);
                    if (player != null)
                        players.Add(player);
                }
                return players;
            }
        }

        public LobbyPlayerData LocalPlayer => PlayerDataManager.Instance.GetLocalPlayerData();

        #endregion

        #region Unity Lifecycle

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
            FrameworkEvents.LobbyEvents.OnLobbyCreated += OnLobbyCreated;
            FrameworkEvents.LobbyEvents.OnLobbyJoined += OnLobbyJoined;
            FrameworkEvents.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
            FrameworkEvents.LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            FrameworkEvents.LobbyEvents.OnPlayerJoined += OnPlayerJoined;
            FrameworkEvents.LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            FrameworkEvents.LobbyEvents.OnPlayerKicked += OnPlayerKicked;
            FrameworkEvents.LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
            FrameworkEvents.LobbyEvents.OnLobbyDataChanged += OnLobbyDataChanged;
        }

        private void UnregisterEvents()
        {
            FrameworkEvents.LobbyEvents.OnLobbyCreated -= OnLobbyCreated;
            FrameworkEvents.LobbyEvents.OnLobbyJoined -= OnLobbyJoined;
            FrameworkEvents.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            FrameworkEvents.LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            FrameworkEvents.LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
            FrameworkEvents.LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
            FrameworkEvents.LobbyEvents.OnPlayerKicked -= OnPlayerKicked;
            FrameworkEvents.LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
            FrameworkEvents.LobbyEvents.OnLobbyDataChanged -= OnLobbyDataChanged;
        }

        #endregion

        #region Event Handlers

        private void OnLobbyCreated(Lobby lobby)
        {
            RefreshLobbyData(lobby);
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            RefreshLobbyData(lobby);
        }

        private void OnLobbyLeft()
        {
            ClearLobbyData();
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            RefreshLobbyData(lobby);
        }

        private void OnPlayerJoined(string lobbyId, Player player)
        {
            if (!lobbyPlayerIds.Contains(player.Id))
            {
                lobbyPlayerIds.Add(player.Id);
                Debug.Log($"Player {player.GetPlayerName()} joined the lobby");
            }
        }

        private void OnPlayerLeft(string playerId)
        {
            var player = PlayerDataManager.Instance.GetPlayerData(playerId);
            if (player != null)
            {
                lobbyPlayerIds.Remove(playerId);
                Debug.Log($"Player {player.PlayerName} left the lobby");
            }

            CheckReadyStatus();
        }

        private void OnPlayerKicked(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                ClearLobbyData();
                Debug.Log("You were kicked from the lobby");
            }
            else
            {
                OnPlayerLeft(playerId);
                Debug.Log($"Player {playerId} was kicked from the lobby");
            }
        }

        private void OnPlayerDataChanged(string playerId, Dictionary<string, PlayerDataObject> data)
        {
            var previousPlayerData = PlayerDataManager.Instance.GetPlayerData(playerId);
            if (previousPlayerData == null)
            {
                Debug.LogWarning($"Received data for unknown player: {playerId}");
                return;
            }

            var previousCharacter = previousPlayerData.CharacterId;
            var previousColor = previousPlayerData.ColorId;
            var previousReady = previousPlayerData.IsReady;
            var previousTeam = previousPlayerData.TeamId;

            var newPlayerData = PlayerDataManager.Instance.GetPlayerData(playerId);

            if (newPlayerData.CharacterId != previousCharacter)
                LobbyEvents.InvokeCharacterSelected(playerId, newPlayerData.CharacterId);

            if (newPlayerData.ColorId != previousColor)
                LobbyEvents.InvokePlayerColorSelected(playerId, newPlayerData.ColorId);

            if (newPlayerData.IsReady != previousReady)
            {
                LobbyEvents.InvokePlayerReadyChanged(playerId, newPlayerData.IsReady);
                CheckReadyStatus();
            }

            if (newPlayerData.TeamId != previousTeam)
                LobbyEvents.InvokeTeamSelected(playerId, newPlayerData.TeamId);
        }

        private void OnLobbyDataChanged(Lobby lobby, Dictionary<string, DataObject> data)
        {
            var previousMapIndex = lobbyData?.MapIndex ?? 0;

            lobbyData = new LobbyData();
            lobbyData.Initialize(data);

            if (lobbyData.MapIndex != previousMapIndex)
                LobbyEvents.InvokeArenaSelected(lobbyData.MapIndex.ToString());

            LobbyEvents.InvokeMatchSettingsUpdated(lobbyData.Serialize());
        }

        #endregion

        #region Lobby Creation and Management

        /// <summary>
        /// Creates a Player object with the current player's name and other properties.
        /// </summary>
        /// <returns>The Player object for lobby operations.</returns>
        public Player GetPlayer(bool isReady = false, int teamId = 0)
        {
            string characterId = PlayerDataManager.PlayerConstants.DEFAULT_CHARACTER_ID;
            string colorId = PlayerDataManager.PlayerConstants.DEFAULT_COLOR_ID;
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");

            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                    { "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString().ToLower())},
                    { "TeamId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, teamId.ToString())},
                    { "CharacterId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, characterId)},
                    { "ColorId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, colorId)}
                }
            };
        }

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
                Player = GetPlayer(),
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
        /// Joins a lobby using a lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new() { Player = GetPlayer() };

            return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode, joinLobbyByCodeOptions);
        }

        /// <summary>
        /// Joins a lobby using a lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new() { Player = GetPlayer() };

            return await LobbyManager.Instance.JoinLobbyById(lobbyId, joinLobbyByIdOptions);
        }

        /// <summary>
        /// Leaves the current lobby
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> LeaveLobby()
        {
            if (!LobbyManager.Instance.IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

            return await LobbyManager.Instance.LeaveLobby();
        }

        /// <summary>
        /// Kicks a player from the lobby (host only)
        /// </summary>
        /// <param name="playerId">The ID of the player to kick</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> KickPlayer(string playerId)
        {
            if (!LobbyManager.Instance.IsLobbyHost)
                return OperationResult.FailureResult("NotHost", "Only the host can kick players.");

            return await LobbyManager.Instance.KickPlayer(playerId);
        }

        /// <summary>
        /// Refreshes the list of active lobbies.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> RefreshLobbyList()
        {
            return await LobbyManager.Instance.GetLobbies();
        }

        #endregion

        #region Player Actions

        /// <summary>
        /// Sets the local player's ready status
        /// </summary>
        /// <param name="isReady">Whether the player is ready</param>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> SetPlayerReady(bool isReady)
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            return PlayerDataManager.Instance.SetLocalPlayerReady(isReady);
        }

        /// <summary>
        /// Sets the local player's character selection
        /// </summary>
        /// <param name="characterId">The ID of the selected character</param>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> SelectCharacter(string characterId)
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            string localPlayerId = AuthenticationService.Instance.PlayerId;
            return PlayerDataManager.Instance.SelectCharacter(localPlayerId, characterId);
        }

        /// <summary>
        /// Sets the local player's color selection
        /// </summary>
        /// <param name="colorId">The ID of the selected color</param>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> SelectColor(string colorId)
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            string localPlayerId = AuthenticationService.Instance.PlayerId;
            return PlayerDataManager.Instance.SelectColor(localPlayerId, colorId);
        }

        /// <summary>
        /// Sets the local player's team selection
        /// </summary>
        /// <param name="teamId">The ID of the selected team (0 or 1)</param>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> SelectTeam(int teamId)
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            string localPlayerId = AuthenticationService.Instance.PlayerId;
            return PlayerDataManager.Instance.SelectTeam(localPlayerId, teamId);
        }

        /// <summary>
        /// Toggles the local player's team
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> ToggleTeam()
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            return PlayerDataManager.Instance.ToggleLocalPlayerTeam();
        }

        /// <summary>
        /// Balance teams by moving players (host only)
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public Task<OperationResult> BalanceTeams()
        {
            if (!IsInLobby)
                return Task.FromResult(OperationResult.FailureResult("NotInLobby", "Not in a lobby."));

            if (!IsHost)
                return Task.FromResult(OperationResult.FailureResult("NotHost", "Only the host can balance teams."));

            return PlayerDataManager.Instance.BalanceTeams();
        }

        #endregion

        #region Lobby Data Updates

        /// <summary>
        /// Updates the lobby's game mode (host only)
        /// </summary>
        /// <param name="gameMode">The new game mode</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> UpdateGameMode(string gameMode)
        {
            if (!IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");

            if (!IsHost)
                return OperationResult.FailureResult("NotHost", "Only the host can change the game mode.");

            try
            {
                await Lobbies.Instance.UpdateLobbyAsync(LobbyManager.Instance.LobbyId, new UpdateLobbyOptions
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
        /// Updates the player's name in the lobby and saves to PlayerPrefs
        /// </summary>
        /// <param name="newPlayerName">The new player name</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> UpdatePlayerName(string newPlayerName)
        {
            if (!IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not currently in a lobby.");

            if (string.IsNullOrWhiteSpace(newPlayerName))
                return OperationResult.FailureResult("InvalidName", "Player name cannot be empty.");

            try
            {
                await Lobbies.Instance.UpdatePlayerAsync(
                    LobbyManager.Instance.LobbyId,
                    AuthenticationService.Instance.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newPlayerName) }
                        }
                    });

                PlayerPrefs.SetString("PlayerName", newPlayerName);
                PlayerPrefs.Save();

                // LobbyEvents.InvokePlayerNameUpdated(AuthenticationService.Instance.PlayerId, newPlayerName);
                return OperationResult.SuccessResult("PlayerNameUpdated", $"Player name updated to {newPlayerName}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update player name: {e.Message}");
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Updates the selected map/arena (host only)
        /// </summary>
        /// <param name="mapIndex">The index of the selected map</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> SelectMap(int mapIndex)
        {
            if (!IsInLobby)
                return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

            if (!IsHost)
                return OperationResult.FailureResult("NotHost", "Only the host can change the map.");

            try
            {
                await Lobbies.Instance.UpdateLobbyAsync(
                    LobbyManager.Instance.LobbyId,
                    new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { "MapIndex", new DataObject(DataObject.VisibilityOptions.Public, mapIndex.ToString()) }
                        }
                    });

                LobbyEvents.InvokeArenaSelected(mapIndex.ToString());
                return OperationResult.SuccessResult("MapUpdated", $"Map updated to index {mapIndex}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to update map index: {e.Message}");
                return OperationResult.FailureResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Refreshes the lobby data from a lobby object
        /// </summary>
        /// <param name="lobby">The lobby object to refresh from</param>
        private void RefreshLobbyData(Lobby lobby)
        {
            if (lobby == null)
            {
                ClearLobbyData();
                return;
            }

            lobbyPlayerIds.Clear();
            foreach (var player in lobby.Players)
            {
                PlayerDataManager.Instance.UpdateCache(player);
                lobbyPlayerIds.Add(player.Id);
            }

            lobbyData = new LobbyData();
            lobbyData.Initialize(lobby.Data);

            CheckReadyStatus();
        }

        /// <summary>
        /// Clears all lobby data
        /// </summary>
        private void ClearLobbyData()
        {
            lobbyPlayerIds.Clear();
            PlayerDataManager.Instance.ClearCache();
            lobbyData = null;
        }

        /// <summary>
        /// Checks if all players are ready and fires appropriate events
        /// </summary>
        private void CheckReadyStatus()
        {
            if (!IsInLobby || LobbyManager.Instance.Players.Count == 0) return;

            // Use the extension method to check ready status
            bool allPlayersReady = LobbyManager.Instance.Players.All(p => p.IsReady());

            if (allPlayersReady)
                LobbyEvents.InvokeAllPlayersReady();
            else
                LobbyEvents.InvokeNotAllPlayersReady();
        }

        #endregion
    }
}