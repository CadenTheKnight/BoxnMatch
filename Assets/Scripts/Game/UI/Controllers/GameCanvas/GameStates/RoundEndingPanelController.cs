using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Controllers.GameCanvas.GameStates
{
    public class RoundEndingPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winnerText;

        public void ShowResults(string winner)
        {
            winnerText.text = winner;
        }
    }
}