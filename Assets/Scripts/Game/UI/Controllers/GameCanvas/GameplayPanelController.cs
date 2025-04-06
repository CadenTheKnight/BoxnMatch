using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas
{
    public class GameplayPanelController : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private GameObject gameplayScreen;
        [SerializeField] private GameObject roundEndScreen;
        [SerializeField] private GameObject gameEndScreen;
        [SerializeField] private GameObject roundStartingScreen;
        [SerializeField] private GameObject returnToLobbyScreen;


        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI team1ScoreText;
        [SerializeField] private TextMeshProUGUI team2ScoreText;
        [SerializeField] private TextMeshProUGUI gameWinnerText;
        [SerializeField] private TextMeshProUGUI roundWinnerText;
        [SerializeField] private TextMeshProUGUI team1EndScoreText;
        [SerializeField] private TextMeshProUGUI team2EndScoreText;
        [SerializeField] private TextMeshProUGUI gameEndCountdownText;
        [SerializeField] private TextMeshProUGUI roundStartingCountdownText;


        private void Awake()
        {
            HideAllScreens();
            UpdateScores(0, 0);
        }

        #region Screen Management

        public void HideAllScreens()
        {
            roundStartingScreen.SetActive(false);
            gameplayScreen.SetActive(false);
            roundEndScreen.SetActive(false);
            gameEndScreen.SetActive(false);
            returnToLobbyScreen.SetActive(false);
        }

        public void ShowRoundStartingScreen(int currentRound, int maxRounds)
        {
            HideAllScreens();
            roundStartingScreen.SetActive(true);
            roundText.text = $"Round {currentRound}/{maxRounds}";
        }

        public void ShowGameplayScreen(bool isHost)
        {
            HideAllScreens();
            gameplayScreen.SetActive(true);
        }

        public void ShowRoundEndScreen(int winningTeam)
        {
            HideAllScreens();
            roundEndScreen.SetActive(true);
            roundWinnerText.text = $"Team {winningTeam} wins the round!";
        }

        public void ShowGameEndScreen(int winningTeam, int team1Score, int team2Score)
        {
            HideAllScreens();
            gameEndScreen.SetActive(true);
            gameWinnerText.text = $"Team {winningTeam} wins the game!";
            team1EndScoreText.text = $"{team1Score}";
            team2EndScoreText.text = $"{team2Score}";
        }

        public void ShowReturnToLobbyScreen()
        {
            HideAllScreens();
            returnToLobbyScreen.SetActive(true);
        }

        #endregion

        #region UI Updates

        public void UpdateCountdown(int seconds)
        {
            string countdownString = seconds.ToString();

            if (roundStartingScreen.activeSelf)
                roundStartingCountdownText.text = countdownString;
            else if (gameEndScreen.activeSelf)
                gameEndCountdownText.text = countdownString;
        }

        public void UpdateScores(int team1Score, int team2Score)
        {
            if (team1ScoreText != null && team2ScoreText != null)
            {
                team1ScoreText.text = $"{team1Score}";
                team2ScoreText.text = $"{team2Score}";
            }
        }

        public void UpdateRound(int currentRound, int maxRounds)
        {
            roundText.text = $"Round {currentRound}/{maxRounds}";
        }

        #endregion
    }
}