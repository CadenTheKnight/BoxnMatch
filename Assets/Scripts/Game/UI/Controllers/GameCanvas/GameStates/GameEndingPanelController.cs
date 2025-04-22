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
        [SerializeField] private Button mainMenuButton;

        private void OnEnable()
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        private void OnDisable()
        {
            mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
        }

        public void ShowResults(string winner, int player1Score, int player2Score)
        {
            winnerText.text = winner;
            finalScoreText.text = $"FINAL SCORE: {player1Score} - {player2Score}";
        }

        public void ReturnToMainMenu()
        {
            Application.Quit();
        }
    }
}