using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;

using UnityEngine;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        /// <summary>
        /// Returns the number of players that are ready in the lobby.
        /// Invokes events to notify if the lobby is ready or not.
        /// </summary>
        /// <returns>Number of players ready.</returns>
        public int GetPlayersReady()
        {
            int playersReady = 0;

            foreach (Player player in LobbyManager.Instance.Lobby.Players)
            {

                Debug.Log($"Player {player.Id} status: {player.Data["Status"].Value}");
                if (player.Data["Status"].Value == PlayerStatus.Ready.ToString())
                    playersReady++;
            }

            if (playersReady == LobbyManager.Instance.Lobby.MaxPlayers)
                Events.LobbyEvents.InvokeLobbyReady();
            else
                Events.LobbyEvents.InvokeLobbyNotReady(playersReady, LobbyManager.Instance.Lobby.MaxPlayers);

            return playersReady;
        }

        #region Player Management
        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <param name="player">The player to toggle.</param>
        /// <param name="setReady">True to set the player as ready.</param>
        /// <param name="setUnready">True to set the player as not ready.</param>
        public async Task TogglePlayerReady(Player player, bool setReady = false, bool setUnready = false)
        {
            if (setReady)
                player.Data["Status"].Value = ((int)PlayerStatus.Ready).ToString();
            else if (setUnready)
                player.Data["Status"].Value = ((int)PlayerStatus.NotReady).ToString();
            else
                player.Data["Status"].Value = player.Data["Status"].Value == ((int)PlayerStatus.Ready).ToString()
                ? ((int)PlayerStatus.NotReady).ToString() : ((int)PlayerStatus.Ready).ToString();

            await LobbyManager.Instance.UpdatePlayerData(player.Id, player.Data);
        }

        /// <summary>
        /// Sets all players to not ready.
        /// </summary>
        public async Task SetAllPlayersUnready()
        {
            foreach (Player player in LobbyManager.Instance.Lobby.Players)
                await TogglePlayerReady(player, setUnready: true);
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

    }
}