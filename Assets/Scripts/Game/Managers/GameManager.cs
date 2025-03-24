// using System.Linq;
// using UnityEngine;
// using Unity.Netcode;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.SceneManagement;
// using Unity.Netcode.Transports.UTP;
// using Assets.Scripts.Game.Managers;
// using Assets.Scripts.Game.UI.Controllers.GameplayMenu;

// namespace Assets.Scripts.Game.Managers
// {
//     public class GameManager : NetworkBehaviour
//     {
//         public static GameManager Instance { get; private set; }

//         private const float DEATH_HEIGHT = -5f;

//         [SerializeField] private GameObject inGameUI;

//         #region Private Fields
//         private Dictionary<ulong, int> playerTeams = new();
//         private GameplayPanelController inGameUIController;
//         private bool isCountingDown = false;
//         private bool canCheckPositions = false;
//         private bool isGameInitialized = false;
//         private bool isUICreated = false;
//         #endregion

//         #region Unity Lifecycle

//         private void Awake()
//         {
//             // Handle singleton pattern
//             if (Instance == null)
//             {
//                 Instance = this;
//                 Debug.Log("GameManager instance created");
//             }
//             else if (Instance != this)
//             {
//                 Debug.LogWarning("Multiple GameManager instances detected. Destroying duplicate.");
//                 Destroy(gameObject);
//                 return;
//             }
//         }

//         private void Start()
//         {
//             Debug.Log("GameManager Start() - Current scene: " + SceneManager.GetActiveScene().name);

//             try
//             {
//                 if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
//                 {
//                     Debug.Log("Network is already listening, initializing game only");

//                     if (IsServer && !isGameInitialized)
//                         InitializeGame();
//                     return;
//                 }

//                 if (SceneManager.GetActiveScene().name != "Loading" && SceneManager.GetActiveScene().name != "Lobby")
//                 {
//                     SetupNetworking();

//                     if (RelayManager.Instance != null)
//                     {
//                         if (RelayManager.Instance.IsHost)
//                         {
//                             Debug.Log("Starting as host");
//                             StartAsHost();
//                         }
//                         else
//                         {
//                             Debug.Log("Starting as client");
//                             StartAsClient();
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError("RelayManager is null - unable to start network connection");
//                         ReturnToLobbyAfterError("Network setup failed: RelayManager not found");
//                     }
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error in GameManager Start: {ex.Message}");
//                 ReturnToLobbyAfterError("Game initialization failed: " + ex.Message);
//             }
//         }

//         private void Update()
//         {
//             // Only server updates game state
//             if (IsServer)
//             {
//                 // Update countdown timer
//                 UpdateCountdown();

//                 // Check for players falling off the map
//                 if (gameState.Value == GameState.RoundInProgress && canCheckPositions)
//                     CheckPlayerPositions();
//             }
//         }

//         public override void OnDestroy()
//         {
//             Debug.Log("GameManager OnDestroy()");
//             CleanupGameResources();
//             base.OnDestroy();
//         }
//         #endregion

//         #region Network Setup

//         private void SetupNetworking()
//         {
//             Debug.Log("Setting up networking");

//             // Configure network manager
//             NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

//             // Only host sets up callbacks
//             if (RelayManager.Instance.IsHost)
//             {
//                 NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
//                 NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//                 NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
//             }
//         }

//         private void StartAsHost()
//         {
//             try
//             {
//                 if (!NetworkManager.Singleton.IsListening)
//                 {
//                     (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) = RelayManager.Instance.GetHostConnectionInfo();
//                     Debug.Log($"Host connection info obtained - AllocationId: {AllocationId}, Key: {Key}, ConnectionData: {ConnectionData}");

//                     NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(dtlsAddress, (ushort)dtlsPort, AllocationId, Key, ConnectionData);

//                     bool success = NetworkManager.Singleton.StartHost();
//                     Debug.Log($"StartHost() result: {success}");
//                 }
//                 else
//                     Debug.Log("Network is already initialized, skipping StartHost");

//                 NetworkObject netObj = GetComponent<NetworkObject>();
//                 if (netObj != null && !netObj.IsSpawned)
//                 {
//                     netObj.Spawn();
//                     Debug.Log("GameManager NetworkObject spawned");
//                 }
//                 else
//                     Debug.Log("GameManager NetworkObject already spawned or null");
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Error starting host: {e.Message}");
//                 ReturnToLobbyAfterError("Failed to start as host");
//             }
//         }

