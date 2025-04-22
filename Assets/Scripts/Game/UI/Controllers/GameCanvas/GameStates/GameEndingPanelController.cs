using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas.GameStates
{
    public class GameEndingPanelController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI winnerText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        private void OnDisable()
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }

        public void ShowResults(string winner, int player1Score, int player2Score)
        {
            winnerText.text = winner;
            finalScoreText.text = $"FINAL SCORE: {player1Score} - {player2Score}";
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}