using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Colors;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas.GameStates
{
    public class RoundInProgressPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI oneScoreText;
        [SerializeField] private TextMeshProUGUI twoScoreText;
        [SerializeField] private Button pauseButton;

        private void OnEnable()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        private void OnDisable()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }

        private void OnPauseButtonClicked()
        {
            // GameManager.Instance.PauseGame();
        }

        public void StartRound(int currentRound, int totalRounds)
        {
            UpdateScores(0, 0);
        }

        public void UpdateTimer(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (timeRemaining <= 10f)
                timerText.color = Color.Lerp(UIColors.Red.One, UIColors.Primary.One, Mathf.PingPong(Time.time * 2f, 1f));
            else
                timerText.color = UIColors.Primary.One;
        }

        public void UpdateScores(int player1Score, int player2Score)
        {
            oneScoreText.text = player1Score.ToString();
            twoScoreText.text = player2Score.ToString();
        }

    }
}