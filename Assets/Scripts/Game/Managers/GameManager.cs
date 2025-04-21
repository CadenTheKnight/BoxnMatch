using System;
using UnityEngine;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Game.UI.Controllers.GameCanvas;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private MapSelectionData mapSelectionData;
        [SerializeField] private GamePanelController gamePanelController;

        private GameState gameState;
        private Map map;
        private int rounds;
        private float roundTimeSeconds;
        private GameMode gameMode;

        private int currentRound;
        private int oneScore;
        private int twoScore;

        public int OneScore => oneScore;
        public int TwoScore => twoScore;

        public event Action<GameState> OnGameStateChanged;
        public event Action<int, int> OnRoundChanged;
        public event Action<int, int> OnScoreChanged;

        public void StartGame(int mapIndex, int rounds, int roundTimeSeconds, GameMode gameMode)
        {
            if (showDebugLogs) Debug.Log($"Starting game: Map {mapIndex}, {rounds} rounds, {roundTimeSeconds}s per round, {gameMode}");

            map = mapSelectionData.GetMap(mapIndex);
            this.rounds = rounds;
            this.roundTimeSeconds = roundTimeSeconds;
            this.gameMode = gameMode;

            currentRound = 0;
            oneScore = 0;
            twoScore = 0;

            SceneManager.LoadSceneAsync(map.SceneName);

            gamePanelController.Initialize(map, rounds, roundTimeSeconds, gameMode);

            if (gameMode == GameMode.AI) EnableAI();
            ChangeGameState(GameState.RoundStarting);
        }

        private void EnableAI()
        {
            var player2 = GameObject.Find("Player2");
            if (player2 != null)
            {
                if (player2.TryGetComponent<CPUController>(out var aiController))
                    aiController.enabled = true;

                if (player2.TryGetComponent<playerControllerPlayer2>(out var player2Controller))
                    player2Controller.enabled = false;
            }
        }

        public void ChangeGameState(GameState newState)
        {
            gameState = newState;
            OnGameStateChanged?.Invoke(newState);

            if (showDebugLogs) Debug.Log($"Game state changed to: {newState}");

            switch (newState)
            {
                case GameState.RoundInProgress:
                    ResetPlayersForRound();
                    break;
            }
        }

        private void ResetPlayersForRound()
        {
            var player1 = GameObject.Find("Player1");
            var player2 = GameObject.Find("Player2");

            if (player1 != null)
            {
                player1.transform.position = new Vector3(-3, 2, 0);
                if (player1.TryGetComponent<Rigidbody2D>(out var rb1)) rb1.velocity = Vector2.zero;
            }

            if (player2 != null)
            {
                player2.transform.position = new Vector3(3, 2, 0);
                if (player2.TryGetComponent<Rigidbody2D>(out var rb2)) rb2.velocity = Vector2.zero;
            }
        }

        public void PlayerEliminated(int playerNumber)
        {
            if (gameState != GameState.RoundInProgress) return;

            if (playerNumber == 1) twoScore++;
            else oneScore++;

            OnScoreChanged?.Invoke(oneScore, twoScore);
            gamePanelController.UpdateScores(oneScore, twoScore);
            ChangeGameState(GameState.RoundEnding);
        }

        public void RoundTimeExpired()
        {
            if (gameState != GameState.RoundInProgress) return;

            DetermineRoundWinner();
        }

        private void DetermineRoundWinner()
        {
            var player1 = GameObject.Find("Player1");
            var player2 = GameObject.Find("Player2");

            if (player1 != null && player2 != null)
                // compare player %

                OnScoreChanged?.Invoke(oneScore, twoScore);
            gamePanelController.UpdateScores(oneScore, twoScore);
            ChangeGameState(GameState.RoundEnding);
        }
    }
}