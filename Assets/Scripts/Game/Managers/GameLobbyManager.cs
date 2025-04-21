// using UnityEngine;
// using System.Collections;
// using Unity.Services.Lobbies;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Types;
// using Assets.Scripts.Game.Events;
// using System.Collections.Generic;
// using Assets.Scripts.Framework.Core;
// using Unity.Services.Lobbies.Models;
// using Unity.Services.Authentication;
// // using Assets.Scripts.Framework.Types;
// using Assets.Scripts.Framework.Events;
// using Assets.Scripts.Framework.Managers;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Handles high-level lobby operations and game-specific lobby functionality.
//     /// </summary>
//     public class GameLobbyManager : Singleton<GameLobbyManager>
//     {
//         [Header("Debug Options")]
//         [SerializeField] private bool showDebugMessages = true;

//         public Lobby Lobby { get; private set; }
//         private Coroutine heartbeatCoroutine;
//         private static bool leavingVolunatirily = false;
//         private ILobbyEvents lobbyEvents;

//         public async void Initialize(Lobby lobby)
//         {
//             Lobby = lobby;
//             await SubscribeToLobbyEvents();
//             if (AuthenticationService.Instance.PlayerId == Lobby.HostId) StartHeartbeat(Lobby.Id, 15f);

//             if (showDebugMessages) Debug.Log($"GameLobbyManager initialized with lobby {lobby.Name} ({lobby.Id})");
//         }

//         public async void Cleanup()
//         {
//             StopHeartbeat();
//             await UnsubscribeFromLobbyEvents();
//             Lobby = null;

//             if (showDebugMessages) Debug.Log($"GameLobbyManager cleaned up");
//         }

//         /// <summary>
//         /// Handles the lobby heartbeat, sending regular updates to the lobby service.
//         /// </summary>
//         /// <param name="lobbyId">The ID of the lobby to send heartbeats to.</param>
//         /// <param name="waitTimeSeconds">The time to wait between heartbeats.</param>
//         /// <returns>Coroutine for the heartbeat process.</returns>
//         private IEnumerator HeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
//         {
//             while (true)
//             {
//                 LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
//                 yield return new WaitForSecondsRealtime(waitTimeSeconds);
//             }
//         }

//         private void StartHeartbeat(string lobbyId, float waitTimeSeconds)
//         {
//             if (heartbeatCoroutine != null) StopCoroutine(heartbeatCoroutine);
//             heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(lobbyId, waitTimeSeconds));

//             if (showDebugMessages) Debug.Log($"Started heartbeat coroutine for lobby {lobbyId} every {waitTimeSeconds} seconds");
//         }

//         private void StopHeartbeat()
//         {
//             if (heartbeatCoroutine != null) StopCoroutine(heartbeatCoroutine);
//             heartbeatCoroutine = null;

//             if (showDebugMessages) Debug.Log("Stopped heartbeat coroutine");
//         }

//         /// <summary>
//         /// Updates the game settings in the lobby.
//         /// </summary>
//         /// <param name="mapValue">The updated map index.</param>
//         /// <param name="roundCountValue">The updated round count.</param>
//         /// <param name="roundTimeValue">The updated round time.</param>
//         /// <param name="gameModeSelection">The updated game mode.</param>
//         public async Task UpdateGameSettings(int mapValue, int roundCountValue, int roundTimeValue, int gameModeSelection)
//         {
//             Dictionary<string, DataObject> changedData = new()
//             {
//                 ["MapIndex"] = new DataObject(DataObject.VisibilityOptions.Public, mapValue.ToString()),
//                 ["RoundCount"] = new DataObject(DataObject.VisibilityOptions.Public, roundCountValue.ToString()),
//                 ["RoundTime"] = new DataObject(DataObject.VisibilityOptions.Public, roundTimeValue.ToString()),
//                 ["GameMode"] = new DataObject(DataObject.VisibilityOptions.Public, gameModeSelection.ToString())
//             };

