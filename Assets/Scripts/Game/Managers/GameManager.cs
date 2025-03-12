using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Controllers.GameplayMenu;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : NetworkBehaviour
    {
        #region Singleton & Events
        public static GameManager Instance { get; private set; }
        public delegate void GameStateChangedHandler(GameState previous, GameState current);
        public event GameStateChangedHandler GameStateChanged;
        #endregion

        #region Constants & Enums
        private const float DEATH_HEIGHT = -5f;

        public enum GameState
        {
            RoundStarting,
            RoundInProgress,
            RoundEnding,
            GameEnding,
            ReturnToLobby
        }
        #endregion

        #region Serialized Fields
        [SerializeField] private GameObject inGameUI;
        [SerializeField] private float roundStartCountdown = 3.0f;
        [SerializeField] private float roundEndCountdown = 2.0f;
        [SerializeField] private float gameEndCountdown = 5.0f;
        #endregion

        #region Network Variables
        private NetworkVariable<GameState> gameState = new(GameState.RoundStarting);
        private NetworkVariable<int> currentRound = new(1);
        private NetworkVariable<int> team1Score = new(0);
        private NetworkVariable<int> team2Score = new(0);
        private NetworkVariable<int> lastRoundWinner = new(0);
        private NetworkVariable<float> countdownTimer = new(3.0f);
        #endregion

        #region Private Fields
        private Dictionary<ulong, int> playerTeams = new();
        private GameplayPanelController inGameUIController;
        private bool isCountingDown = false;
        private bool canCheckPositions = false;
        #endregion

        #region Properties
        public GameState CurrentGameState => gameState.Value;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            SetupNetworking();

            if (RelayManager.Instance.IsHost)
                StartAsHost();
            else
                StartAsClient();

            CreateInGameUI();
        }

        private void Update()
        {
            if (IsServer)
            {
                UpdateCountdown();

                if (gameState.Value == GameState.RoundInProgress)
                    CheckPlayerPositions();
            }
        }

        public override void OnDestroy()
        {
            CleanupGameResources();
            base.OnDestroy();
        }
        #endregion

        #region Network Setup

        private void SetupNetworking()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

            if (RelayManager.Instance.IsHost)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        private void StartAsHost()
        {
            try
            {
                (byte[] allocationId, byte[] key, byte[] connectionData, string ipAddress, int port) = RelayManager.Instance.GetHostConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipAddress, (ushort)port, allocationId, key, connectionData);
                NetworkManager.Singleton.StartHost();

                GetComponent<NetworkObject>().Spawn();

                countdownTimer.Value = roundStartCountdown;
                isCountingDown = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error starting host: {e.Message}");
                ReturnToLobbyAfterError("Failed to start as host");
            }
        }

        private void StartAsClient()
        {
            try
            {
                (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ipAddress, int port) = RelayManager.Instance.GetClientConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipAddress, (ushort)port, allocationId, key, connectionData, hostConnectionData);
                NetworkManager.Singleton.StartClient();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error starting client: {e.Message}");
                ReturnToLobbyAfterError("Failed to connect to host");
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"Network spawn - IsServer: {IsServer}, IsClient: {IsClient}");

            RegisterNetworkCallbacks();

            if (IsClient)
                UpdateUIState(gameState.Value);
        }

        private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = NetworkManager.Singleton.NetworkConfig.PlayerPrefab.GetComponent<NetworkObject>().PrefabIdHash;

            int teamNumber = (playerTeams.Count % 2) + 1;
            playerTeams[request.ClientNetworkId] = teamNumber;

            float xPos = teamNumber == 1 ? -3f : 3f;
            response.Position = new Vector3(xPos, 1f, 0);

            Debug.Log($"Client {request.ClientNetworkId} approved on team {teamNumber}");
        }
        #endregion

        #region Client Connection Handling

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer) return;

            Debug.Log($"Client connected: {clientId}");
            AssignPlayerToTeam(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!IsServer) return;

            Debug.Log($"Client disconnected: {clientId}");

            if (playerTeams.ContainsKey(clientId))
                playerTeams.Remove(clientId);

            if (gameState.Value == GameState.RoundInProgress && NetworkManager.Singleton.ConnectedClientsList.Count < 2)
                EndRound(playerTeams.First().Value);
        }

        private void AssignPlayerToTeam(ulong clientId)
        {
            int team1Count = playerTeams.Count(p => p.Value == 1);
            int team2Count = playerTeams.Count(p => p.Value == 2);

            int teamToAssign = team1Count <= team2Count ? 1 : 2;
            playerTeams[clientId] = teamToAssign;

            Debug.Log($"Player {clientId} assigned to Team {teamToAssign}");
        }
        #endregion

        #region Game Flow Control

        private void UpdateCountdown()
        {
            if (!isCountingDown) return;

            countdownTimer.Value -= Time.deltaTime;

            if (countdownTimer.Value <= 0f)
            {
                isCountingDown = false;
                AdvanceGameState();
            }
        }

        private void StartCountdown(float seconds)
        {
            countdownTimer.Value = seconds;
            isCountingDown = true;
        }

        private void AdvanceGameState()
        {
            if (!IsServer) return;

            switch (gameState.Value)
            {
                case GameState.RoundStarting:
                    StartRound();
                    break;

                case GameState.RoundEnding:
                    if (ShouldEndGame())
                        EndGame();
                    else
                        StartNextRound();
                    break;

                case GameState.GameEnding:
                    ReturnToLobby();
                    break;
            }
        }

        private void StartRound()
        {
            RespawnPlayers();
            gameState.Value = GameState.RoundInProgress;
            canCheckPositions = false;
            StartCoroutine(EnablePositionCheckingAfterDelay(0.5f));
        }

        private void EndRound(int winningTeam)
        {
            if (gameState.Value != GameState.RoundInProgress) return;

            lastRoundWinner.Value = winningTeam;
            gameState.Value = GameState.RoundEnding;
            StartCountdown(roundEndCountdown);
        }

        private void StartNextRound()
        {
            currentRound.Value++;
            gameState.Value = GameState.RoundStarting;
            StartCountdown(roundStartCountdown);
        }

        private void EndGame()
        {
            gameState.Value = GameState.GameEnding;
            StartCountdown(gameEndCountdown);
        }

        private bool ShouldEndGame()
        {
            int maxRounds = GameLobbyManager.Instance.RoundCount;

            int requiredWins = (maxRounds / 2) + 1;

            return currentRound.Value >= maxRounds ||
                   team1Score.Value >= requiredWins ||
                   team2Score.Value >= requiredWins;
        }
        #endregion

        #region Player Management

        private void RespawnPlayers()
        {
            if (!IsServer) return;

            Debug.Log("Respawning all players...");

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null) continue;

                int team = GetPlayerTeam(client.ClientId);
                float xPos = team == 1 ? -3f : 3f;
                Vector3 spawnPos = new(xPos, 1f, 0);

                ResetPlayerPhysics(client.PlayerObject, spawnPos);
                RespawnPlayerClientRpc(client.ClientId, spawnPos);
            }
        }

        private int GetPlayerTeam(ulong clientId)
        {
            if (playerTeams.TryGetValue(clientId, out int team))
                return team;

            AssignPlayerToTeam(clientId);
            return playerTeams[clientId];
        }

        private void ResetPlayerPhysics(NetworkObject playerObject, Vector3 position)
        {
            if (playerObject == null) return;

            playerObject.transform.position = position;

            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                StartCoroutine(UnfreezePlayerAfterDelay(rb, 0.5f));
            }
        }

        private IEnumerator UnfreezePlayerAfterDelay(Rigidbody2D rb, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        private IEnumerator EnablePositionCheckingAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            canCheckPositions = true;
        }

        private void CheckPlayerPositions()
        {
            if (!canCheckPositions) return;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject != null && client.PlayerObject.transform.position.y < DEATH_HEIGHT)
                {
                    if (playerTeams.TryGetValue(client.ClientId, out int team))
                    {
                        TeamWins(team == 1 ? 2 : 1);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Game Result Handling

        public void TeamWins(int teamNumber)
        {
            if (IsServer)
                ProcessTeamWin(teamNumber);
            else
                TeamWinsServerRpc(teamNumber);
        }

        private void ProcessTeamWin(int teamNumber)
        {
            if (gameState.Value != GameState.RoundInProgress) return;

            if (teamNumber == 1)
                team1Score.Value++;
            else if (teamNumber == 2)
                team2Score.Value++;

            EndRound(teamNumber);
        }

        private void ReturnToLobby()
        {
            if (IsServer)
                ReturnToLobbyClientRpc();
        }

        private void ReturnToLobbyAfterError(string errorMessage)
        {
            StartCoroutine(ReturnToLobbyRoutine(errorMessage));
        }
        #endregion

        #region ServerRPCs

        [ServerRpc(RequireOwnership = false)]
        private void TeamWinsServerRpc(int teamNumber)
        {
            ProcessTeamWin(teamNumber);
        }
        #endregion

        #region ClientRPCs

        [ClientRpc]
        private void RespawnPlayerClientRpc(ulong clientId, Vector3 position)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId) return;

            var playerObjects = FindObjectsOfType<NetworkObject>()
                .Where(netObj => netObj.IsPlayerObject && netObj.OwnerClientId == clientId);

            foreach (var playerObj in playerObjects)
            {
                playerObj.transform.position = position;

                Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0;
                }
            }
        }

        [ClientRpc]
        private void ReturnToLobbyClientRpc()
        {
            StartCoroutine(ReturnToLobbyRoutine());
        }
        #endregion

        #region UI State Management

        private void CreateInGameUI()
        {
            GameObject uiObject = Instantiate(inGameUI);
            inGameUIController = uiObject.GetComponent<GameplayPanelController>();

            if (inGameUIController != null)
                inGameUIController.ShowWaitingScreen();
            else
                Debug.LogError("Failed to get GameplayPanelController from UI prefab");
        }

        private void UpdateUIState(GameState state)
        {
            if (inGameUIController == null) return;

            switch (state)
            {
                case GameState.RoundStarting:
                    inGameUIController.ShowRoundStartingScreen(
                        currentRound.Value,
                        GameLobbyManager.Instance != null ? GameLobbyManager.Instance.RoundCount : 3);
                    break;

                case GameState.RoundInProgress:
                    inGameUIController.ShowGameplayScreen(NetworkManager.Singleton.IsHost);
                    break;

                case GameState.RoundEnding:
                    inGameUIController.ShowRoundEndScreen(lastRoundWinner.Value);
                    break;

                case GameState.GameEnding:
                    int gameWinner = team1Score.Value > team2Score.Value ? 1 : 2;
                    inGameUIController.ShowGameEndScreen(gameWinner, team1Score.Value, team2Score.Value);
                    break;

                case GameState.ReturnToLobby:
                    inGameUIController.ShowReturnToLobbyScreen();
                    break;
            }
        }

        private void RegisterNetworkCallbacks()
        {
            gameState.OnValueChanged += OnGameStateChanged;
            currentRound.OnValueChanged += OnRoundChanged;
            team1Score.OnValueChanged += OnTeam1ScoreChanged;
            team2Score.OnValueChanged += OnTeam2ScoreChanged;
            countdownTimer.OnValueChanged += OnCountdownChanged;
        }
        #endregion

        #region Network Variable Callbacks

        private void OnGameStateChanged(GameState previous, GameState current)
        {
            Debug.Log($"Game state changed from {previous} to {current}");
            GameStateChanged?.Invoke(previous, current);
            UpdateUIState(current);
        }

        private void OnCountdownChanged(float previous, float current)
        {
            if (inGameUIController != null)
                inGameUIController.UpdateCountdown(Mathf.CeilToInt(current));
        }

        private void OnTeam1ScoreChanged(int previous, int current)
        {
            if (inGameUIController != null)
                inGameUIController.UpdateScores(current, team2Score.Value);
        }

        private void OnTeam2ScoreChanged(int previous, int current)
        {
            if (inGameUIController != null)
                inGameUIController.UpdateScores(team1Score.Value, current);
        }

        private void OnRoundChanged(int previous, int current)
        {
            if (inGameUIController != null)
                inGameUIController.UpdateRound(current,
                    GameLobbyManager.Instance != null ? GameLobbyManager.Instance.RoundCount : 3);
        }
        #endregion

        #region Cleanup & Transitions

        private IEnumerator ReturnToLobbyRoutine(string errorMessage = null)
        {
            CleanupGameResources();

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
                yield return new WaitForSeconds(0.5f);
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Loading");
            yield return loadOperation;

            LoadingPanelController loadingPanel = FindObjectOfType<LoadingPanelController>();
            if (loadingPanel != null)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    loadingPanel.StartLoading(null, "Returning to Lobby", "Game ended");
                else
                    loadingPanel.StartLoading(null, "Returning to Lobby", errorMessage);
            }

            yield return new WaitForSeconds(1.0f);

            if (GameLobbyManager.Instance != null)
                yield return GameLobbyManager.Instance.ReturnToLobby();
            else
                SceneManager.LoadSceneAsync("Lobby");
        }

        private void CleanupGameResources()
        {
            if (inGameUIController != null)
            {
                Destroy(inGameUIController.gameObject);
                inGameUIController = null;
            }

            playerTeams.Clear();

            isCountingDown = false;
            canCheckPositions = false;
        }
        #endregion
    }
}