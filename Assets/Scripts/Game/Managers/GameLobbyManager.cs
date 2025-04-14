using System;
using UnityEngine;
using System.Collections;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Framework.Types;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = true;

        public Lobby Lobby { get; private set; } = null;
        private Coroutine _heartbeatCoroutine = null;
        private ILobbyEvents lobbyEvents = null;
        private bool leftVolunatarily = false;

        public async Task Initialize(Lobby lobby)
        {
            if (Lobby != null) await Cleanup();

            Lobby = lobby;
            if (AuthenticationService.Instance.PlayerId == Lobby.HostId) _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(Lobby.Id, 6f));
            await SubscribeToLobbyEvents();

            if (showDebugMessages) Debug.Log($"GameLobbyManager initialized: {Lobby.Name} (ID: {Lobby.Id})");
        }

        public async Task Cleanup()
        {
            if (_heartbeatCoroutine != null)
            {
                StopCoroutine(_heartbeatCoroutine);
                _heartbeatCoroutine = null;
            }
            await UnsubscribeFromLobbyEvents();
            Lobby = null;
            if (showDebugMessages) Debug.Log($"GameLobbyManager cleaned up");
        }

        /// <summary>
        /// Handles the lobby heartbeat, sending regular updates to the lobby service.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to send heartbeats to.</param>
        /// <param name="waitTimeSeconds">The time to wait between heartbeats.</param>
        /// <returns>Coroutine for the heartbeat process.</returns>
        private IEnumerator HeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {
            if (showDebugMessages) Debug.Log($"Starting heartbeat coroutine for lobby {lobbyId} every {waitTimeSeconds} seconds");
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        /// <summary>
        /// Returns the current player in the lobby with the given id.
        /// </summary>
        /// <param name="playerId">The id of the player to find.</param>
        /// <returns>The player object if found, otherwise null.</returns>
        public Player GetPlayerById(string playerId)
        {
            return Lobby.Players.Find(player => player.Id == playerId);
        }

        /// <summary>
        /// Gets the number of players that are ready in the lobby.
        /// </summary>
        /// <returns>The number of players that are ready.</returns>
        public int GetPlayersReady()
        {
            int playersReady = 0;

            foreach (Player player in Lobby.Players)
                if ((PlayerStatus)int.Parse(player.Data["Status"].Value) == PlayerStatus.Ready)
                    playersReady++;

            if (showDebugMessages) Debug.Log($"Total players ready: {playersReady} / {Lobby.MaxPlayers}");
            return playersReady;
        }

        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <param name="player">The player to toggle.</param>
        /// <param name="setReady">True to set the player as ready.</param>
        /// <param name="setUnready">True to set the player as not ready.</param>
        /// <returns>An OperationResult.</returns>
        public async Task<OperationResult> TogglePlayerStatus(Player player, bool setReady = false, bool setUnready = false)
        {
            PlayerStatus status;
            if (setReady) status = PlayerStatus.Ready;
            else if (setUnready) status = PlayerStatus.NotReady;
            else status = (PlayerStatus)int.Parse(player.Data["Status"].Value) == PlayerStatus.NotReady ? PlayerStatus.Ready : PlayerStatus.NotReady;

            OperationResult result = await LobbyManager.UpdatePlayerData(Lobby.Id, player.Id, new() { ["Status"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)status).ToString()) });
            if (result.Status == ResultStatus.Success) player.Data["Status"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)status).ToString());
            return result;
        }

        /// <summary>
        /// Changes the team of a player.
        /// </summary>
        /// <param name="player">The player to change.</param>
        /// <param name="team">The new team for the player.</param>
        /// <returns>An OperationResult.</returns>
        public async Task<OperationResult> ChangePlayerTeam(Player player, Team team)
        {
            OperationResult result = await LobbyManager.UpdatePlayerData(Lobby.Id, player.Id, new() { ["Team"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)team).ToString()) });
            if (result.Status == ResultStatus.Success) player.Data["Team"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)team).ToString());
            return result;
        }

        /// <summary>
        /// Updates the game settings in the lobby data.
        /// </summary>
        /// <param name="mapValue">The updated map index.</param>
        /// <param name="roundCountValue">The updated round count.</param>
        /// <param name="roundTimeValue">The updated round time.</param>
        /// <param name="gameModeSelection">The updated game mode.</param>
        /// <returns>An OperationResult.</returns> 
        public async Task<OperationResult> UpdateGameSettings(int mapValue, int roundCountValue, int roundTimeValue, int gameModeSelection)
        {
            Dictionary<string, DataObject> changedData = new()
            {
                ["MapIndex"] = new DataObject(DataObject.VisibilityOptions.Public, mapValue.ToString()),
                ["RoundCount"] = new DataObject(DataObject.VisibilityOptions.Member, roundCountValue.ToString()),
                ["RoundTime"] = new DataObject(DataObject.VisibilityOptions.Member, roundTimeValue.ToString()),
                ["GameMode"] = new DataObject(DataObject.VisibilityOptions.Public, gameModeSelection.ToString())
            };

            OperationResult result = await LobbyManager.UpdateLobbyData(Lobby.Id, changedData);
            if (result.Status == ResultStatus.Success)
            {
                Lobby.Data["MapIndex"] = new DataObject(DataObject.VisibilityOptions.Public, mapValue.ToString());
                Lobby.Data["RoundCount"] = new DataObject(DataObject.VisibilityOptions.Member, roundCountValue.ToString());
                Lobby.Data["RoundTime"] = new DataObject(DataObject.VisibilityOptions.Member, roundTimeValue.ToString());
                Lobby.Data["GameMode"] = new DataObject(DataObject.VisibilityOptions.Public, gameModeSelection.ToString());
            }
            return result;
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <returns>An OperationResult.</returns>
        public async Task<OperationResult> LeaveCurrentLobby()
        {
            try
            {
                leftVolunatarily = true;
                return await LobbyManager.LeaveLobby(Lobby.Id);
            }
            catch (Exception e)
            {
                leftVolunatarily = false;
                return OperationResult.ErrorResult("LeaveLobby", e.Message);
            }
        }

        #region Unity Lobby Events
        /// <summary>
        /// Sets up and subscribes to Unity's built-in lobby events.
        /// </summary>
        private async Task SubscribeToLobbyEvents()
        {
            LobbyEventCallbacks lobbyEventCallbacks = new();

            lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            lobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            lobbyEventCallbacks.PlayerLeft += OnPlayerLeft;
            lobbyEventCallbacks.DataChanged += OnDataChanged;
            lobbyEventCallbacks.PlayerDataChanged += OnPlayerDataChanged;
            lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

            lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, lobbyEventCallbacks);
            if (showDebugMessages) Debug.Log("Subscribed to lobby events");
        }

        /// <summary>
        /// Unsubscribes from all lobby events.
        /// </summary>
        private async Task UnsubscribeFromLobbyEvents()
        {
            if (lobbyEvents != null)
            {
                await lobbyEvents.UnsubscribeAsync();
                lobbyEvents = null;
                if (showDebugMessages) Debug.Log("Unsubscribed from lobby events");
            }
        }

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            if (showDebugMessages) Debug.Log("Lobby changed event received");

            if (lobbyChanges.HostId.Changed)
            {
                if (showDebugMessages) Debug.Log($"Host changed to {lobbyChanges.HostId.Value}");
                if (AuthenticationService.Instance.PlayerId == lobbyChanges.HostId.Value)
                {
                    if (_heartbeatCoroutine == null)
                    {
                        _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(Lobby.Id, 6f));
                        if (showDebugMessages) Debug.Log("Started heartbeat coroutine as new host");
                    }
                }
                LobbyEvents.InvokeLobbyHostMigrated(lobbyChanges.HostId.Value);
            }

            if (lobbyChanges.MaxPlayers.Changed)
            {
                if (showDebugMessages) Debug.Log($"Max players changed to {lobbyChanges.MaxPlayers.Value}");
            }

            if (lobbyChanges.IsPrivate.Changed)
            {
                if (showDebugMessages) Debug.Log($"Lobby privacy changed to {(lobbyChanges.IsPrivate.Value ? "Private" : "Public")}");
            }

            if (lobbyChanges.Name.Changed)
            {
                if (showDebugMessages) Debug.Log($"Lobby name changed to {lobbyChanges.Name.Value}");
            }
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> playersJoined)
        {
            foreach (LobbyPlayerJoined playerJoined in playersJoined)
            {
                if (showDebugMessages) Debug.Log($"Player {playerJoined.Player.Id} joined the lobby");
                LobbyEvents.InvokePlayerJoined(playerJoined.Player.Id);
            }
        }

        private void OnPlayerLeft(List<int> playerIndices)
        {
            foreach (int playerIndex in playerIndices)
            {
                if (showDebugMessages) Debug.Log($"Player {playerIndex} left the lobby");
                LobbyEvents.InvokePlayerLeft(playerIndex);
            }
        }

        private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dataChanges)
        {
            if (showDebugMessages) Debug.Log($"Lobby data changed: {dataChanges.Count} " + (dataChanges.Count == 1 ? "field" : "fields"));
            foreach (var kvp in dataChanges)
            {
                if (kvp.Key == "MapIndex")
                {
                    if (showDebugMessages) Debug.Log($"Map index changed to {kvp.Value.Value.Value}");
                    Lobby.Data["MapIndex"] = kvp.Value.Value;
                }
                else if (kvp.Key == "RoundCount")
                {
                    if (showDebugMessages) Debug.Log($"Round count changed to {kvp.Value.Value.Value}");
                    Lobby.Data["RoundCount"] = kvp.Value.Value;
                }
                else if (kvp.Key == "RoundTime")
                {
                    if (showDebugMessages) Debug.Log($"Round time changed to {kvp.Value.Value.Value}");
                    Lobby.Data["RoundTime"] = kvp.Value.Value;
                }
                else if (kvp.Key == "GameMode")
                {
                    if (showDebugMessages) Debug.Log($"Game mode changed to {kvp.Value.Value.Value}");
                    Lobby.Data["GameMode"] = kvp.Value.Value;
                }
                else if (kvp.Key == "Status")
                {
                    if (showDebugMessages) Debug.Log($"Lobby status changed to {kvp.Value.Value.Value}");
                    Lobby.Data["Status"] = kvp.Value.Value;
                }
                else if (kvp.Key == "RelayJoinCode")
                {
                    if (showDebugMessages) Debug.Log($"Relay join code changed to {kvp.Value.Value.Value}");
                    Lobby.Data["RelayJoinCode"] = kvp.Value.Value;
                }
            }
        }

        private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> changes)
        {
            if (showDebugMessages) Debug.Log($"Player data changed: {changes.Count} " + (changes.Count == 1 ? "player" : "players"));
            foreach (var kvp in changes)
                foreach (var dataChange in kvp.Value)
                {
                    if (dataChange.Key == "Status")
                    {
                        if (showDebugMessages) Debug.Log($"Player {kvp.Key} status changed to {(PlayerStatus)int.Parse(dataChange.Value.Value.Value)}");
                        Lobby.Players[kvp.Key].Data["Status"] = dataChange.Value.Value;
                    }
                    else if (dataChange.Key == "Team")
                    {
                        if (showDebugMessages) Debug.Log($"Player {kvp.Key} team changed to {(Team)int.Parse(dataChange.Value.Value.Value)}");
                        Lobby.Players[kvp.Key].Data["Team"] = dataChange.Value.Value;
                    }
                }
        }

        private void OnKickedFromLobby()
        {
            if (leftVolunatarily) LobbyEvents.InvokeLobbyLeft(OperationResult.SuccessResult("LeftLobby", null, $"Left lobby {Lobby.Name}"));
            else LobbyEvents.InvokeLobbyLeft(OperationResult.WarningResult("LeftLobby", null, $"Kicked from lobby {Lobby.Name}"));
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            Debug.Log($"Lobby connection state changed to: {state}");

            if (state == LobbyEventConnectionState.Subscribing)
                LobbyEvents.InvokePlayerConnecting(AuthenticationService.Instance.PlayerId);
            else if (state == LobbyEventConnectionState.Subscribed)
                LobbyEvents.InvokePlayerConnected(AuthenticationService.Instance.PlayerId);
            else if (state == LobbyEventConnectionState.Unsynced)
                LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
            // else if (state == LobbyEventConnectionState.Unsubscribed)
            //     LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
            // else if (state == LobbyEventConnectionState.Error)
            //     LobbyEvents.InvokePlayerDisconnected(AuthenticationService.Instance.PlayerId);
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