//             // Task<OperationResult> updateTask = LobbyManager.UpdateLobbyData(Lobby.Id, changedData);
//             Task completedTask = await Task.WhenAny(updateTask, Task.Delay(5000));
//             if (completedTask == updateTask)
//             {
//                 OperationResult result = await updateTask;
//                 if (result.Status == ResultStatus.Success)
//                 {
//                     if (showDebugMessages) Debug.Log($"Game settings updated successfully: {result.Message}");
//                     foreach (var kvp in changedData) Lobby.Data[kvp.Key] = kvp.Value;
//                     GameLobbyEvents.InvokeGameSettingsChanged(true, changedData);
//                 }
//                 else
//                 {
//                     if (showDebugMessages) Debug.LogError($"Failed to update game settings: {result.Message}");
//                     GameLobbyEvents.InvokeGameSettingsChanged(false, null);
//                 }
//             }
//             else
//             {
//                 if (showDebugMessages) Debug.LogWarning($"UpdateGameSettings timed out for lobby {Lobby.Id}");
//                 GameLobbyEvents.InvokeGameSettingsChanged(false, null);
//             }
//         }

//         /// <summary>
//         /// Changes the team of a player.
//         /// </summary>
//         /// <param name="playerId">The ID of the player whose team is being changed.</param>
//         /// <param name="team">The new team for the player.</param>
//         public async Task ChangePlayerTeam(string playerId, Team team)
//         {
//             Task<OperationResult> updateTask = LobbyManager.UpdatePlayerData(Lobby.Id, playerId, new() { ["Team"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)team).ToString()) });
//             Task completedTask = await Task.WhenAny(updateTask, Task.Delay(5000));
//             if (completedTask == updateTask)
//             {
//                 OperationResult result = await updateTask;
//                 if (result.Status == ResultStatus.Success)
//                 {
//                     if (showDebugMessages) Debug.Log($"Player {playerId} changed to team {team} successfully: {result.Message}");
//                     Lobby.Players.Find(p => p.Id == playerId).Data["Team"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)team).ToString());
//                     GameLobbyEvents.InvokePlayerTeamChanged(true, playerId, team);
//                 }
//                 else
//                 {
//                     if (showDebugMessages) Debug.LogError($"Failed to change player {playerId} to team {team}: {result.Message}");
//                     GameLobbyEvents.InvokePlayerTeamChanged(false, playerId, team);
//                 }
//             }
//             else
//             {
//                 if (showDebugMessages) Debug.LogWarning($"ChangePlayerTeam timed out for player {playerId}");
//                 GameLobbyEvents.InvokePlayerTeamChanged(false, playerId, team);
//             }
//         }

//         /// <summary>
//         /// Toggle the ready status of the current player.
//         /// </summary>
//         /// <param name="playerId">The ID of the player whose ready status is being toggled.</param>
//         /// <param name="readyStatus">The new ready status for the player.</param>
//         public async Task SetPlayerReadyStatus(string playerId, ReadyStatus readyStatus)
//         {
//             Task<OperationResult> updateTask = LobbyManager.UpdatePlayerData(Lobby.Id, playerId, new() { ["ReadyStatus"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)readyStatus).ToString()) });
//             Task completedTask = await Task.WhenAny(updateTask, Task.Delay(5000));
//             if (completedTask == updateTask)
//             {
//                 OperationResult result = await updateTask;
//                 if (result.Status == ResultStatus.Success)
//                 {
//                     if (showDebugMessages) Debug.Log($"Player {playerId} ready status changed to {readyStatus} successfully: {result.Message}");
//                     Lobby.Players.Find(p => p.Id == playerId).Data["ReadyStatus"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)readyStatus).ToString());
//                     GameLobbyEvents.InvokePlayerReadyStatusChanged(true, playerId, readyStatus);
//                 }
//                 else
//                 {
//                     if (showDebugMessages) Debug.LogError($"Failed to change player {playerId} ready status to {readyStatus}: {result.Message}");
//                     GameLobbyEvents.InvokePlayerReadyStatusChanged(false, playerId, readyStatus);
//                 }
//             }
//             else
//             {
//                 if (showDebugMessages) Debug.LogWarning($"TogglePlayerReadyStatus timed out for player {playerId}");
//                 GameLobbyEvents.InvokePlayerReadyStatusChanged(false, playerId, readyStatus);
//             }
//         }

