using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Controllers.GameCanvas.GameStates;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas
{
    public class GamePanelController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private RoundStartingPanelController roundStartingPanelController;
        [SerializeField] private RoundInProgressPanelController roundInProgressPanelController;
        [SerializeField] private RoundEndingPanelController roundEndingPanelController;
        [SerializeField] private GameEndingPanelController gameEndingPanelController;

        private int rounds;
        private float roundTimeSeconds;
        private GameMode gameMode;

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            HideAllPanels();
            switch (newState)
            {
                case GameState.RoundStarting:
                    ShowRoundStarting();
                    break;

                case GameState.RoundInProgress:
                    ShowRoundInProgress();
                    break;

                case GameState.RoundEnding:
                    UpdateScores(GameManager.Instance.OneScore, GameManager.Instance.TwoScore);
                    ShowRoundEnding();
                    break;
            }
        }

        public void Initialize(int rounds, float roundTimeSeconds, GameMode gameMode)
        {
            this.rounds = rounds;
            this.roundTimeSeconds = roundTimeSeconds;
            this.gameMode = gameMode;

            UpdateScores(0, 0);
            GameManager.Instance.ChangeGameState(GameState.RoundStarting);
        }

        private void HideAllPanels()
        {
            roundStartingPanelController.gameObject.SetActive(false);
            roundInProgressPanelController.gameObject.SetActive(false);
            roundEndingPanelController.gameObject.SetActive(false);
            gameEndingPanelController.gameObject.SetActive(false);
        }

        private void ShowRoundStarting()
        {
            roundStartingPanelController.gameObject.SetActive(true);
            roundStartingPanelController.StartRound(GameManager.Instance.CurrentRound, rounds);

            GameManager.Instance.SetPlayerControlsEnabled(false);

            StartCoroutine(CountdownSequence());
        }

        private IEnumerator CountdownSequence()
        {
            roundStartingPanelController.SetCountdownValue(3);
            yield return new WaitForSeconds(1f);

            roundStartingPanelController.SetCountdownValue(2);
            yield return new WaitForSeconds(1f);

            roundStartingPanelController.SetCountdownValue(1);
            yield return new WaitForSeconds(1f);

            GameManager.Instance.ChangeGameState(GameState.RoundInProgress);
        }

        private void ShowRoundInProgress()
        {
            roundInProgressPanelController.gameObject.SetActive(true);
            GameManager.Instance.SetPlayerControlsEnabled(true);

            StartCoroutine(RoundTimer());
        }

        private IEnumerator RoundTimer()
        {
            float timeLeft = roundTimeSeconds;
            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                roundInProgressPanelController.UpdateTimer(timeLeft);
                yield return null;
            }

            GameManager.Instance.RoundTimeExpired();
        }

        private void ShowRoundEnding()
        {
            roundEndingPanelController.gameObject.SetActive(true);
            GameManager.Instance.SetPlayerControlsEnabled(false);

            bool player1Won = GameManager.Instance.OneScore > GameManager.Instance.TwoScore;
            string winnerText = player1Won ? "PLAYER 1 WINS!" : (gameMode == GameMode.AI ? "CPU WINS!" : "PLAYER 2 WINS!");
            roundEndingPanelController.ShowResults(winnerText);

            StartCoroutine(EndOfRoundDelay());
        }

        private IEnumerator EndOfRoundDelay()
        {
            yield return new WaitForSeconds(3f);

            if (GameManager.Instance.CurrentRound >= rounds || GameManager.Instance.OneScore > rounds / 2 || GameManager.Instance.TwoScore > rounds / 2)
                ShowGameEnding();
            else
                GameManager.Instance.ChangeGameState(GameState.RoundStarting);
        }

        private void ShowGameEnding()
        {
            HideAllPanels();
            gameEndingPanelController.gameObject.SetActive(true);
            GameManager.Instance.SetPlayerControlsEnabled(false);

            bool player1Won = GameManager.Instance.OneScore > GameManager.Instance.TwoScore;
            string winnerText = player1Won ? "PLAYER 1 WINS THE MATCH!" : (gameMode == GameMode.AI ? "CPU WINS THE MATCH!" : "PLAYER 2 WINS THE MATCH!");
            gameEndingPanelController.ShowResults(winnerText, GameManager.Instance.OneScore, GameManager.Instance.TwoScore);

            StartCoroutine(ReturnToMainMenu());
        }

        private IEnumerator ReturnToMainMenu()
        {
            yield return new WaitForSeconds(5f);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }

        public void UpdateScores(int playerOneScore, int playerTwoScore)
        {
            roundInProgressPanelController.UpdateScores(playerOneScore, playerTwoScore);
        }
    }
}