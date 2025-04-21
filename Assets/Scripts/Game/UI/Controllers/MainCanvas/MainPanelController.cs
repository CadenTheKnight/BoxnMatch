using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel;
using UnityEngine.SceneManagement;
using Assets.Scripts.Game.UI.Controllers.SettingsCanvas;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas
{
    public class MainPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button playButton;
        // [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private LobbyPanelController lobbyPanelController;
        // [SerializeField] private SettingsPanelController settingsPanelController;

        public void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            // settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        public void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            // settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            lobbyPanelController.ShowPanel();
        }

        // private void OnSettingsClicked()
        // {
        //     settingsPanelController.ShowPanel();
        // }

        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}