//         public async Task LeaveLobby(string lobbyId)
//         {
//             leavingVolunatirily = true;
//             await LobbyManager.LeaveLobby(lobbyId);
//         }
//         #region Unity Lobby Events
//         private async Task SubscribeToLobbyEvents()
//         {
//             LobbyEventCallbacks lobbyEventCallbacks = new();

//             lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
//             lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
//             lobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
//             lobbyEventCallbacks.PlayerLeft += OnPlayerLeft;
//             lobbyEventCallbacks.DataChanged += OnDataChanged;
//             lobbyEventCallbacks.PlayerDataChanged += OnPlayerDataChanged;
//             lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

//             lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, lobbyEventCallbacks);
//             if (showDebugMessages) Debug.Log("Subscribed to lobby events");
//         }

//         private async Task UnsubscribeFromLobbyEvents()
//         {
//             if (lobbyEvents != null)
//             {
//                 await lobbyEvents.UnsubscribeAsync();
//                 lobbyEvents = null;
//                 if (showDebugMessages) Debug.Log("Unsubscribed from lobby events");
//             }
//         }

//         private void OnKickedFromLobby()
//         {
//             if (leavingVolunatirily) LobbyEvents.InvokeLobbyLeft(OperationResult.SuccessResult("LeftLobby", "Left lobby"));
//             else LobbyEvents.InvokeLobbyKicked(OperationResult.SuccessResult("KickedFromLobby", "You have been kicked from the lobby"));
//         }

//         private void OnLobbyChanged(ILobbyChanges lobbyChanges)
//         {
//             if (lobbyChanges.HostId.Changed || lobbyChanges.MaxPlayers.Changed || lobbyChanges.IsPrivate.Changed || lobbyChanges.Name.Changed)
//                 Lobby = new Lobby(
//                     id: Lobby.Id,
//                     lobbyCode: Lobby.LobbyCode,
//                     name: Lobby.Name,
//                     maxPlayers: Lobby.MaxPlayers,
//                     isPrivate: Lobby.IsPrivate,
//                     players: Lobby.Players,
//                     data: Lobby.Data,
//                     hostId: lobbyChanges.HostId.Value,
//                     lastUpdated: Lobby.LastUpdated
//                 );

//             if (lobbyChanges.HostId.Changed)
//             {
//                 if (showDebugMessages) Debug.Log($"Host changed to {lobbyChanges.HostId.Value}");

//                 if (AuthenticationService.Instance.PlayerId == lobbyChanges.HostId.Value)
//                 {
//                     if (showDebugMessages) Debug.Log("You are now the host of the lobby");
//                     StartHeartbeat(Lobby.Id, 15f);
//                 }
//                 else
//                 {
//                     if (showDebugMessages) Debug.Log("You are no longer the host of the lobby");
//                     StopHeartbeat();
//                 }
//                 LobbyEvents.InvokeHostMigrated(lobbyChanges.HostId.Value);
//             }
//             if (lobbyChanges.MaxPlayers.Changed)
//             {
//                 if (showDebugMessages) Debug.Log($"Max players changed to {lobbyChanges.MaxPlayers.Value}");
//                 LobbyEvents.InvokeMaxPlayersChanged(lobbyChanges.MaxPlayers.Value);
//             }
//             if (lobbyChanges.IsPrivate.Changed)
//             {
//                 if (showDebugMessages) Debug.Log($"Lobby privacy changed to {(lobbyChanges.IsPrivate.Value ? "Private" : "Public")}");
//                 LobbyEvents.InvokePrivacyChanged(lobbyChanges.IsPrivate.Value);
//             }
//             if (lobbyChanges.Name.Changed)
//             {
//                 if (showDebugMessages) Debug.Log($"Lobby name changed to {lobbyChanges.Name.Value}");
//                 LobbyEvents.InvokeNameChanged(lobbyChanges.Name.Value);
//             }
//         }