//         private void StartAsClient()
//         {
//             try
//             {
//                 if (!NetworkManager.Singleton.IsListening)
//                 {
//                     (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) = RelayManager.Instance.GetClientConnectionInfo();
//                     Debug.Log($"Client connection info obtained - AllocationId: {AllocationId}, Key: {Key}, ConnectionData: {ConnectionData}");

//                     NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(dtlsAddress, (ushort)dtlsPort, AllocationId, Key, ConnectionData, HostConnectionData);

//                     bool success = NetworkManager.Singleton.StartClient();
//                     Debug.Log($"StartClient() result: {success}");
//                 }
//                 else
//                     Debug.Log("Network is already initialized, skipping StartClient");
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Error starting client: {e.Message}");
//                 ReturnToLobbyAfterError("Failed to connect to host");
//             }
//         }

//         private void OnSceneEvent(SceneEvent sceneEvent)
//         {
//             // Log all scene events for debugging
//             Debug.Log($"SceneEvent: {sceneEvent.SceneEventType} for scene {sceneEvent.SceneName}");

//             // Only handle completed loads
//             if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
//             {
//                 string sceneName = sceneEvent.SceneName;
//                 Debug.Log($"Scene {sceneName} loaded via NetworkSceneManager");

//                 // Only initialize in gameplay maps (not utility scenes)
//                 if (sceneName != "Loading" && sceneName != "Lobby" && sceneName != "Main" && sceneName != "Initialization")
//                 {
//                     // Clear initialization flag to ensure we initialize properly
//                     isGameInitialized = false;
//                     isUICreated = false;

//                     if (IsServer)
//                     {
//                         Debug.Log("SERVER initializing game after network scene load");
//                         InitializeGame();

//                         // Sync state to all clients with slight delay to ensure they're ready
//                         StartCoroutine(SyncStateAfterDelay(0.5f));
//                     }
//                     else
//                     {
//                         Debug.Log("CLIENT creating UI after network scene load");
//                         // Clients only create UI here, state sync will come from server
//                         CreateInGameUI();
//                     }
//                 }
//             }
//         }

//         private IEnumerator SyncStateAfterDelay(float delay)
//         {
//             yield return new WaitForSeconds(delay);
//             Debug.Log($"SERVER syncing state to clients: {gameState.Value}, round {currentRound.Value}");
//             SyncGameStateClientRpc(gameState.Value, currentRound.Value);
//         }

//         private void InitializeGame()
//         {
//             Debug.Log($"InitializeGame called, isGameInitialized: {isGameInitialized}");

//             // Always create UI regardless of initialization state
//             CreateInGameUI();

//             // Only initialize game state once
//             if (!isGameInitialized)
//             {
//                 if (IsServer)
//                 {
//                     Debug.Log("Setting initial game state to RoundStarting");

//                     // Reset all game state
//                     gameState.Value = GameState.RoundStarting;
//                     currentRound.Value = 1;
//                     team1Score.Value = 0;
//                     team2Score.Value = 0;

//                     // Start the countdown timer
//                     StartCountdown(roundStartCountdown);
//                 }

//                 isGameInitialized = true;
//                 Debug.Log($"Game initialized with state: {gameState.Value}");
//             }
//         }

//         [ClientRpc]
//         private void SyncGameStateClientRpc(GameState state, int round)
//         {
//             Debug.Log($"CLIENT received game state sync: {state}, round: {round}");

//             try
//             {
//                 // Create UI if it hasn't been created yet
//                 if (!isUICreated)
//                 {
//                     CreateInGameUI();
//                 }

//                 // Explicitly update network variables for clients
//                 if (!IsServer)
//                 {
//                     gameState.Value = state;
//                     currentRound.Value = round;
//                 }

//                 // Update UI
//                 UpdateUIState(state);

//                 if (inGameUIController != null)
//                 {
//                     inGameUIController.UpdateRound(round, GameLobbyManager.Instance?.RoundCount ?? 3);

//                     // Make sure we update scores too
//                     inGameUIController.UpdateScores(team1Score.Value, team2Score.Value);
//                 }

//                 isGameInitialized = true;

//                 Debug.Log($"Client UI initialized with game state: {state}");
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Error in SyncGameStateClientRpc: {ex.Message}");
//             }
//         }

//         public override void OnNetworkSpawn()
//         {
//             base.OnNetworkSpawn();
//             Debug.Log($"OnNetworkSpawn - IsServer: {IsServer}, IsClient: {IsClient}");

//             NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

//             RegisterNetworkCallbacks();
//         }

