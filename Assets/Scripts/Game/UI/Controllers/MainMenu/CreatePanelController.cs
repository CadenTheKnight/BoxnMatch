using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Testing;
using Assets.Scripts.Game.Managers;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the logic for the create lobby panel.
    /// </summary>
    public class CreatePanelController : BasePanel
    {
        [Header("Components")]
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private ResultHandler resultHandler;
        [SerializeField] private TMP_InputField lobbyNameInput;

        /// <summary>
        /// Regex pattern for a valid lobby name. Must be alphanumeric and between 1 and 14 characters.
        /// </summary>
        private readonly Regex lobbyNameRegex = new(@"^[a-zA-Z0-9]{1,14}$");

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
            await Tests.TestDelay(1000);
            OperationResult result = await GameLobbyManager.Instance.CreateLobby(lobbyNameInput.text.Trim());
            loadingBar.StopLoading();

            if (result.Status == ResultStatus.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);

            createLobbyButton.interactable = true;
        }
    }
}