//         private void OnPlayerJoined(List<LobbyPlayerJoined> players)
//         {
//             foreach (LobbyPlayerJoined player in players)
//             {
//                 if (showDebugMessages) Debug.Log($"Player {player.Player.Id} joined the lobby");
//                 Lobby.Players.Add(player.Player);
//                 LobbyEvents.InvokePlayerJoined(player.Player.Id);
//             }
//         }

//         private void OnPlayerLeft(List<int> playerIndices)
//         {
//             foreach (int index in playerIndices)
//             {
//                 if (showDebugMessages) Debug.Log($"Player {Lobby.Players[index].Id} left the lobby");
//                 string leftId = Lobby.Players[index].Id;
//                 Lobby.Players.RemoveAt(index);
//                 LobbyEvents.InvokePlayerLeft(leftId);
//             }
//         }

//         private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dataChanges)
//         {
//             if (Lobby.HostId == AuthenticationService.Instance.PlayerId) return;

//             foreach (var kvp in dataChanges)
//             {
//                 if (kvp.Value.Removed) Lobby.Data.Remove(kvp.Key);
//                 else Lobby.Data[kvp.Key] = kvp.Value.Value;

//                 if (kvp.Key == "MapIndex") { if (showDebugMessages) Debug.Log($"Map index changed to {kvp.Value.Value.Value}"); }
//                 else if (kvp.Key == "RoundCount") { if (showDebugMessages) Debug.Log($"Round count changed to {kvp.Value.Value.Value}"); }
//                 else if (kvp.Key == "RoundTime") { if (showDebugMessages) Debug.Log($"Round time changed to {kvp.Value.Value.Value}"); }
//                 else if (kvp.Key == "GameMode") { if (showDebugMessages) Debug.Log($"Game mode changed to {(GameMode)int.Parse(kvp.Value.Value.Value)}"); }
//             }

//             GameLobbyEvents.InvokeGameSettingsChanged(true, Lobby.Data);
//         }

//         private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> changes)
//         {
//             foreach (var kvp in changes)
//                 foreach (var dataChange in kvp.Value)
//                 {
//                     if (Lobby.Players[kvp.Key].Id == AuthenticationService.Instance.PlayerId) continue;

//                     if (!dataChange.Value.Removed) Lobby.Players[kvp.Key].Data[dataChange.Key] = dataChange.Value.Value;
//                     else Lobby.Players[kvp.Key].Data.Remove(dataChange.Key);

//                     if (dataChange.Key == "Team")
//                     {
//                         if (showDebugMessages) Debug.Log($"Player {Lobby.Players[kvp.Key].Id} team changed to {(Team)int.Parse(dataChange.Value.Value.Value)}");
//                         GameLobbyEvents.InvokePlayerTeamChanged(true, Lobby.Players[kvp.Key].Id, (Team)int.Parse(dataChange.Value.Value.Value));
//                     }
//                     else if (dataChange.Key == "ReadyStatus")
//                     {
//                         if (showDebugMessages) Debug.Log($"Player {Lobby.Players[kvp.Key].Id} ready status changed to {(ReadyStatus)int.Parse(dataChange.Value.Value.Value)}");
//                         GameLobbyEvents.InvokePlayerReadyStatusChanged(true, Lobby.Players[kvp.Key].Id, (ReadyStatus)int.Parse(dataChange.Value.Value.Value));
//                     }
//                     else if (dataChange.Key == "ConnectionStatus")
//                     {
//                         if (showDebugMessages) Debug.Log($"Player {Lobby.Players[kvp.Key].Id} connection status changed to {(ConnectionStatus)int.Parse(dataChange.Value.Value.Value)}");
//                         GameLobbyEvents.InvokePlayerConnectionStatusChanged(true, Lobby.Players[kvp.Key].Id, (ConnectionStatus)int.Parse(dataChange.Value.Value.Value));
//                     }
//                 }
//         }