//         public override void OnNetworkDespawn()
//         {
//             if (NetworkManager.Singleton != null)
//                 NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;

//             base.OnNetworkDespawn();
//         }

//         private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
//         {
//             // Always approve connections
//             response.Approved = true;
//             response.CreatePlayerObject = true;
//             response.PlayerPrefabHash = NetworkManager.Singleton.NetworkConfig.PlayerPrefab.GetComponent<NetworkObject>().PrefabIdHash;

//             // Assign team
//             int teamNumber = (playerTeams.Count % 2) + 1;
//             playerTeams[request.ClientNetworkId] = teamNumber;

//             // Set spawn position based on team
//             float xPos = teamNumber == 1 ? -3f : 3f;
//             response.Position = new Vector3(xPos, 1f, 0);

//             Debug.Log($"Client {request.ClientNetworkId} approved on team {teamNumber}");
//         }
//         #endregion

//         #region Client Connection Handling

//         private void OnClientConnected(ulong clientId)
//         {
//             if (!IsServer) return;

//             Debug.Log($"Client connected: {clientId}");
//             AssignPlayerToTeam(clientId);
//         }

//         private void OnClientDisconnected(ulong clientId)
//         {
//             if (!IsServer) return;

//             Debug.Log($"Client disconnected: {clientId}");

//             // Remove from team tracking
//             if (playerTeams.ContainsKey(clientId))
//                 playerTeams.Remove(clientId);

//             // Handle game impact of disconnection
//             if (gameState.Value == GameState.RoundInProgress && NetworkManager.Singleton.ConnectedClientsList.Count < 2)
//             {
//                 // If only one player left, award win to them
//                 if (playerTeams.Count > 0)
//                     EndRound(playerTeams.First().Value);
//             }
//         }

//         private void AssignPlayerToTeam(ulong clientId)
//         {
//             // Balance teams by count
//             int team1Count = playerTeams.Count(p => p.Value == 1);
//             int team2Count = playerTeams.Count(p => p.Value == 2);

//             // Assign to smaller team
//             int teamToAssign = team1Count <= team2Count ? 1 : 2;
//             playerTeams[clientId] = teamToAssign;

//             Debug.Log($"Player {clientId} assigned to Team {teamToAssign}");
//         }
//         #endregion

//         #region Game Flow Control

//         private void UpdateCountdown()
//         {
//             // Skip if countdown not active
//             if (!isCountingDown) return;

//             // Decrement timer
//             countdownTimer.Value -= Time.deltaTime;

//             // Check for countdown completion
//             if (countdownTimer.Value <= 0f)
//             {
//                 isCountingDown = false;
//                 AdvanceGameState();
//             }
//         }

//         private void StartRound()
//         {
//             // Respawn players at starting positions
//             RespawnPlayers();

//             // Update game state
//             gameState.Value = GameState.RoundInProgress;

//             // Safety delay before checking positions
//             canCheckPositions = false;
//             StartCoroutine(EnablePositionCheckingAfterDelay(0.5f));

//             Debug.Log($"Round {currentRound.Value} started");
//         }

//         #endregion

//         #region Player Management

//         private void RespawnPlayers()
//         {
//             // Only server respawns players
//             if (!IsServer) return;

//             Debug.Log("Respawning all players...");

//             // Loop through all connected clients
//             foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
//             {
//                 if (client.PlayerObject == null) continue;

//                 // Get team and position
//                 int team = GetPlayerTeam(client.ClientId);
//                 float xPos = team == 1 ? -3f : 3f;  // Team 1 left, Team 2 right
//                 Vector3 spawnPos = new(xPos, 1f, 0);

//                 // Reset physics on server
//                 ResetPlayerPhysics(client.PlayerObject, spawnPos);

//                 // Notify client to reset position
//                 RespawnPlayerClientRpc(client.ClientId, spawnPos);
//             }
//         }

//         private int GetPlayerTeam(ulong clientId)
//         {
//             // Return team if already assigned
//             if (playerTeams.TryGetValue(clientId, out int team))
//                 return team;

//             // Assign new team if not found
//             AssignPlayerToTeam(clientId);
//             return playerTeams[clientId];
//         }

//         private void ResetPlayerPhysics(NetworkObject playerObject, Vector3 position)
//         {
//             if (playerObject == null) return;

//             // Set position
//             playerObject.transform.position = position;

//             // Reset physics
//             Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
//             if (rb != null)
//             {
//                 rb.velocity = Vector2.zero;
//                 rb.angularVelocity = 0;
//                 rb.constraints = RigidbodyConstraints2D.FreezeAll;  // Freeze temporarily

