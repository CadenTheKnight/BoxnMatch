using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private void Start()
        {
            LobbyEvents.OnLobbyRefreshed += OnLobbyUpdated;
        }

        private void OnDestroy()
        {
            LobbyEvents.OnLobbyRefreshed -= OnLobbyUpdated;
        }

        /// <summary>
        /// Returns list of lobbies based on the provided filters and maximum results.
        /// </summary>
        /// <param name="filters">Optional filters to apply to the lobby query.</param>
        /// <param name="maxResults">>Maximum number of lobbies to return.</param>
        /// <returns>List of lobbies matching the filters.</returns>
        public async Task<List<Lobby>> GetLobbies(List<QueryFilter> filters = null, int maxResults = 20)
        {
            return await LobbyManager.Instance.GetLobbies(filters, maxResults);
        }

        /// <summary>
        /// Returns a list of lobby IDs that the player has joined.
        /// </summary>
        /// <returns>List of lobby IDs.</returns>
        public async Task<List<string>> GetJoinedLobbies()
        {
            return await LobbyManager.Instance.GetJoinedLobbies();
        }

        /// <summary>
        /// Returns the number of players that are ready in the lobby.
        /// Invokes events to notify if the lobby is ready or not.
        /// </summary>
        /// <returns>Number of players ready.</returns>
        public int GetPlayersReady()
        {
            int playersReady = 0;

            foreach (Player player in LobbyManager.Instance.Lobby.Players)
                if (player.Data["Status"].Value == PlayerStatus.Ready.ToString())
                    playersReady++;

            if (playersReady == LobbyManager.Instance.Lobby.MaxPlayers)
                Events.LobbyEvents.InvokeLobbyReady();
            else
                Events.LobbyEvents.InvokeLobbyNotReady(playersReady, LobbyManager.Instance.Lobby.MaxPlayers);

            return playersReady;
        }

        #region Lobby Management
        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">Name of the lobby.</param>
        /// <param name="isPrivate">True if the lobby is private, false if public.</param>
        /// <param name="maxPlayers">Maximum number of players in the lobby.</param>
        public void CreateLobby(string lobbyName, bool isPrivate, int maxPlayers)
        {
            LobbyData lobbyData = new();
            lobbyData.Initialize();

            AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.Ready.ToString();

            LobbyManager.Instance.CreateLobby(lobbyName, isPrivate, maxPlayers, lobbyData.Serialize());
        }

        /// <summary>
        /// Joins a lobby using the lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        public void JoinLobbyByCode(string lobbyCode)
        {
            AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.NotReady.ToString();

            LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
        }

        /// <summary>
        /// Joins the selected lobby using the lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        public void JoinLobbyById(string lobbyId)
        {
            AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.NotReady.ToString();

            LobbyManager.Instance.JoinLobbyById(lobbyId);
        }

        /// <summary>
        /// Rejoins the first lobby in the list of joined lobbies.
        /// </summary>
        public void RejoinLobby(List<string> joinedLobbyIds)
        {
            AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.NotReady.ToString();

            LobbyManager.Instance.RejoinLobby(joinedLobbyIds);
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        public void LeaveLobby()
        {
            LobbyManager.Instance.LeaveLobby();
        }

        /// <summary>
        /// Kicks a player from the lobby.
        /// </summary>
        /// <param name="playerId">The ID of the player to kick.</param>
        public void KickPlayer(string playerId)
        {
            LobbyManager.Instance.KickPlayer(playerId);
        }
        #endregion

        #region Player Management
        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        public void TogglePlayerReady()
        {
            AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value =
                (AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.NotReady.ToString()) ?
                    PlayerStatus.Ready.ToString() : PlayerStatus.NotReady.ToString();

            LobbyManager.Instance.UpdatePlayerData(AuthenticationManager.Instance.LocalPlayer.Id, AuthenticationManager.Instance.LocalPlayer.Data);

            GetPlayersReady();
        }

        /// <summary>
        /// Sets all players to not ready.
        /// </summary>
        public void SetAllPlayersUnready()
        {
            foreach (Player player in LobbyManager.Instance.Lobby.Players) // ????
            {
                if (player.Id == AuthenticationManager.Instance.LocalPlayer.Id) continue;

                player.Data["Status"].Value = PlayerStatus.NotReady.ToString();
                LobbyManager.Instance.UpdatePlayerData(player.Id, player.Data);
            }

            GetPlayersReady();
        }

        /// <summary>
        /// Updates the connection status of the local player.
        /// </summary>
        /// <param name="isConnected">True if the player is connected, false otherwise.</param>
        private void UpdateConnectionStatus(bool isConnected)
        {
            if (!isConnected) AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.Disconnected.ToString();
            LobbyManager.Instance.UpdatePlayerData(AuthenticationManager.Instance.LocalPlayer.Id, AuthenticationManager.Instance.LocalPlayer.Data);
        }
        #endregion

        #region Data Management

        // public void UpdatePlayerData(string playerId, string playerData)
        // {
        //     PlayerData playerData = new();
        //     playerData.Initialize();

        //     LobbyManager.Instance.UpdatePlayerData(playerId, playerData.Serialize());
        // }


        /// <summary>
        /// Sets the selected map for the game.
        /// </summary>
        /// <param name="mapIndex">Index of the  map.</param>
        /// <param name="roundCount">Number of rounds.</param>
        /// <param name="roundTime">Time per round.</param>
        /// <param name="gameMode">Game mode.</param>
        public void UpdateLobbyData(int mapIndex, int roundCount, int roundTime, GameMode gameMode)
        {
            LobbyData lobbyData = new();
            lobbyData.Initialize(mapIndex, roundCount, roundTime, gameMode);

            LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
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
        private void OnLobbyUpdated()
        {
            if (LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id &&
                AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value == PlayerStatus.NotReady.ToString())
            {
                AuthenticationManager.Instance.LocalPlayer.Data["Status"].Value = PlayerStatus.Ready.ToString();
                LobbyManager.Instance.UpdatePlayerData(AuthenticationManager.Instance.LocalPlayer.Id, AuthenticationManager.Instance.LocalPlayer.Data);
            }
        }
    }
    #endregion
}