//         private async void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
//         {
//             var connectionStatus = state switch
//             {
//                 LobbyEventConnectionState.Subscribing => ConnectionStatus.Connecting,
//                 LobbyEventConnectionState.Subscribed => ConnectionStatus.Connected,
//                 LobbyEventConnectionState.Error => ConnectionStatus.Error,
//                 LobbyEventConnectionState.Unknown or LobbyEventConnectionState.Unsubscribed or LobbyEventConnectionState.Unsynced => ConnectionStatus.Disconnected,
//                 _ => ConnectionStatus.Disconnected,
//             };

//             if (showDebugMessages) Debug.Log($"Lobby event connection state changed to {connectionStatus}");

//             Dictionary<string, PlayerDataObject> updateData = new() { ["ConnectionStatus"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)connectionStatus).ToString()) };
//             if (connectionStatus == ConnectionStatus.Disconnected) updateData.Add("ReadyStatus", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, ((int)ReadyStatus.NotReady).ToString()));
//             await LobbyManager.UpdatePlayerData(Lobby.Id, AuthenticationService.Instance.PlayerId, updateData);
//         }
//         #endregion

//         #region Game Flow
//         /// <summary>
//         /// Handles the entire loading process - transitions to loading scene, initializes the relay, and then loads the game map
//         /// </summary>
//         // public async Task HandleGameLoading(string mapName, Sprite mapThumbnail, string mapSceneName)
//         // {
//         //     try
//         //     {
//         //         // Step 1: Load the loading scene locally
//         //         Debug.Log($"Loading loading screen for {mapName}");
//         //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
//         //         while (!loadOperation.isDone)
//         //             await Task.Yield();

//         //         // Step 2: Set up the loading UI
//         //         loadingPanelController = FindObjectOfType<LoadingPanelController>();
//         //         if (loadingPanelController == null)
//         //         {
//         //             Debug.LogError("LoadingPanelController not found in Loading scene!");
//         //             return;
//         //         }

//         //         loadingPanelController.StartLoading(mapName, mapThumbnail,
//         //             LobbyManager.Instance.IsLobbyHost ? "Setting up relay server..." : "Waiting for host...");

//         //         // Step 3: Host sets up relay and waits for clients to connect
//         //         if (LobbyManager.Instance.IsLobbyHost)
//         //         {
//         //             // Create relay
//         //             string relayJoinCode = await RelayManager.Instance.CreateRelay(LobbyManager.Instance.MaxPlayers);
//         //             loadingPanelController.SetStatus("Created relay server...");

//         //             // Update lobby data for clients to join
//         //             lobbyData.InGame = true;
//         //             lobbyData.GameStarted = false;
//         //             lobbyData.RelayJoinCode = relayJoinCode;
//         //             await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

//         //             // Update host's player data with connection info
//         //             string allocationId = RelayManager.Instance.GetAllocationId();
//         //             string connectionData = RelayManager.Instance.GetConnectionData();
//         //             localLobbyPlayerData.InGame = true;
//         //             await LobbyManager.Instance.UpdatePlayerData(
//         //                 localLobbyPlayerData.PlayerId,
//         //                 localLobbyPlayerData.Serialize(),
//         //                 allocationId,
//         //                 connectionData);

//         //             // Initialize network BEFORE waiting for players
//         //             SetupNetworking();
//         //             await Task.Delay(1000); // Give network time to initialize

