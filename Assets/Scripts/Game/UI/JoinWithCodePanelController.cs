using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game.UI
{
    public class JoinWithCodePanelController : BasePanelController
    {
        [SerializeField] private LoadingBarAnimator loadingBar;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button submitLobbyCodeButton;
        private Loading loading;

        private void Awake()
        {
            loading = gameObject.AddComponent<Loading>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            errorText.text = "";
            submitLobbyCodeButton.onClick.AddListener(OnSubmitCodeClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            submitLobbyCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);

            if (loading != null)
            {
                loading.StopLoading(submitLobbyCodeButton, loadingBar);
            }
        }

        private async void OnSubmitCodeClicked()
        {
            errorText.text = "";
            if (string.IsNullOrWhiteSpace(lobbyCodeInput.text))
            {
                errorText.text = "Lobby code cannot be empty.";
                return;
            }

            loading.StartLoading(submitLobbyCodeButton, loadingBar);
            var result = await GameLobbyManager.Instance.JoinLobby(lobbyCodeInput.text);
            loading.StopLoading(submitLobbyCodeButton, loadingBar);

            if (result.Success)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                errorText.text = $"Failed to join lobby with code {lobbyCodeInput.text}: {result.ErrorCode} - {result.ErrorMessage}";
            }
        }
    }
}
