using UnityEngine;
using Unity.Netcode;
using Assets.Scripts.Game.Enums;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages the game state and transitions between different states in the game.
    /// </summary>
    public class GameStateManager : NetworkBehaviour
    {
        #region Singleton
        public static GameStateManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
        #endregion

        #region Network Variables
        public NetworkVariable<GameState> gameState = new(GameState.RoundStarting);

        public NetworkVariable<int> currentRound = new(1);
        public NetworkVariable<int> leftTeamScore = new(0);
        public NetworkVariable<int> rightTeamScore = new(0);
        public NetworkVariable<int> lastRoundWinner = new(0);

        public NetworkVariable<float> countdownTimer = new(3.0f);
        public NetworkVariable<bool> isCountingDown = new(false);
        #endregion

        #region Configuration
        [SerializeField] private float roundStartCountdown = 3.0f;
        [SerializeField] private float roundEndCountdown = 2.0f;
        [SerializeField] private float gameEndCountdown = 5.0f;
        #endregion

        #region Events

        #endregion

        #region Lifecycle Methods
        private void Start()
        {
            gameState.OnValueChanged += (oldState, newState) =>
            {
                Debug.Log($"Game State changed: {oldState} -> {newState}");
                GameEvents.InvokeGameStateChanged(oldState, newState);
            };

            currentRound.OnValueChanged += (oldRound, newRound) =>
            {
                Debug.Log($"Round changed: {oldRound} -> {newRound}");
                GameEvents.InvokeRoundChanged(oldRound, newRound);
            };

            leftTeamScore.OnValueChanged += (oldScore, newScore) =>
            {
                Debug.Log($"Left team score changed: {oldScore} -> {newScore}");
                GameEvents.InvokeScoreChanged(newScore, rightTeamScore.Value);
            };

            rightTeamScore.OnValueChanged += (oldScore, newScore) =>
            {
                Debug.Log($"Right team score changed: {oldScore} -> {newScore}");
                GameEvents.InvokeScoreChanged(leftTeamScore.Value, newScore);
            };

            countdownTimer.OnValueChanged += (oldTime, newTime) =>
            {
                Debug.Log($"Countdown updated: {newTime}");
                GameEvents.InvokeCountdownUpdated(newTime);
            };
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"GameStateManager.OnNetworkSpawn - IsServer: {IsServer}, IsClient: {IsClient}");
        }

        private void Update()
        {
            if (isCountingDown.Value)
                UpdateCountdown();
        }
        #endregion

        #region State Management
        public void InitializeGameState()
        {
            if (!IsServer) return;

            Debug.Log("Initializing game state");
            gameState.Value = GameState.RoundStarting;
            currentRound.Value = 1;
            leftTeamScore.Value = 0;
            rightTeamScore.Value = 0;
            StartCountdown(roundStartCountdown);
        }

        public void StartCountdown(float duration)
        {
            countdownTimer.Value = duration;
            isCountingDown.Value = true;
            Debug.Log($"Starting countdown: {duration} seconds");
        }

        private void UpdateCountdown()
        {
            countdownTimer.Value -= Time.deltaTime;

            if (countdownTimer.Value <= 0f)
            {
                isCountingDown.Value = false;
                AdvanceGameState();
            }
        }

        private void AdvanceGameState()
        {
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
                    RequestReturnToLobby();
                    break;
            }
        }

        private void StartRound()
        {
            gameState.Value = GameState.RoundInProgress;
            Debug.Log($"Round {currentRound.Value} started");
        }

        public void EndRound(int winningTeam)
        {
            lastRoundWinner.Value = winningTeam;

            if (winningTeam == 0)
                leftTeamScore.Value++;
            else if (winningTeam == 1)
                rightTeamScore.Value++;

            gameState.Value = GameState.RoundEnding;
            StartCountdown(roundEndCountdown);
            Debug.Log($"Round {currentRound.Value} ended. Team {winningTeam} wins. Scores: {leftTeamScore.Value}-{rightTeamScore.Value}");
        }

        private bool ShouldEndGame()
        {
            int maxRounds = LobbyManager.Instance.RoundCount;
            int requiredWins = (maxRounds / 2) + 1;

            if (leftTeamScore.Value >= requiredWins || rightTeamScore.Value >= requiredWins || currentRound.Value >= maxRounds)
                return true;

            return false;
        }

        private void StartNextRound()
        {
            currentRound.Value++;
            gameState.Value = GameState.RoundStarting;
            StartCountdown(roundStartCountdown);
            Debug.Log($"Starting Round {currentRound.Value}");
        }

        private void EndGame()
        {
            gameState.Value = GameState.GameEnding;
            StartCountdown(gameEndCountdown);

            if (leftTeamScore.Value > rightTeamScore.Value)
                Debug.Log($"Game ended. Left team wins with score {leftTeamScore.Value}-{rightTeamScore.Value}");
            else
                Debug.Log($"Game ended. Right team wins with score {rightTeamScore.Value}-{leftTeamScore.Value}");
        }

        private void RequestReturnToLobby()
        {
            if (!IsServer) return;

            gameState.Value = GameState.ReturnToLobby;
            Debug.Log("Returning to lobby");

            // GameManager.Instance.ReturnToLobby();
        }
        #endregion

        #region Remote State Change Requests
        public void RequestEndRound(int winningTeam)
        {
            if (IsServer) EndRound(winningTeam);
            else EndRoundServerRpc(winningTeam);
        }

        [ServerRpc(RequireOwnership = false)]
        private void EndRoundServerRpc(int winningTeam)
        {
            EndRound(winningTeam);
        }
        #endregion

        #region Public Accessor Methods
        public GameState GetGameState()
        {
            return gameState.Value;
        }

        public int GetCurrentRound()
        {
            return currentRound.Value;
        }

        public (int leftTeamScore, int rightTeamScore) GetScores()
        {
            return (leftTeamScore.Value, rightTeamScore.Value);
        }

        public float GetCountdownTime()
        {
            return countdownTimer.Value;
        }
        #endregion
    }
}