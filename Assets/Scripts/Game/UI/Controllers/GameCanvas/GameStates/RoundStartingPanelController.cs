using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas.GameStates
{
    public class RoundStartingPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI countdownText;

        public void StartRound(int currentRound, int totalRounds)
        {
            roundText.text = $"ROUND {currentRound}/{totalRounds}";
            SetCountdownValue(3);
        }

        public void SetCountdownValue(int value)
        {
            countdownText.text = value.ToString();
        }
    }
}