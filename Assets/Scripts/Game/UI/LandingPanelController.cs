using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI
{
    public class LandingPanelController : MonoBehaviour
    {
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button lobbyListButton;
        [SerializeField] private Button joinWithCodeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private CreateLobbyPanelController createLobbyPanelController;
        [SerializeField] private LobbyListPanelController lobbyListPanelController;
        [SerializeField] private JoinWithCodePanelController joinWithCodePanelController;
        [SerializeField] private SettingsPanelController settingsPanelController;

        void OnEnable()
        {
            createLobbyButton.onClick.AddListener(OnCreateClicked);
            lobbyListButton.onClick.AddListener(OnListClicked);
            joinWithCodeButton.onClick.AddListener(OnJoinClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        void OnDisable()
        {
            createLobbyButton.onClick.RemoveListener(OnCreateClicked);
            lobbyListButton.onClick.RemoveListener(OnListClicked);
            joinWithCodeButton.onClick.RemoveListener(OnJoinClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnCreateClicked()
        {
            createLobbyPanelController.ShowPanel();
        }

        private void OnListClicked()
        {
            lobbyListPanelController.ShowPanel();
        }

        private void OnJoinClicked()
        {
            joinWithCodePanelController.ShowPanel();
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