//         //             // Wait for clients to connect to relay
//         //             loadingPanelController.SetStatus("Waiting for players to join relay...");
//         //             int connectedPlayersNeeded = LobbyManager.Instance.MaxPlayers;
//         //             int waitAttempts = 0;
//         //             int maxWaitAttempts = 40; // 20 seconds total wait time

//         //             while (waitAttempts < maxWaitAttempts)
//         //             {
//         //                 // Count players by tracking in-game flags in player data
//         //                 int connectedPlayers = lobbyPlayersData.Count(p => p.InGame);

//         //                 // Update UI
//         //                 loadingPanelController.SetStatus($"Waiting for players to join relay... ({connectedPlayers}/{connectedPlayersNeeded})");

//         //                 // Check if all expected players are connected
//         //                 if (connectedPlayers >= connectedPlayersNeeded)
//         //                 {
//         //                     Debug.Log($"All {connectedPlayers}/{connectedPlayersNeeded} players connected!");
//         //                     break;
//         //                 }

//         //                 await Task.Delay(500);
//         //                 waitAttempts++;
//         //             }

//         //             // Signal to clients that game is starting
//         //             loadingPanelController.SetStatus("Starting game...");
//         //             await Task.Delay(1000);

//         //             lobbyData.GameStarted = true;
//         //             await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

//         //             // Load game map with network scene manager
//         //             LoadGameMap(mapSceneName);
//         //         }
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error in HandleGameLoading: {ex.Message}");
//         //         await ReturnToLobby($"Error loading game: {ex.Message}");
//         //     }
//         // }

//         // private void SetupNetworking()
//         // {
//         //     try
//         //     {
//         //         if (NetworkManager.Singleton == null)
//         //         {
//         //             Debug.LogError("NetworkManager.Singleton is null!");
//         //             return;
//         //         }

//         //         if (NetworkManager.Singleton.IsListening)
//         //         {
//         //             Debug.Log("NetworkManager is already listening, skipping setup");
//         //             return;
//         //         }

//         //         // Configure NetworkManager
//         //         NetworkManager.Singleton.NetworkConfig.EnableSceneManagement = true;

//         //         if (RelayManager.Instance.IsHost)
//         //         {
//         //             Debug.Log("Setting up host relay connection");
//         //             (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) =
//         //                 RelayManager.Instance.GetHostConnectionInfo();

//         //             var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
//         //             transport.SetHostRelayData(
//         //                 dtlsAddress,
//         //                 (ushort)dtlsPort,
//         //                 AllocationId,
//         //                 Key,
//         //                 ConnectionData);

//         //             bool success = NetworkManager.Singleton.StartHost();
//         //             Debug.Log($"Started host: {success}");
//         //         }
//         //         else
//         //         {
//         //             Debug.Log("Setting up client relay connection");
//         //             (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) =
//         //                 RelayManager.Instance.GetClientConnectionInfo();

//         //             var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
//         //             transport.SetClientRelayData(
//         //                 dtlsAddress,
//         //                 (ushort)dtlsPort,
//         //                 AllocationId,
//         //                 Key,
//         //                 ConnectionData,
//         //                 HostConnectionData);

//         //             bool success = NetworkManager.Singleton.StartClient();
//         //             Debug.Log($"Started client: {success}");
//         //         }
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error setting up networking: {ex.Message}");
//         //     }
//         // }

//         /// <summary>
//         /// Starts the game.
//         /// </summary>
//         /// <param name="mapName">Name of the map.</param>
//         /// <param name="mapThumbnail">Thumbnail of the map.</param>
//         /// <param name="mapSceneName">Scene name of the map.</param>
//         // public async Task StartGame(string mapName, Sprite mapThumbnail, string mapSceneName)
//         // {
//         //     try
//         //     {
//         //         Debug.Log($"Starting game with map: {mapName}, scene: {mapSceneName}");
//         //         inGame = true;
//         //         await HandleGameLoading(mapName, mapThumbnail, mapSceneName);
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error starting game: {ex.Message}");
//         //         inGame = false;
//         //     }
//         // }

