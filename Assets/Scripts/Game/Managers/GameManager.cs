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

        public int CurrentRound => currentRound;
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

            currentRound = 1;
            oneScore = 0;
            twoScore = 0;

            SceneManager.LoadSceneAsync(map.SceneName);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (scene.name == map.SceneName)
            {
                gamePanelController.Initialize(rounds, roundTimeSeconds, gameMode);

                if (gameMode == GameMode.AI) EnableAI();
                ChangeGameState(GameState.RoundStarting);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            currentRound++;
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
            {
                var damageObj1 = player1.GetComponent<DamageableObject>();
                var damageObj2 = player2.GetComponent<DamageableObject>();

                if (damageObj1 != null && damageObj2 != null)
                {
                    float player1DamagePercentage = damageObj1.currentDamage;
                    float player2DamagePercentage = damageObj2.currentDamage;

                    Debug.Log($"Round ended - P1 damage: {player1DamagePercentage}%, P2 damage: {player2DamagePercentage}%");

                    if (player1DamagePercentage < player2DamagePercentage)
                    {
                        oneScore++;
                        Debug.Log("Player 1 wins the round!");
                    }
                    else if (player2DamagePercentage < player1DamagePercentage)
                    {
                        twoScore++;
                        Debug.Log("Player 2 wins the round!");
                    }
                    else
                    {
                        oneScore++;
                        twoScore++;
                        Debug.Log("Round ended in a tie!");
                    }
                }
            }

            OnScoreChanged?.Invoke(oneScore, twoScore);
            gamePanelController.UpdateScores(oneScore, twoScore);
            ChangeGameState(GameState.RoundEnding);
        }

        public void SetPlayerControlsEnabled(bool enabled)
        {
            var player1 = GameObject.FindWithTag("Player");
            if (player1 != null)
            {
                Debug.Log($"Found Player1: {player1.name}");

                if (player1.TryGetComponent<playerControllerPlayer1>(out var p1Controller))
                    p1Controller.enabled = enabled;

                if (player1.TryGetComponent<playerController>(out var pController))
                    pController.enabled = enabled;

                if (player1.TryGetComponent<PlayerInputManager>(out var input))
                {
                    if (enabled)
                        input.SendMessage("EnableInputs", null, SendMessageOptions.DontRequireReceiver);
                    else
                        input.SendMessage("DisableInputs", null, SendMessageOptions.DontRequireReceiver);
                }

                if (player1.TryGetComponent<PlayerRotator>(out var rotator))
                    rotator.enabled = enabled;
            }
            else
            {
                Debug.LogWarning("Player 1 not found with tag 'Player'");
            }

            var player2 = GameObject.FindWithTag("Player2");
            if (player2 != null)
            {
                Debug.Log($"Found Player2: {player2.name}");

                if (gameMode == GameMode.AI)
                {
                    if (player2.TryGetComponent<CPUController>(out var cpuController))
                        cpuController.enabled = enabled;
                }
                else
                {
                    if (player2.TryGetComponent<playerControllerPlayer2>(out var p2Controller))
                        p2Controller.enabled = enabled;

                    if (player2.TryGetComponent<playerController>(out var pController))
                        pController.enabled = enabled;

                    if (player2.TryGetComponent<PlayerInputManager>(out var input))
                    {
                        if (enabled)
                            input.SendMessage("EnableInputs", null, SendMessageOptions.DontRequireReceiver);
                        else
                            input.SendMessage("DisableInputs", null, SendMessageOptions.DontRequireReceiver);
                    }

                    if (player2.TryGetComponent<PlayerRotator>(out var rotator))
                        rotator.enabled = enabled;
                }
            }
            else
            {
                Debug.LogWarning("Player 2 not found with tag 'Player2'");
            }

            if (enabled)
                Debug.Log("Player controls enabled");
            else
                Debug.Log("Player controls disabled");
        }
    }
}