//                 // Unfreeze after delay
//                 StartCoroutine(UnfreezePlayerAfterDelay(rb, 0.5f));
//             }
//         }

//         private IEnumerator UnfreezePlayerAfterDelay(Rigidbody2D rb, float delay)
//         {
//             yield return new WaitForSeconds(delay);
//             if (rb != null)
//             {
//                 // Unfreeze all except rotation
//                 rb.constraints = RigidbodyConstraints2D.None;
//                 rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//             }
//         }

//         private IEnumerator EnablePositionCheckingAfterDelay(float delay)
//         {
//             yield return new WaitForSeconds(delay);
//             canCheckPositions = true;
//         }

//         private void CheckPlayerPositions()
//         {
//             // Skip if position checking not enabled yet
//             if (!canCheckPositions) return;

//             // Check each player's position
//             foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
//             {
//                 if (client.PlayerObject == null) continue;

//                 // Check if player fell below threshold
//                 if (client.PlayerObject.transform.position.y < DEATH_HEIGHT)
//                 {
//                     if (playerTeams.TryGetValue(client.ClientId, out int team))
//                     {
//                         // If team 1 fell, team 2 wins and vice versa
//                         TeamWins(team == 1 ? 2 : 1);
//                         return;
//                     }
//                 }
//             }
//         }
//         #endregion

//         #region Game Result Handling

//         private void ReturnToLobby()
//         {
//             // Only server initiates return to lobby
//             if (IsServer)
//                 ReturnToLobbyClientRpc();
//         }

//         private void ReturnToLobbyAfterError(string errorMessage)
//         {
//             StartCoroutine(ReturnToLobbyRoutine(errorMessage));
//         }
//         #endregion

//         #region ClientRPCs

//         [ClientRpc]
//         private void RespawnPlayerClientRpc(ulong clientId, Vector3 position)
//         {
//             // Only process for local client
//             if (clientId != NetworkManager.Singleton.LocalClientId) return;

//             // Find player objects owned by this client
//             var playerObjects = FindObjectsOfType<NetworkObject>()
//                 .Where(netObj => netObj.IsPlayerObject && netObj.OwnerClientId == clientId);

//             // Reset each object
//             foreach (var playerObj in playerObjects)
//             {
//                 playerObj.transform.position = position;

//                 Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
//                 if (rb != null)
//                 {
//                     rb.velocity = Vector2.zero;
//                     rb.angularVelocity = 0;
//                 }
//             }
//         }

//         [ClientRpc]
//         private void ReturnToLobbyClientRpc()
//         {
//             StartCoroutine(ReturnToLobbyRoutine());
//         }
//         #endregion

//         #region UI State Management

//         private void CreateInGameUI()
//         {
//             // Avoid creating UI multiple times
//             if (isUICreated)
//             {
//                 Debug.Log("UI already created, skipping");
//                 return;
//             }

//             Debug.Log("Creating in-game UI");

//             // Create UI instance
//             GameObject uiObject = Instantiate(inGameUI);
//             inGameUIController = uiObject.GetComponent<GameplayPanelController>();

//             if (inGameUIController != null)
//             {
//                 // Initialize UI with current state
//                 UpdateUIState(gameState.Value);
//                 Debug.Log("Game UI created successfully");
//             }
//             else
//             {
//                 Debug.LogError("Failed to get GameplayPanelController component from UI prefab");
//             }

//             isUICreated = true;
//         }

//         private void UpdateUIState(GameState state)
//         {
//             if (inGameUIController == null)
//             {
//                 Debug.LogWarning("Cannot update UI state - UI controller is null");
//                 return;
//             }

//             // Update UI based on current game state
//             switch (state)
//             {
//                 case GameState.RoundStarting:
//                     int maxRounds = GameLobbyManager.Instance != null ? GameLobbyManager.Instance.RoundCount : 3;
//                     inGameUIController.ShowRoundStartingScreen(currentRound.Value, maxRounds);
//                     break;

//                 case GameState.RoundInProgress:
//                     inGameUIController.ShowGameplayScreen(NetworkManager.Singleton.IsHost);
//                     break;

//                 case GameState.RoundEnding:
//                     inGameUIController.ShowRoundEndScreen(lastRoundWinner.Value);
//                     break;

//                 case GameState.GameEnding:
//                     int gameWinner = team1Score.Value > team2Score.Value ? 1 : 2;
//                     inGameUIController.ShowGameEndScreen(gameWinner, team1Score.Value, team2Score.Value);
//                     break;