//         /// <summary>
//         /// Joins an existing relay server.
//         /// </summary>
//         // private async Task JoinRelayServer(string relayJoinCode)
//         // {
//         //     try
//         //     {
//         //         Debug.Log($"Client joining relay server with code: {relayJoinCode}");

//         //         // Load loading scene
//         //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
//         //         while (!loadOperation.isDone)
//         //             await Task.Yield();

//         //         loadingPanelController = FindObjectOfType<LoadingPanelController>();
//         //         if (loadingPanelController == null)
//         //         {
//         //             Debug.LogError("LoadingPanelController not found!");
//         //             return;
//         //         }

//         //         MapInfo mapInfo = MapSelectionManager.Instance.GetMapInfo(lobbyData.MapIndex);

//         //         string mapName = mapInfo.MapName;
//         //         Sprite mapThumbnail = mapInfo.MapThumbnail;

//         //         loadingPanelController.StartLoading(mapName, mapThumbnail, "Joining relay server...");

//         //         // Set in-game state to prevent multiple join attempts
//         //         inGame = true;

//         //         // Initialize Unity Transport with relay info
//         //         await RelayManager.Instance.JoinRelay(relayJoinCode);
//         //         loadingPanelController.SetStatus("Connected to relay server...");

//         //         // Setup networking on client
//         //         SetupNetworking();

//         //         // Update player data with connection info
//         //         string allocationId = RelayManager.Instance.GetAllocationId();
//         //         string connectionData = RelayManager.Instance.GetConnectionData();
//         //         localLobbyPlayerData.InGame = true;
//         //         await LobbyManager.Instance.UpdatePlayerData(
//         //             localLobbyPlayerData.PlayerId,
//         //             localLobbyPlayerData.Serialize(),
//         //             allocationId,
//         //             connectionData);

//         //         loadingPanelController.SetStatus("Waiting for host to start game...");
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error joining relay server: {ex.Message}");
//         //         inGame = false;
//         //         await ReturnToLobby("Failed to join game");
//         //     }
//         // }

//         /// <summary>
//         /// Loads the game map using NetworkSceneManager.
//         /// </summary>
//         // public void LoadGameMap(string mapSceneName)
//         // {
//         //     try
//         //     {
//         //         Debug.Log($"Loading game map: {mapSceneName}");

//         //         if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
//         //         {
//         //             Debug.Log($"Host loading scene via NetworkSceneManager: {mapSceneName}");
//         //             NetworkManager.Singleton.SceneManager.LoadScene(mapSceneName, LoadSceneMode.Single);
//         //         }
//         //         else
//         //         {
//         //             Debug.Log("Client waiting for host to load scene via NetworkSceneManager");
//         //         }
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error loading game map: {ex.Message}");
//         //     }
//         // }

//         /// <summary>
//         /// Returns to the lobby after a game and sets all players to unready.
//         /// </summary>
//         // public async Task ReturnToLobby(string message = "Returning to lobby...")
//         // {
//         //     try
//         //     {
//         //         inGame = false;

//         //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
//         //         while (!loadOperation.isDone)
//         //             await Task.Yield();

//         //         loadingPanelController = FindObjectOfType<LoadingPanelController>();

//         //         if (LobbyManager.Instance.IsLobbyHost)
//         //         {
//         //             if (lobbyData != null)
//         //             {
//         //                 lobbyData.RelayJoinCode = default;
//         //                 lobbyData.GameStarted = false;
//         //                 await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
//         //             }

//         //             await SetAllPlayersUnready();
//         //         }

//         //         await Task.Delay(1500);

//         //         SceneManager.LoadSceneAsync("Lobby");
//         //     }
//         //     catch (System.Exception ex)
//         //     {
//         //         Debug.LogError($"Error returning to lobby: {ex.Message}");

//         //         await Task.Delay(1000);
//         //         SceneManager.LoadSceneAsync("Lobby");
//         //     }
//         // }

//         #endregion

//     }
// }