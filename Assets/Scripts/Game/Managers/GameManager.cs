using System;
using UnityEngine;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Game.UI.Controllers.GameCanvas;
using System.Collections;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private MapSelectionData mapSelectionData;
        [SerializeField] private GamePanelController gamePanelController;
        [SerializeField] private GameObject cpuPrefab;
        [SerializeField] private GameObject p2Prefab;

        private GameState gameState;
        private Map map;
        private int rounds;
        private float roundTimeSeconds;
        private GameMode gameMode;

        private Vector3 player1StartPosition;
        private Vector3 player2StartPosition;

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

            StartCoroutine(LoadGameSceneAsync());
        }

        private IEnumerator LoadGameSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(map.SceneName);

            while (!asyncLoad.isDone) yield return null;
            yield return null;

            player1StartPosition = GameObject.Find("Player-Couch-P1").transform.position;
            player2StartPosition = GameObject.Find("Player-Couch-P2").transform.position;

            EnableAI(gameMode == GameMode.AI);
            gamePanelController.Initialize(rounds, roundTimeSeconds, gameMode);
        }

        private void EnableAI(bool enable)
        {
            if (enable)
            {
                var player2 = GameObject.Find("Player-Couch-P2");
                DamageableObject player2d = player2.GetComponent<DamageableObject>();
                player2d.PermaDie();
                Destroy(player2);
            }
            else
            {
                var cpu = GameObject.Find("CPU");
                DamageableObject cpud = cpu.GetComponent<DamageableObject>();
                cpud.PermaDie();
                Destroy(cpu);
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
            var player1 = GameObject.Find("Player-Couch-P1");
            player1.transform.position = player1StartPosition;
            player1.transform.rotation = Quaternion.identity;
            player1.GetComponent<DamageableObject>().currentDamage = 0;

            if (gameMode == GameMode.AI)
            {
                CPURotator cpur = FindObjectOfType<CPURotator>();
                GameObject cpu;
                if(cpur == null)
                {
                    cpu = Instantiate(cpuPrefab);
                } else
                {
                    cpu = cpur.gameObject;
                }
                cpu.transform.position = player2StartPosition;
                cpu.transform.rotation = Quaternion.identity;
                cpu.GetComponent<DamageableObject>().currentDamage = 0;
            }
            else
            {
                var player2 = GameObject.Find("Player-Couch-P2");
                if(player2 == null)
                {
                    player2 = Instantiate(p2Prefab);
                }

                player2.transform.position = player2StartPosition;
                player2.transform.rotation = Quaternion.identity;
                player2.GetComponent<DamageableObject>().currentDamage = 0;
            }
        }

        public void PlayerEliminated(int playerNumber)
        {
            if (gameState != GameState.RoundInProgress) return;

            if (playerNumber == 1) twoScore++;
            else oneScore++;

            OnScoreChanged?.Invoke(oneScore, twoScore);
            gamePanelController.UpdateScores(oneScore, twoScore);
            Debug.Log("blah blah blah game ended");
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

            DamageableObject player1 = GameObject.Find("Player-Couch-P1").GetComponent<DamageableObject>();
            if (gameMode == GameMode.AI)
            {
                DamageableObject cpu = GameObject.Find("CPU").GetComponent<DamageableObject>();
                if (player1.currentDamage > cpu.currentDamage) twoScore++;
                else if (player1.currentDamage < cpu.currentDamage) oneScore++;
            }
            else
            {
                DamageableObject player2 = GameObject.Find("Player-Couch-P2").GetComponent<DamageableObject>();
                if (player1.currentDamage > player2.currentDamage) twoScore++;
                else if (player1.currentDamage < player2.currentDamage) oneScore++;
            }

            OnScoreChanged?.Invoke(oneScore, twoScore);
            gamePanelController.UpdateScores(oneScore, twoScore);
            ChangeGameState(GameState.RoundEnding);
        }

        public void SetPlayerControlsEnabled(bool enabled)
        {
            GameObject go = GameObject.Find("Player-Couch-P1");
            if(go != null)
            {
                playerController player1 = go.GetComponent<playerController>();
                if(player1 != null)
                {
                    if (enabled) player1.EnableInputs();
                    else player1.DisableInputs();
                }
            }
            
            if (gameMode == GameMode.AI)
            {
                GameObject go2 = GameObject.Find("CPU");
                if (go2 == null) return;
                CPUController cpu = go2.GetComponent<CPUController>();
                if (cpu == null) return;

                if (enabled) cpu.StartCPU();
                else cpu.StopCPU();
            }
            else
            {
                playerController player2 = GameObject.Find("Player-Couch-P2").GetComponent<playerController>();
                if (enabled) player2.EnableInputs();
                else player2.DisableInputs();
            }
        }
    }
}