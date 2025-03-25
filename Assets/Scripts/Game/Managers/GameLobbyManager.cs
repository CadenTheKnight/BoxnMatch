using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private LobbyData lobbyData;
        private LobbyPlayerData localLobbyPlayerData;
        private readonly List<LobbyPlayerData> lobbyPlayersData = new();

        private void Start()
        {
            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDestroy()
        {
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        private void OnApplicationQuit()
        {
            UpdateConnectionStatus(false);
        }

        public async Task<bool> HasActiveLobbies()
        {
            return await LobbyManager.Instance.HasActiveLobbies();
        }

        #region Lobby Creation and Joining
        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">Name of the lobby.</param>
        /// <param name="maxPlayers">Maximum number of players in the lobby.</param>
        /// <param name="isPrivate">True if the lobby is private, false if public.</param>
        /// <param name="roundCount">Number of rounds to play in the game.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, int roundCount)
        {
            try
            {
                LobbyPlayerData playerData = new();
                playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));

                lobbyData = new();
                lobbyData.Initialize(lobbyName, maxPlayers, isPrivate, roundCount);

                return await LobbyManager.Instance.CreateLobby(lobbyData.Serialize(), playerData.Serialize());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("CreateLobby", $"Failed to create lobby: {ex.Message}");
            }
        }

        /// <summary>
        /// Joins a lobby using the lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                LobbyPlayerData playerData = new();
                playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
                return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode, playerData.Serialize());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("JoinLobby", $"Failed to join lobby: {ex.Message}");
            }
        }

        /// <summary>
        /// Joins the selected lobby using the lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            try
            {
                LobbyPlayerData playerData = new();
                playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
                return await LobbyManager.Instance.JoinLobbyById(lobbyId, playerData.Serialize());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("JoinLobby", $"Failed to join lobby: {ex.Message}");
            }
        }

        /// <summary>
        /// Rejoins the lobby using the lobby ID.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> RejoinLobby()
        {
            OperationResult result = await LobbyManager.Instance.RejoinLobby();

            if (result.Status == ResultStatus.Success)
                UpdateConnectionStatus(true);

            return result;
        }
        #endregion

        #region Player Management
        /// <summary>
        /// Gets all players in the lobby.
        /// </summary>
        /// <returns>List of player data.</returns>
        public List<LobbyPlayerData> GetPlayers()
        {
            return lobbyPlayersData;
        }

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
            try
            {
                localLobbyPlayerData.IsReady = !localLobbyPlayerData.IsReady;
                return await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("ToggleReady", $"Failed to toggle ready status: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets all players to not ready.
        /// </summary>
        public async Task<OperationResult> SetAllPlayersUnready()
        {
            try
            {
                foreach (LobbyPlayerData playerData in lobbyPlayersData)
                    playerData.IsReady = false;

                return await LobbyManager.Instance.UpdateAllPlayerData(lobbyPlayersData.Select(player => player.Serialize()).ToList());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("SetAllUnready", $"Failed to set all players unready: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the connection status of the local player.
        /// </summary>
        /// <param name="isConnected">True if the player is connected, false otherwise.</param>
        private async void UpdateConnectionStatus(bool isConnected)
        {
            try
            {
                localLobbyPlayerData.IsConnected = isConnected;
                await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error updating connection status: {ex.Message}");
            }
        }
        #endregion

        #region Game Settings Management

        /// <summary>
        /// Gets the current map index.
        /// </summary>
        /// <returns>Current map index.</returns>
        public int GetMapIndex()
        {
            return lobbyData?.MapIndex ?? 0;
        }

        /// <summary>
        /// Sets the selected map for the game.
        /// </summary>
        /// <param name="mapIndex">Index of the map.</param>
        /// <returns>Operation result</returns>
        public async Task<OperationResult> SetSelectedMap(int mapIndex)
        {
            try
            {
                lobbyData.MapIndex = mapIndex;
                return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult("SetSelectedMap", $"Failed to set selected map: {ex.Message}");
            }
        }
        #endregion

        #region Game Flow

        /// <summary>
        /// Handles the entire loading process - transitions to loading scene, initializes the relay, and then loads the game map
        /// </summary>
        // public async Task HandleGameLoading(string mapName, Sprite mapThumbnail, string mapSceneName)
        // {
        //     try
        //     {
        //         // Step 1: Load the loading scene locally
        //         Debug.Log($"Loading loading screen for {mapName}");
        //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
        //         while (!loadOperation.isDone)
        //             await Task.Yield();

        //         // Step 2: Set up the loading UI
        //         loadingPanelController = FindObjectOfType<LoadingPanelController>();
        //         if (loadingPanelController == null)
        //         {
        //             Debug.LogError("LoadingPanelController not found in Loading scene!");
        //             return;
        //         }

        //         loadingPanelController.StartLoading(mapName, mapThumbnail,
        //             LobbyManager.Instance.IsLobbyHost ? "Setting up relay server..." : "Waiting for host...");

        //         // Step 3: Host sets up relay and waits for clients to connect
        //         if (LobbyManager.Instance.IsLobbyHost)
        //         {
        //             // Create relay
        //             string relayJoinCode = await RelayManager.Instance.CreateRelay(LobbyManager.Instance.MaxPlayers);
        //             loadingPanelController.SetStatus("Created relay server...");

        //             // Update lobby data for clients to join
        //             lobbyData.InGame = true;
        //             lobbyData.GameStarted = false;
        //             lobbyData.RelayJoinCode = relayJoinCode;
        //             await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

        //             // Update host's player data with connection info
        //             string allocationId = RelayManager.Instance.GetAllocationId();
        //             string connectionData = RelayManager.Instance.GetConnectionData();
        //             localLobbyPlayerData.InGame = true;
        //             await LobbyManager.Instance.UpdatePlayerData(
        //                 localLobbyPlayerData.PlayerId,
        //                 localLobbyPlayerData.Serialize(),
        //                 allocationId,
        //                 connectionData);

        //             // Initialize network BEFORE waiting for players
        //             SetupNetworking();
        //             await Task.Delay(1000); // Give network time to initialize

        //             // Wait for clients to connect to relay
        //             loadingPanelController.SetStatus("Waiting for players to join relay...");
        //             int connectedPlayersNeeded = LobbyManager.Instance.MaxPlayers;
        //             int waitAttempts = 0;
        //             int maxWaitAttempts = 40; // 20 seconds total wait time

        //             while (waitAttempts < maxWaitAttempts)
        //             {
        //                 // Count players by tracking in-game flags in player data
        //                 int connectedPlayers = lobbyPlayersData.Count(p => p.InGame);

        //                 // Update UI
        //                 loadingPanelController.SetStatus($"Waiting for players to join relay... ({connectedPlayers}/{connectedPlayersNeeded})");

        //                 // Check if all expected players are connected
        //                 if (connectedPlayers >= connectedPlayersNeeded)
        //                 {
        //                     Debug.Log($"All {connectedPlayers}/{connectedPlayersNeeded} players connected!");
        //                     break;
        //                 }

        //                 await Task.Delay(500);
        //                 waitAttempts++;
        //             }

        //             // Signal to clients that game is starting
        //             loadingPanelController.SetStatus("Starting game...");
        //             await Task.Delay(1000);

        //             lobbyData.GameStarted = true;
        //             await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

        //             // Load game map with network scene manager
        //             LoadGameMap(mapSceneName);
        //         }
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error in HandleGameLoading: {ex.Message}");
        //         await ReturnToLobby($"Error loading game: {ex.Message}");
        //     }
        // }

        // private void SetupNetworking()
        // {
        //     try
        //     {
        //         if (NetworkManager.Singleton == null)
        //         {
        //             Debug.LogError("NetworkManager.Singleton is null!");
        //             return;
        //         }

        //         if (NetworkManager.Singleton.IsListening)
        //         {
        //             Debug.Log("NetworkManager is already listening, skipping setup");
        //             return;
        //         }

        //         // Configure NetworkManager
        //         NetworkManager.Singleton.NetworkConfig.EnableSceneManagement = true;

        //         if (RelayManager.Instance.IsHost)
        //         {
        //             Debug.Log("Setting up host relay connection");
        //             (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) =
        //                 RelayManager.Instance.GetHostConnectionInfo();

        //             var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //             transport.SetHostRelayData(
        //                 dtlsAddress,
        //                 (ushort)dtlsPort,
        //                 AllocationId,
        //                 Key,
        //                 ConnectionData);

        //             bool success = NetworkManager.Singleton.StartHost();
        //             Debug.Log($"Started host: {success}");
        //         }
        //         else
        //         {
        //             Debug.Log("Setting up client relay connection");
        //             (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) =
        //                 RelayManager.Instance.GetClientConnectionInfo();

        //             var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //             transport.SetClientRelayData(
        //                 dtlsAddress,
        //                 (ushort)dtlsPort,
        //                 AllocationId,
        //                 Key,
        //                 ConnectionData,
        //                 HostConnectionData);

        //             bool success = NetworkManager.Singleton.StartClient();
        //             Debug.Log($"Started client: {success}");
        //         }
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error setting up networking: {ex.Message}");
        //     }
        // }

        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="mapName">Name of the map.</param>
        /// <param name="mapThumbnail">Thumbnail of the map.</param>
        /// <param name="mapSceneName">Scene name of the map.</param>
        // public async Task StartGame(string mapName, Sprite mapThumbnail, string mapSceneName)
        // {
        //     try
        //     {
        //         Debug.Log($"Starting game with map: {mapName}, scene: {mapSceneName}");
        //         inGame = true;
        //         await HandleGameLoading(mapName, mapThumbnail, mapSceneName);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error starting game: {ex.Message}");
        //         inGame = false;
        //     }
        // }

        /// <summary>
        /// Joins an existing relay server.
        /// </summary>
        // private async Task JoinRelayServer(string relayJoinCode)
        // {
        //     try
        //     {
        //         Debug.Log($"Client joining relay server with code: {relayJoinCode}");

        //         // Load loading scene
        //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
        //         while (!loadOperation.isDone)
        //             await Task.Yield();

        //         loadingPanelController = FindObjectOfType<LoadingPanelController>();
        //         if (loadingPanelController == null)
        //         {
        //             Debug.LogError("LoadingPanelController not found!");
        //             return;
        //         }

        //         MapInfo mapInfo = MapSelectionManager.Instance.GetMapInfo(lobbyData.MapIndex);

        //         string mapName = mapInfo.MapName;
        //         Sprite mapThumbnail = mapInfo.MapThumbnail;

        //         loadingPanelController.StartLoading(mapName, mapThumbnail, "Joining relay server...");

        //         // Set in-game state to prevent multiple join attempts
        //         inGame = true;

        //         // Initialize Unity Transport with relay info
        //         await RelayManager.Instance.JoinRelay(relayJoinCode);
        //         loadingPanelController.SetStatus("Connected to relay server...");

        //         // Setup networking on client
        //         SetupNetworking();

        //         // Update player data with connection info
        //         string allocationId = RelayManager.Instance.GetAllocationId();
        //         string connectionData = RelayManager.Instance.GetConnectionData();
        //         localLobbyPlayerData.InGame = true;
        //         await LobbyManager.Instance.UpdatePlayerData(
        //             localLobbyPlayerData.PlayerId,
        //             localLobbyPlayerData.Serialize(),
        //             allocationId,
        //             connectionData);

        //         loadingPanelController.SetStatus("Waiting for host to start game...");
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error joining relay server: {ex.Message}");
        //         inGame = false;
        //         await ReturnToLobby("Failed to join game");
        //     }
        // }

        /// <summary>
        /// Loads the game map using NetworkSceneManager.
        /// </summary>
        // public void LoadGameMap(string mapSceneName)
        // {
        //     try
        //     {
        //         Debug.Log($"Loading game map: {mapSceneName}");

        //         if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        //         {
        //             Debug.Log($"Host loading scene via NetworkSceneManager: {mapSceneName}");
        //             NetworkManager.Singleton.SceneManager.LoadScene(mapSceneName, LoadSceneMode.Single);
        //         }
        //         else
        //         {
        //             Debug.Log("Client waiting for host to load scene via NetworkSceneManager");
        //         }
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error loading game map: {ex.Message}");
        //     }
        // }

        /// <summary>
        /// Returns to the lobby after a game and sets all players to unready.
        /// </summary>
        // public async Task ReturnToLobby(string message = "Returning to lobby...")
        // {
        //     try
        //     {
        //         inGame = false;

        //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
        //         while (!loadOperation.isDone)
        //             await Task.Yield();

        //         loadingPanelController = FindObjectOfType<LoadingPanelController>();

        //         if (LobbyManager.Instance.IsLobbyHost)
        //         {
        //             if (lobbyData != null)
        //             {
        //                 lobbyData.RelayJoinCode = default;
        //                 lobbyData.GameStarted = false;
        //                 await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        //             }

        //             await SetAllPlayersUnready();
        //         }

        //         await Task.Delay(1500);

        //         SceneManager.LoadSceneAsync("Lobby");
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Error returning to lobby: {ex.Message}");

        //         await Task.Delay(1000);
        //         SceneManager.LoadSceneAsync("Lobby");
        //     }
        // }

        #endregion

        #region Event Handlers
        private void OnLobbyUpdated(Lobby lobby)
        {
            try
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

                Events.LobbyEvents.InvokeLobbyUpdated();

                if (playersReady == LobbyManager.Instance.MaxPlayers)
                    Events.LobbyEvents.InvokeAllPlayersReady();
                else
                    Events.LobbyEvents.InvokeNotAllPlayersReady(playersReady, LobbyManager.Instance.MaxPlayers);

                // // Handle relay join code - only if not already in game
                // if (!LobbyManager.Instance.IsLobbyHost && lobbyData.RelayJoinCode != default && !inGame)
                // {
                //     await JoinRelayServer(lobbyData.RelayJoinCode);
                // }

                // if (!LobbyManager.Instance.IsLobbyHost && inGame && lobbyData.GameStarted)
                // {
                //     if (loadingPanelController != null)
                //     {
                //         loadingPanelController.SetStatus("Game is starting...");
                //         Debug.Log("Client waiting for NetworkManager to load scene");
                //     }
                // }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in OnLobbyUpdated: {ex.Message}");
            }
        }
        #endregion
    }
}