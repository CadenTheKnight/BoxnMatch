using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Game.UI
{
    public class CreateLobbyPanelController : BasePanelController
    {
        [SerializeField] private LoadingBarAnimator loadingBar;
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button createLobbyButton;
        private Loading loading;
        private static readonly Regex lobbyNameRegex = new("^[a-zA-Z0-9 ]{1,20}$");

        protected override void OnEnable()
        {
            base.OnEnable();
            errorText.text = "";
            createLobbyButton.onClick.AddListener(OnCreateClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            createLobbyButton.onClick.RemoveListener(OnCreateClicked);

            if (loading != null)
            {
                loading.StopLoading(createLobbyButton, loadingBar);
            }
        }

        private void Awake()
        {
            loading = gameObject.AddComponent<Loading>();
        }

        private async void OnCreateClicked()
        {
            errorText.text = "";
            if (!lobbyNameRegex.IsMatch(lobbyNameInput.text))
            {
                errorText.text = "Lobby name must be 1-20 characters long and only contain letters, numbers, and spaces.";
                return;
            }

            loading.StartLoading(createLobbyButton, loadingBar);
            var result = await GameLobbyManager.Instance.CreateLobby(lobbyNameInput.text);
            loading.StartLoading(createLobbyButton, loadingBar);

            if (result.Success)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                errorText.text = $"Failed to create lobby: {result.ErrorCode} - {result.ErrorMessage}";
            }
        }
    }
}