using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Controllers.MainCanvas.JoinMenu;
using Assets.Scripts.Game.UI.Controllers.MainCanvas.CreateMenu;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas
{
    /// <summary>
    /// Handles the base logic for the main menu.
    /// </summary>
    public class MainCanvasController : MonoBehaviour
    {
        [SerializeField] private Button joinButton;
        [SerializeField] private Button createButton;
        [SerializeField] private JoinPanelController joinPanelController;
        [SerializeField] private CreatePanelController createPanelController;

        public void OnEnable()
        {
            joinButton.onClick.AddListener(OnJoinClicked);
            createButton.onClick.AddListener(OnCreateClicked);
        }

        public void OnDestroy()
        {
            joinButton.onClick.RemoveListener(OnJoinClicked);
            createButton.onClick.RemoveListener(OnCreateClicked);
        }

        private void OnCreateClicked()
        {
            createPanelController.ShowPanel();
        }

        private void OnJoinClicked()
        {
            joinPanelController.ShowPanel();
        }
    }
}