using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using FrameworkEvents = Assets.Scripts.Framework.Events;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        //         private LobbyData lobbyData;
        //         private readonly List<string> lobbyPlayerIds = new();

        //         public bool IsHost => LobbyManager.Instance != null && LobbyManager.Instance.IsLobbyHost;
        //         public string LobbyName => LobbyManager.Instance != null ? LobbyManager.Instance.LobbyName : null;
        //         public string LobbyCode => LobbyManager.Instance != null ? LobbyManager.Instance.LobbyCode : null;
        //         public int MaxPlayers => PlayerDataManager.PlayerConstants.MAX_PLAYERS;
        //         public int CurrentPlayerCount => lobbyPlayerIds?.Count ?? 0;
        //         public LobbyData CurrentLobbyData => lobbyData;
        //         public IReadOnlyList<LobbyPlayerData> Players
        //         {
        //             get
        //             {
        //                 List<LobbyPlayerData> players = new();
        //                 foreach (var playerId in lobbyPlayerIds)
        //                 {
        //                     var player = PlayerDataManager.Instance.GetPlayerData(playerId);
        //                     if (player != null)
        //                         players.Add(player);
        //                 }
        //                 return players;
        //             }
        //         }
        //         public LobbyPlayerData LocalPlayer => PlayerDataManager.Instance.GetLocalPlayerData();

        //         #region Unity Lifecycle

        //         /// <summary>
        //         /// Subscribes to framework events when enabled.
        //         /// </summary>
        //         private void OnEnable()
        //         {
        //             FrameworkEvents.LobbyEvents.OnLobbyCreated += OnLobbyCreated;
        //             FrameworkEvents.LobbyEvents.OnLobbyJoined += OnLobbyJoined;
        //             FrameworkEvents.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        //             FrameworkEvents.LobbyEvents.OnLobbyLeft += OnLobbyLeft;
        //             FrameworkEvents.LobbyEvents.OnPlayerJoined += OnPlayerJoined;
        //             FrameworkEvents.LobbyEvents.OnPlayerLeft += OnPlayerLeft;
        //             FrameworkEvents.LobbyEvents.OnPlayerKicked += OnPlayerKicked;
        //             FrameworkEvents.LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
        //             FrameworkEvents.LobbyEvents.OnLobbyDataChanged += OnLobbyDataChanged;
        //         }

        //         /// <summary>
        //         /// Unsubscribes from framework events when disabled.
        //         /// </summary>
        //         private void OnDisable()
        //         {
        //             FrameworkEvents.LobbyEvents.OnLobbyCreated -= OnLobbyCreated;
        //             FrameworkEvents.LobbyEvents.OnLobbyJoined -= OnLobbyJoined;
        //             FrameworkEvents.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        //             FrameworkEvents.LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
        //             FrameworkEvents.LobbyEvents.OnPlayerJoined -= OnPlayerJoined;
        //             FrameworkEvents.LobbyEvents.OnPlayerLeft -= OnPlayerLeft;
        //             FrameworkEvents.LobbyEvents.OnPlayerKicked -= OnPlayerKicked;
        //             FrameworkEvents.LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
        //             FrameworkEvents.LobbyEvents.OnLobbyDataChanged -= OnLobbyDataChanged;
        //         }

        //         #endregion

        //         #region Event Handlers

        //         private void OnLobbyCreated(Lobby lobby)
        //         {
        //             RefreshLobbyData(lobby);
        //         }

        //         private void OnLobbyJoined(Lobby lobby)
        //         {
        //             RefreshLobbyData(lobby);
        //         }

        //         private void OnLobbyLeft(Lobby lobby)
        //         {
        //             ClearLobbyData();
        //         }

        //         private void OnLobbyUpdated(Lobby lobby)
        //         {
        //             RefreshLobbyData(lobby);
        //         }

        //         private void OnPlayerJoined(string lobbyId, Player player)
        //         {
        //             PlayerDataManager.Instance.UpdateCache(player.Id, player.Data);
        //             lobbyPlayerIds.Add(player.Id);

        //             Debug.Log($"Player {PlayerDataManager.Instance.GetPlayerData(player.Id)?.PlayerName} joined the lobby");
        //         }

        //         private void OnPlayerLeft(string playerId)
        //         {
        //             var player = PlayerDataManager.Instance.GetPlayerData(playerId);
        //             if (player != null)
        //             {
        //                 lobbyPlayerIds.Remove(playerId);
        //                 Debug.Log($"Player {player.PlayerName} left the lobby");
        //             }

        //             CheckReadyStatus();
        //         }

        //         private void OnPlayerKicked(string playerId)
        //         {
        //             if (playerId == AuthenticationService.Instance.PlayerId)
        //             {
        //                 ClearLobbyData();
        //                 Debug.Log("You were kicked from the lobby");
        //             }
        //             else
        //             {
        //                 OnPlayerLeft(playerId);
        //                 Debug.Log($"Player {playerId} was kicked from the lobby");
        //             }
        //         }

        //         private void OnPlayerDataChanged(string playerId, Dictionary<string, PlayerDataObject> data)
        //         {
        //             var previousPlayerData = PlayerDataManager.Instance.GetPlayerData(playerId);
        //             if (previousPlayerData == null)
        //             {
        //                 Debug.LogWarning($"Received data for unknown player: {playerId}");
        //                 return;
        //             }

        //             var previousCharacter = previousPlayerData.CharacterId;
        //             var previousColor = previousPlayerData.ColorId;
        //             var previousReady = previousPlayerData.IsReady;
        //             var previousTeam = previousPlayerData.TeamId;

        //             PlayerDataManager.Instance.UpdateCache(playerId, data);
        //             var newPlayerData = PlayerDataManager.Instance.GetPlayerData(playerId);

        //             if (newPlayerData.CharacterId != previousCharacter)
        //                 LobbyEvents.InvokeCharacterSelected(playerId, newPlayerData.CharacterId);

        //             if (newPlayerData.ColorId != previousColor)
        //                 LobbyEvents.InvokePlayerColorSelected(playerId, newPlayerData.ColorId);

        //             if (newPlayerData.IsReady != previousReady)
        //             {
        //                 LobbyEvents.InvokePlayerReadyChanged(playerId, newPlayerData.IsReady);
        //                 CheckReadyStatus();
        //             }

        //             if (newPlayerData.TeamId != previousTeam)
        //                 LobbyEvents.InvokeTeamSelected(playerId, newPlayerData.TeamId);
        //         }

        //         private void OnLobbyDataChanged(Lobby lobby, Dictionary<string, DataObject> data)
        //         {
        //             var previousMapIndex = lobbyData?.MapIndex ?? 0;

        //             lobbyData = new LobbyData();
        //             lobbyData.Initialize(data);

        //             if (lobbyData.MapIndex != previousMapIndex)
        //                 LobbyEvents.InvokeArenaSelected(lobbyData.MapIndex.ToString());

        //             LobbyEvents.InvokeMatchSettingsUpdated(lobbyData.Serialize());
        //         }

        //         #endregion

        //         #region Event Handlers

        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers = 4) // hard-coded maxPlayers
        {
            return await LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers);
        }

        /// <summary>
        /// Joins a lobby using a lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
        }

        /// <summary>
        /// Joins a lobby using a lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            return await LobbyManager.Instance.JoinLobbyById(lobbyId);
        }

        /// <summary>
        /// Leaves the current lobby
        /// </summary>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> LeaveLobby()
        {
            return await LobbyManager.Instance.LeaveLobby();
        }

        //         /// <summary>
        //         /// Kicks a player from the lobby (host only)
        //         /// </summary>
        //         /// <param name="playerId">The ID of the player to kick</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> KickPlayer(string playerId)
        //         {
        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can kick players.");

        //             return await LobbyManager.Instance.KickPlayer(playerId);
        //         }

        /// <summary>
        /// Refreshes the list of active lobbies.
        /// </summary>
        /// <returns>List of active lobbies.</returns>
        public async Task<OperationResult> RefreshLobbyList()
        {
            return await LobbyManager.Instance.GetLobbies();
        }

        //         #endregion

        //         #region Public Methods - Player Actions

        //         /// <summary>
        //         /// Sets the local player's ready status
        //         /// </summary>
        //         /// <param name="isReady">Whether the player is ready</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SetPlayerReady(bool isReady)
        //         {
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             return await PlayerDataManager.Instance.SetPlayerReady(
        //                 AuthenticationService.Instance.PlayerId,
        //                 isReady
        //             );
        //         }

        //         /// <summary>
        //         /// Sets the local player's character selection
        //         /// </summary>
        //         /// <param name="characterId">The ID of the selected character</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SelectCharacter(string characterId)
        //         {
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             return await PlayerDataManager.Instance.SelectCharacter(
        //                 AuthenticationService.Instance.PlayerId,
        //                 characterId
        //             );
        //         }

        //         /// <summary>
        //         /// Sets the local player's team selection
        //         /// </summary>
        //         /// <param name="teamId">The ID of the selected team (0 or 1)</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SelectTeam(int teamId)
        //         {
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             return await LobbyTeamManager.Instance.JoinTeam(teamId);
        //         }

        //         /// <summary>
        //         /// Sends a chat message in the lobby
        //         /// </summary>
        //         /// <param name="message">The message to send</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SendChatMessage(string message)
        //         {
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             return await LobbyChatManager.Instance.SendPlayerMessage(
        //                 localLobbyPlayerData.PlayerId,
        //                 localLobbyPlayerData.PlayerName,
        //                 message
        //             );
        //         }

        //         #endregion

        //         #region Public Methods - Lobby Features

        //         /// <summary>
        //         /// Automatically balances teams by moving players to ensure teams have equal numbers
        //         /// </summary>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> BalanceTeams()
        //         {
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can balance teams.");

        //             return await LobbyTeamManager.Instance.BalanceTeams();
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
        //             if (!IsInLobby)
        //                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

        //             return LobbyChatManager.Instance.AnnounceMatchResults(winningTeam, teamAScore, teamBScore);
        //         }


        //         #endregion

        //         #region Public Methods - Lobby Settings (Host Only)

        //         /// <summary>
        //         /// Updates the selected map/arena (host only)
        //         /// </summary>
        //         /// <param name="mapIndex">The index of the selected map</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SelectMap(int mapIndex)
        //         {
        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can change the map.");

        //             lobbyData.MapIndex = mapIndex;
        //             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        //         }

        //         /// <summary>
        //         /// Updates the round count setting (host only)
        //         /// </summary>
        //         /// <param name="roundCount">The number of rounds per game</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SetRoundCount(int roundCount)
        //         {
        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can change match settings.");

        //             lobbyData.RoundCount = Mathf.Clamp(roundCount, 1, 99);
        //             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        //         }

        //         /// <summary>
        //         /// Updates the match time setting (host only)
        //         /// </summary>
        //         /// <param name="minutes">The match time in minutes</param>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> SetMatchTime(int minutes)
        //         {
        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can change match settings.");

        //             lobbyData.MatchTimeMinutes = Mathf.Clamp(minutes, 1, 60);
        //             return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        //         }

        //         /// <summary>
        //         /// Starts the match when all players are ready (host only)
        //         /// </summary>
        //         /// <returns>Operation result indicating success or failure</returns>
        //         public async Task<OperationResult> StartMatch()
        //         {
        //             if (!IsHost)
        //                 return OperationResult.FailureResult("NotHost", "Only the host can start the match.");

        //             if (Players.Any(p => !p.IsReady))
        //                 return OperationResult.FailureResult("NotAllReady", "Not all players are ready.");

        //             LobbyEvents.InvokeMatchStarting(5);
        //             await Task.Delay(5000);
        //             LobbyEvents.InvokeMatchStarted();

        //             return OperationResult.SuccessResult("MatchStarted", "Match started successfully.");
        //         }

        //         #endregion

        //         #region Helper Methods

        //         /// <summary>
        //         /// Refreshes the lobby data from a lobby object
        //         /// </summary>
        //         /// <param name="lobby">The lobby object to refresh from</param>
        //         private void RefreshLobbyData(Lobby lobby)
        //         {
        //             if (lobby == null)
        //             {
        //                 ClearLobbyData();
        //                 return;
        //             }

        //             lobbyPlayerIds.Clear();
        //             foreach (var player in lobby.Players)
        //             {
        //                 PlayerDataManager.Instance.UpdateCache(player.Id, player.Data);
        //                 lobbyPlayerIds.Add(player.Id);
        //             }

        //             lobbyData = new LobbyData();
        //             lobbyData.Initialize(lobby.Data);

        //             CheckReadyStatus();
        //         }

        //         /// <summary>
        //         /// Clears all lobby data
        //         /// </summary>
        //         private void ClearLobbyData()
        //         {
        //             lobbyPlayerIds.Clear();
        //             PlayerDataManager.Instance.ClearCache();
        //             lobbyData = null;
        //         }

        //         /// <summary>
        //         /// Checks if all players are ready and fires appropriate events
        //         /// </summary>
        //         private void CheckReadyStatus()
        //         {
        //             if (lobbyPlayerIds.Count == 0) return;

        //             bool allPlayersReady = Players.All(p => p.IsReady);

        //             if (allPlayersReady)
        //                 LobbyEvents.InvokeAllPlayersReady();
        //             else
        //                 LobbyEvents.InvokeNotAllPlayersReady();
        //         }

        //         #endregion
    }
}