using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game
{
    public class MainMenuController : MonoBehaviour
    {

        [SerializeField] private Button quitButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private GameObject lobbyListMenu;
        [SerializeField] private Button submitLobbyCodeButton;
        [SerializeField] private TMP_InputField lobbyCodeInput;

        void OnEnable()
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            closeButton.onClick.AddListener(OnBackClicked);
            joinLobbyButton.onClick.AddListener(OnJoinClicked);
            createLobbyButton.onClick.AddListener(OnCreateClicked);
            submitLobbyCodeButton.onClick.AddListener(OnSubmitCodeClicked);
        }

        void OnDisable()
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
            closeButton.onClick.RemoveListener(OnBackClicked);
            joinLobbyButton.onClick.RemoveListener(OnJoinClicked);
            createLobbyButton.onClick.RemoveListener(OnCreateClicked);
            submitLobbyCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackClicked();
            }
        }

        private async void OnCreateClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby();
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }

        private void OnJoinClicked()
        {
            mainMenu.SetActive(false);
            lobbyListMenu.SetActive(true);
        }

        private void OnBackClicked()
        {
            mainMenu.SetActive(true);
            lobbyListMenu.SetActive(false);
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        private async void OnSubmitCodeClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.JoinLobby(lobbyCodeInput.text);
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
    }
}
