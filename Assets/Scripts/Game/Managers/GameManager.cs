using System;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using System.Collections;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : NetworkSingleton<GameManager>
    {
        [SerializeField] private bool showDebugLogs = true;

        private GameState currentGameState;
        private int currentRound;
        private int playerOneScore;
        private int playerTwoScore;

        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnRoundChanged;
        public event Action<int, int> OnScoreChanged;
        public event Action<float> OnTimerUpdated;

        private NetworkVariable<int> netCurrentRound = new(0);
        private NetworkVariable<GameState> netGameState = new(GameState.RoundStarting);
        private NetworkVariable<float> netStateTimer = new(0);

        public async Task StartGame(GameMode gameMode)
        {
            if (gameMode == GameMode.PvP)
            {
                GameObject relayManagerObject = new("RelayManager");
                relayManagerObject.AddComponent<RelayManager>();
                if (AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId)
                {
                    Task<string> createTask = RelayManager.Instance.CreateRelay(GameLobbyManager.Instance.Lobby.MaxPlayers);
                    string relayJoinCode = await createTask;
                    if (relayJoinCode != null) GameEvents.InvokeGameStarted(true, relayJoinCode);
                    else GameEvents.InvokeGameStarted(false, null);
                }
            }
            else GameEvents.InvokeGameStarted(true, null);
        }

        public async Task JoinGame(string relayJoinCode)
        {
            await RelayManager.Instance.JoinRelay(relayJoinCode);
        }


        public override void OnNetworkSpawn()
        {
            netGameState.OnValueChanged += OnNetGameStateChanged;
            netCurrentRound.OnValueChanged += OnNetRoundChanged;

            if (IsHost || IsServer) InitializeGame();
        }

        private void OnNetGameStateChanged(GameState previousState, GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        private void OnNetRoundChanged(int previousRound, int newRound)
        {
            currentRound = newRound;
            OnRoundChanged?.Invoke(newRound);
        }

        [ServerRpc(RequireOwnership = false)]
        public void InitializeGameServerRpc()
        {
            if (!IsHost && !IsServer) return;
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (!IsHost && !IsServer) return;

            currentRound = 0;
            playerOneScore = 0;
            playerTwoScore = 0;

            netCurrentRound.Value = 0;
            netGameState.Value = GameState.RoundStarting;
        }

        public void ChangeGameState(GameState newState)
        {
            if (!IsHost && !IsServer) return;

            switch (newState)
            {
                case GameState.RoundStarting:
                    StartNewRound();
                    break;

                case GameState.RoundInProgress:
                    StartRoundPlay();
                    break;

                case GameState.RoundEnding:
                    EndCurrentRound();
                    break;
            }

            netGameState.Value = newState;
        }

        private void StartNewRound()
        {
            if (!IsHost && !IsServer) return;

            currentRound++;
            netCurrentRound.Value = currentRound;
            netStateTimer.Value = 3f;
            StartCoroutine(CountdownToRoundStart());
        }

        private IEnumerator CountdownToRoundStart()
        {
            float countdownTime = 3f;

            while (countdownTime > 0)
            {
                netStateTimer.Value = countdownTime;
                OnTimerUpdated?.Invoke(countdownTime);

                yield return new WaitForSeconds(1f);
                countdownTime -= 1f;
            }

            ChangeGameState(GameState.RoundInProgress);
        }

        private void StartRoundPlay()
        {
            if (!IsHost && !IsServer) return;

            netStateTimer.Value = int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value);
            StartCoroutine(RoundPlayTimer());
        }

        private IEnumerator RoundPlayTimer()
        {
            float timeRemaining = int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value);

            while (timeRemaining > 0)
            {
                netStateTimer.Value = timeRemaining;
                OnTimerUpdated?.Invoke(timeRemaining);

                yield return new WaitForSeconds(0.1f);
                timeRemaining -= 0.1f;
            }

            DetermineRoundWinner();
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayerFellOffServerRpc(ulong playerId)
        {
            if (playerId == 0) AddScore(2);
            else AddScore(1);

            StopAllCoroutines();
            ChangeGameState(GameState.RoundEnding);
        }

        private void DetermineRoundWinner()
        {
            if (currentRound % 2 == 1) AddScore(1);
            else AddScore(2);

            ChangeGameState(GameState.RoundEnding);
        }

        private void AddScore(int playerNumber)
        {
            if (playerNumber == 1) playerOneScore++;
            else playerTwoScore++;

            OnScoreChanged?.Invoke(playerOneScore, playerTwoScore);
        }

        private void EndCurrentRound()
        {
            if (!IsHost && !IsServer) return;

            StartCoroutine(DelayBeforeNextRound());
        }

        private IEnumerator DelayBeforeNextRound()
        {
            yield return new WaitForSeconds(2f);

            if (currentRound >= int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value)
                || playerOneScore > int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value) / 2
                || playerTwoScore > int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value) / 2)
                EndGame();
            else ChangeGameState(GameState.RoundStarting);
        }

        private void EndGame()
        {

            // Return to lobby after a short delay
            StartCoroutine(DelayedReturnToLobby());
        }

        private IEnumerator DelayedReturnToLobby()
        {
            yield return new WaitForSeconds(5f);

            if (IsHost)
            {
                // GameLobbyManager.Instance.EndGame();
            }
        }
    }
}