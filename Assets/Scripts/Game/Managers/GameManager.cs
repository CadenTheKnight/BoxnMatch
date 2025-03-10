using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameObject inGameUI;

        [SerializeField] private int maxRounds = 5; // temporary value until lobby set up with this setting
        private NetworkVariable<int> currentRound = new(1);
        private NetworkVariable<int> team1Score = new(0);
        private NetworkVariable<int> team2Score = new(0);
        private NetworkVariable<int> lastRoundWinner = new(0);
        private Dictionary<ulong, bool> clientReadyStatus = new();
        private NetworkVariable<float> gameEndCountdownTimer = new(5.0f);
        private NetworkVariable<float> roundStartingCountdownTimer = new(3.0f);
        private NetworkVariable<GameState> gameState = new(GameState.WaitingForPlayers);


        private InGameUIController inGameUIController;
        private bool isCountingDown = false;

        public enum GameState
        {
            WaitingForPlayers,
            RoundStarting,
            RoundInProgress,
            RoundEnding,
            GameEnding,
            ReturnToLobby
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

            if (RelayManager.Instance.IsHost)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
                (byte[] allocationId, byte[] key, byte[] connectionData, string ipAddress, int port) = RelayManager.Instance.GetHostConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipAddress, (ushort)port, allocationId, key, connectionData, true);
                NetworkManager.Singleton.StartHost();

                NetworkObject netObj = GetComponent<NetworkObject>();
                if (!netObj.IsSpawned)
                    netObj.Spawn();
            }
            else
            {
                (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ipAddress, int port) = RelayManager.Instance.GetClientConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipAddress, (ushort)port, allocationId, key, connectionData, hostConnectionData, true);
                NetworkManager.Singleton.StartClient();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log($"OnNetworkSpawn called - IsClient: {IsClient}, IsServer: {IsServer}, IsHost: {IsHost}");

            if (IsClient)
            {
                if (inGameUIController == null)
                {
                    GameObject uiObject = Instantiate(inGameUI);
                    inGameUIController = uiObject.GetComponent<InGameUIController>();
                    inGameUIController.ShowWaitingScreen();

                    gameState.OnValueChanged += OnGameStateChanged;
                    currentRound.OnValueChanged += OnRoundChanged;
                    team1Score.OnValueChanged += OnTeam1ScoreChanged;
                    team2Score.OnValueChanged += OnTeam2ScoreChanged;
                    roundStartingCountdownTimer.OnValueChanged += OnCountdownChanged;
                    gameEndCountdownTimer.OnValueChanged += OnCountdownChanged;
                }

                NotifyClientReadyServerRpc(NetworkManager.Singleton.LocalClientId);
            }

            if (IsServer)
            {
                clientReadyStatus[NetworkManager.Singleton.LocalClientId] = true;

                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

                gameState.Value = GameState.WaitingForPlayers;
            }
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Client connected: {clientId}");
            if (IsServer)
            {
                if (!clientReadyStatus.ContainsKey(clientId))
                    clientReadyStatus[clientId] = false;
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"Client disconnected: {clientId}");
            if (IsServer && clientReadyStatus.ContainsKey(clientId))
                clientReadyStatus.Remove(clientId);
        }

        private void Update()
        {
            if (IsServer && isCountingDown)
            {
                if (gameState.Value == GameState.RoundStarting)
                {
                    roundStartingCountdownTimer.Value -= Time.deltaTime;

                    if (roundStartingCountdownTimer.Value <= 0)
                    {
                        isCountingDown = false;
                        AdvanceGameState();
                    }
                }
                else if (gameState.Value == GameState.GameEnding || gameState.Value == GameState.RoundEnding)
                {
                    gameEndCountdownTimer.Value -= Time.deltaTime;

                    if (gameEndCountdownTimer.Value <= 0)
                    {
                        isCountingDown = false;
                        AdvanceGameState();
                    }
                }
            }
        }

        private void CheckAllClientsReady()
        {
            if (!IsServer) return;

            var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

            bool allReady = true;
            foreach (var client in connectedClients)
                if (!clientReadyStatus.TryGetValue(client.ClientId, out bool isReady) || !isReady)
                {
                    allReady = false;
                    break;
                }

            if (allReady && gameState.Value == GameState.WaitingForPlayers)
                AdvanceGameState();
        }

        private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Pending = false;

            // temp spawn positions
            response.Position = new Vector3(NetworkManager.Singleton.ConnectedClientsList.Count * 2.0f, 1, 0);
        }

        #region Game Flow Methods

        private void StartCountdown(int seconds)
        {
            if (!IsServer) return;

            switch (gameState.Value)
            {
                case GameState.RoundStarting:
                    roundStartingCountdownTimer.Value = seconds;
                    break;
                case GameState.RoundEnding:
                    gameEndCountdownTimer.Value = seconds;
                    break;
                case GameState.GameEnding:
                    gameEndCountdownTimer.Value = seconds;
                    break;
            }

            isCountingDown = true;
        }

        private void AdvanceGameState()
        {
            if (!IsServer) return;

            switch (gameState.Value)
            {
                case GameState.WaitingForPlayers:
                    gameState.Value = GameState.RoundStarting;
                    StartCountdown(3);
                    break;

                case GameState.RoundStarting:
                    gameState.Value = GameState.RoundInProgress;
                    break;

                case GameState.RoundEnding:
                    if (currentRound.Value >= maxRounds || team1Score.Value >= (maxRounds / 2) + 1 || team2Score.Value >= (maxRounds / 2) + 1)
                    {
                        gameState.Value = GameState.GameEnding;
                        StartCountdown(5);
                    }
                    else
                    {
                        currentRound.Value++;
                        gameState.Value = GameState.RoundStarting;
                        StartCountdown(3);
                    }
                    break;

                case GameState.GameEnding:
                    gameState.Value = GameState.ReturnToLobby;
                    ReturnToLobby();
                    break;
            }
        }

        public void Team1Wins()
        {
            if (IsServer && gameState.Value == GameState.RoundInProgress)
            {
                team1Score.Value++;
                EndRound(1);
            }
            else if (IsClient && NetworkManager.Singleton.IsHost)
                Team1WinsServerRpc();
        }

        public void Team2Wins()
        {
            if (IsServer && gameState.Value == GameState.RoundInProgress)
            {
                team2Score.Value++;
                EndRound(2);
            }
            else if (IsClient && NetworkManager.Singleton.IsHost)
                Team2WinsServerRpc();
        }

        private void EndRound(int winningTeam)
        {
            if (!IsServer) return;

            lastRoundWinner.Value = winningTeam;

            gameState.Value = GameState.RoundEnding;
            StartCountdown(2);
        }

        private void ReturnToLobby()
        {
            if (IsServer)
                ReturnToLobbyClientRpc();
        }

        #endregion

        #region ServerRPCs

        [ServerRpc(RequireOwnership = false)]
        private void Team1WinsServerRpc()
        {
            if (gameState.Value == GameState.RoundInProgress)
            {
                team1Score.Value++;
                EndRound(1);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void Team2WinsServerRpc()
        {
            if (gameState.Value == GameState.RoundInProgress)
            {
                team2Score.Value++;
                EndRound(2);
            }
        }


        [ServerRpc(RequireOwnership = false)]
        private void NotifyClientReadyServerRpc(ulong clientId)
        {
            Debug.Log($"Client {clientId} is ready");
            clientReadyStatus[clientId] = true;
            CheckAllClientsReady();
        }

        #endregion

        #region ClientRPCs

        [ClientRpc]
        private void ReturnToLobbyClientRpc()
        {
            if (inGameUIController != null)
            {
                Destroy(inGameUIController.gameObject);
                inGameUIController = null;
            }

            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Lobby");
            GameLobbyManager.Instance.SetAllPlayersUnready();
        }

        #endregion

        #region Network Variable Callbacks

        private void OnGameStateChanged(GameState previous, GameState current)
        {
            UpdateUIState(current);
        }

        private void OnCountdownChanged(float previous, float current)
        {
            inGameUIController.UpdateCountdown(Mathf.CeilToInt(current));
        }

        private void OnTeam1ScoreChanged(int previous, int current)
        {
            inGameUIController.UpdateScores(current, team2Score.Value);
        }

        private void OnTeam2ScoreChanged(int previous, int current)
        {
            inGameUIController.UpdateScores(team1Score.Value, current);
        }

        private void OnRoundChanged(int previous, int current)
        {
            inGameUIController.UpdateRound(current, maxRounds);
        }

        private void UpdateUIState(GameState state)
        {
            if (inGameUIController == null) return;

            switch (state)
            {
                case GameState.WaitingForPlayers:
                    inGameUIController.ShowWaitingScreen();
                    break;

                case GameState.RoundStarting:
                    inGameUIController.ShowRoundStartingScreen(currentRound.Value, maxRounds);
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

        #endregion
    }
}