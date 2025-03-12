using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Controllers.GameplayMenu;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// Acts as a bridge between the framework's LobbyManager and the game's specific needs.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        #region Fields and Properties

        private LobbyData lobbyData;
        private LobbyPlayerData localLobbyPlayerData;
        private List<LobbyPlayerData> lobbyPlayersData = new();
        private LoadingPanelController loadingPanelController;
        [SerializeField] private MapSelectionData mapSelectionData;

        private bool inGame = false;
        public int RoundCount => lobbyData.RoundCount;

        #endregion

        #region Unity Lifecycle Methods

        private void OnEnable()
        {
            Framework.Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            Framework.Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        #endregion

        #region Lobby Creation and Joining

        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or not.</param>
        /// <param name="roundCount">The number of rounds to play in the game.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, int roundCount)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));

            lobbyData = new();
            lobbyData.Initialize(lobbyName, maxPlayers, isPrivate, roundCount);

            return await LobbyManager.Instance.CreateLobby(lobbyData.Serialize(), playerData.Serialize());
        }

        /// <summary>
        /// Joins a lobby using the lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
            return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode, playerData.Serialize());
        }

        /// <summary>
        /// Joins the selected lobby using the lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
            return await LobbyManager.Instance.JoinLobbyById(lobbyId, playerData.Serialize());
        }

        #endregion

        #region Player State Management

        /// <summary>
        /// Checks if the given player is ready.
        /// </summary>
        /// <param name="playerId">The ID of the player to check.</param>
        /// <returns>True if the player is ready, false otherwise.</returns>
        public bool IsPlayerReady(string playerId)
        {
            return lobbyPlayersData.FirstOrDefault(player => player.PlayerId == playerId)?.IsReady ?? false;
        }

        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> TogglePlayerReady()
        {
            localLobbyPlayerData.IsReady = !localLobbyPlayerData.IsReady;
            return await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());
        }

        /// <summary>
        /// Sets all players to not ready.
        /// </summary>
        public async Task<OperationResult> SetAllPlayersUnready()
        {
            foreach (LobbyPlayerData playerData in lobbyPlayersData)
                playerData.IsReady = false;

            return await LobbyManager.Instance.UpdateAllPlayerData(lobbyPlayersData.Select(player => player.Serialize()).ToList());
        }

        /// <summary>
        /// Gets all players in the lobby.
        /// </summary>
        /// <returns>List of player data.</returns>
        public List<LobbyPlayerData> GetPlayers()
        {
            return lobbyPlayersData;
        }

        #endregion

        #region Map and Game Settings

        /// <summary>
        /// Gets the current map index.
        /// </summary>
        /// <returns>Current map index.</returns>
        public int GetMapIndex()
        {
            return lobbyData.MapIndex;
        }

        /// <summary>
        /// Sets the selected map for the game.
        /// </summary>
        /// <param name="mapIndex">Index of the map.</param>
        /// <returns>Operation result</returns>
        public async Task<OperationResult> SetSelectedMap(int mapIndex)
        {
            lobbyData.MapIndex = mapIndex;

            return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        }

        #endregion

        #region Game Flow

        /// <summary>
        /// Starts the game.
        /// </summary>
        public async Task StartGame()
        {
            SceneManager.LoadSceneAsync("Loading");
            InitializeLoadingScreen("Creating relay server...");

            string relayJoinCode = await RelayManager.Instance.CreateRelay(LobbyManager.Instance.MaxPlayers);
            inGame = true;
            loadingPanelController.SetStatus("Created relay server...");

            lobbyData.RelayJoinCode = relayJoinCode;
            await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize(), allocationId, connectionData);
            loadingPanelController.SetStatus("Waiting for players...");

            await Task.Delay(2000);
            LoadGameMap();
        }

        /// <summary>
        /// Joins an existing relay server.
        /// </summary>
        /// <param name="relayJoinCode">Relay code to join.</param>
        private async Task JoinRelayServer(string relayJoinCode)
        {
            SceneManager.LoadSceneAsync("Loading");
            InitializeLoadingScreen("Joining relay server...");

            inGame = true;
            await RelayManager.Instance.JoinRelay(relayJoinCode);
            loadingPanelController.SetStatus("Joined game...");

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize(), allocationId, connectionData);
            loadingPanelController.SetStatus("Waiting for players...");
        }

        private void InitializeLoadingScreen(string startMessage)
        {
            MapInfo mapInfo = mapSelectionData.Maps[lobbyData.MapIndex];
            loadingPanelController.StartLoading(mapInfo.MapThumbnail, mapInfo.MapName, startMessage);
        }

        /// <summary>
        /// Transitions from loading screen to the actual game map.
        /// </summary>
        public void LoadGameMap()
        {
            SceneManager.LoadSceneAsync(mapSelectionData.Maps[lobbyData.MapIndex].MapSceneName);
        }

        /// <summary>
        /// Returns to the lobby after a game and sets all players to unready.
        /// </summary>
        public async Task ReturnToLobby()
        {
            inGame = false;

            SceneManager.LoadSceneAsync("Loading");
            InitializeLoadingScreen("Returning to lobby...");

            lobbyData.RelayJoinCode = default;
            await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

            await SetAllPlayersUnready();

            await Task.Delay(2000);

            SceneManager.LoadSceneAsync("Lobby");
        }

        #endregion

        #region Event Handlers

        private async void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playersData = LobbyManager.Instance.GetPlayersData();
            lobbyPlayersData.Clear();

            int playersReady = 0;

            foreach (Dictionary<string, PlayerDataObject> playerData in playersData)
            {
                LobbyPlayerData lobbyPlayerData = new();
                lobbyPlayerData.Initialize(playerData);

                if (lobbyPlayerData.IsReady)
                    playersReady++;

                if (lobbyPlayerData.PlayerId == AuthenticationManager.Instance.PlayerId)
                    localLobbyPlayerData = lobbyPlayerData;

                lobbyPlayersData.Add(lobbyPlayerData);
            }

            lobbyData = new();
            lobbyData.Initialize(lobby.Data);

            LobbyEvents.InvokeLobbyUpdated();

            if (playersReady == LobbyManager.Instance.MaxPlayers)
                LobbyEvents.InvokeAllPlayersReady();
            else
                LobbyEvents.InvokeNotAllPlayersReady(playersReady, LobbyManager.Instance.MaxPlayers);

            if (lobbyData.RelayJoinCode != default && !inGame)
                await JoinRelayServer(lobbyData.RelayJoinCode);
        }

        #endregion
    }
}