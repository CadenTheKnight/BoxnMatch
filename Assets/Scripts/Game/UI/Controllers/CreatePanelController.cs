using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class CreatePanelController : BasePanel
    {
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private ResultHandler resultHandler;
        [SerializeField] private TMP_InputField lobbyNameInput;
        private static readonly Regex lobbyNameRegex = new("^[a-zA-Z0-9 ]{1,20}$");

        protected override void OnEnable()
        {
            base.OnEnable();
            createLobbyButton.onClick.AddListener(OnCreateClicked);
            lobbyNameInput.onValueChanged.AddListener(OnLobbyNameChanged);

            OnLobbyNameChanged(lobbyNameInput.text);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            createLobbyButton.onClick.RemoveListener(OnCreateClicked);
            lobbyNameInput.onValueChanged.RemoveListener(OnLobbyNameChanged);
            loadingBar.StopLoading();
        }

        protected override void Update()
        {
            base.Update();
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return) && createLobbyButton.interactable)
                OnCreateClicked();
        }

        private void OnLobbyNameChanged(string lobbyName)
        {
            createLobbyButton.interactable = lobbyNameRegex.IsMatch(lobbyNameInput.text.Trim());
        }

        private async void OnCreateClicked()
        {
            createLobbyButton.interactable = false;

            loadingBar.StartLoading();
            await Task.Delay(1000); // Simulate loading
            var result = await GameLobbyManager.Instance.CreateLobby(lobbyNameInput.text.Trim());
            loadingBar.StopLoading();

            resultHandler.HandleResult(result);
            createLobbyButton.interactable = true;
        }
    }
}