//                 case GameState.ReturnToLobby:
//                     inGameUIController.ShowReturnToLobbyScreen();
//                     break;
//             }
//         }

//         private void RegisterNetworkCallbacks()
//         {
//             // Register for network variable change events
//             gameState.OnValueChanged += OnGameStateChanged;
//             currentRound.OnValueChanged += OnRoundChanged;
//             team1Score.OnValueChanged += OnTeam1ScoreChanged;
//             team2Score.OnValueChanged += OnTeam2ScoreChanged;
//             countdownTimer.OnValueChanged += OnCountdownChanged;
//         }
//         #endregion

//         #region Network Variable Callbacks

//         private void OnGameStateChanged(GameState previous, GameState current)
//         {
//             Debug.Log($"Game state changed from {previous} to {current}");

//             // Invoke event for listeners
//             GameStateChanged?.Invoke(previous, current);

//             // Update UI
//             UpdateUIState(current);
//         }

//         private void OnCountdownChanged(float previous, float current)
//         {
//             // Update countdown display
//             if (inGameUIController != null)
//                 inGameUIController.UpdateCountdown(Mathf.CeilToInt(current));
//         }

//         private void OnTeam1ScoreChanged(int previous, int current)
//         {
//             // Update score display
//             if (inGameUIController != null)
//                 inGameUIController.UpdateScores(current, team2Score.Value);
//         }

//         private void OnTeam2ScoreChanged(int previous, int current)
//         {
//             // Update score display
//             if (inGameUIController != null)
//                 inGameUIController.UpdateScores(team1Score.Value, current);
//         }

//         private void OnRoundChanged(int previous, int current)
//         {
//             // Update round display
//             if (inGameUIController != null)
//             {
//                 int maxRounds = GameLobbyManager.Instance != null ? GameLobbyManager.Instance.RoundCount : 3;
//                 inGameUIController.UpdateRound(current, maxRounds);
//             }
//         }
//         #endregion

//         #region Cleanup & Transitions

//         private IEnumerator ReturnToLobbyRoutine(string errorMessage = null)
//         {
//             if (!string.IsNullOrEmpty(errorMessage))
//                 Debug.LogError($"Returning to lobby due to error: {errorMessage}");

//             CleanupGameResources();

//             if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
//             {
//                 Debug.Log("Shutting down network manager");
//                 NetworkManager.Singleton.Shutdown();
//                 yield return new WaitForSeconds(0.5f);
//             }

//             Debug.Log("Loading Loading scene");
//             AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
//             while (!loadOperation.isDone)
//                 yield return null;

//             Debug.Log("Loading scene loaded, finding loading panel");


//             LoadingPanelController loadingPanel = FindObjectOfType<LoadingPanelController>();
//             if (loadingPanel != null)
//             {
//                 if (string.IsNullOrEmpty(errorMessage))
//                     loadingPanel.StartLoading("Lobby", null, "Returning to lobby...");
//                 else
//                     loadingPanel.StartLoading("Lobby", null, $"Error: {errorMessage}");

//                 yield return new WaitForSeconds(1.0f);
//             }
//             else
//             {
//                 Debug.LogWarning("LoadingPanelController not found in Loading scene");
//                 yield return new WaitForSeconds(0.5f);
//             }

//             if (GameLobbyManager.Instance != null)
//             {
//                 Debug.Log("Using GameLobbyManager to return to lobby");
//                 yield return StartCoroutine(ReturnToLobbyViaManager());
//             }
//             else
//             {
//                 Debug.Log("GameLobbyManager not found, loading lobby scene directly");
//                 SceneManager.LoadSceneAsync("Lobby");
//             }
//         }

//         private IEnumerator ReturnToLobbyViaManager()
//         {
//             var task = GameLobbyManager.Instance.ReturnToLobby();
//             while (!task.IsCompleted)
//                 yield return null;

//             if (task.IsFaulted)
//             {
//                 Debug.LogError($"ReturnToLobby task failed: {task.Exception}");
//                 SceneManager.LoadSceneAsync("Lobby");
//             }
//         }

//         private void CleanupGameResources()
//         {
//             if (inGameUIController != null)
//             {
//                 Destroy(inGameUIController.gameObject);
//                 inGameUIController = null;
//             }

//             playerTeams.Clear();
//             isCountingDown = false;
//             canCheckPositions = false;
//             isGameInitialized = false;
//             isUICreated = false;

//             Debug.Log("Game resources cleaned up");
//         }
//         #endregion
//     }
// }