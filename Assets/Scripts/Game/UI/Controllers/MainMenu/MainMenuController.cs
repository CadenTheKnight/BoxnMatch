using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Controllers.LobbyCanvas;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu;

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the base logic for the main menu.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        // [SerializeField] private JoinPanelController joinPanelController;
        // [SerializeField] private CreatePanelController createPanelController;
        [SerializeField] private LobbyPanelController lobbyPanelController;
        [SerializeField] private SettingsPanelController settingsPanelController;


        public void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        public void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            lobbyPanelController.ShowPanel();
        }

        private void OnSettingsClicked()
        {
            settingsPanelController.ShowPanel();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}