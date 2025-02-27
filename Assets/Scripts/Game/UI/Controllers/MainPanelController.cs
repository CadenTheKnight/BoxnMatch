using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework.Events;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class MainPanelController : MonoBehaviour
    {
        [SerializeField] private Button quitButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button createButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private JoinPanelController joinPanelController;
        [SerializeField] private CreatePanelController createPanelController;
        [SerializeField] private SettingsPanelController settingsPanelController;

        public void OnEnable()
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            joinButton.onClick.AddListener(OnJoinClicked);
            createButton.onClick.AddListener(OnCreateClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);

            LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            LobbyEvents.OnLobbyKicked += OnLobbyKicked;
        }

        public void OnDestroy()
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
            joinButton.onClick.RemoveListener(OnJoinClicked);
            createButton.onClick.RemoveListener(OnCreateClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

            LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            LobbyEvents.OnLobbyKicked -= OnLobbyKicked;
        }

        private void OnCreateClicked()
        {
            createPanelController.ShowPanel();
        }

        private void OnJoinClicked()
        {
            joinPanelController.ShowPanel();
        }

        private void OnSettingsClicked()
        {
            settingsPanelController.ShowPanel();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        private void OnLobbyLeft()
        {
            joinPanelController.ShowPanel();
        }

        private void OnLobbyKicked()
        {
            joinPanelController.ShowPanel();
        }
    }
}