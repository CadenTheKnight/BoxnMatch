using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Framework.Types;
using System.Text.RegularExpressions;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Assets.Scripts.Game.UI.Components.Options.ToggleSwitch;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.CreateMenu
{
    /// <summary>
    /// Handles the logic for the create lobby panel.
    /// </summary>
    public class CreatePanelController : BasePanel
    {
        [Header("Options Components")]
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private ToggleSwitchColorChange isPrivateToggle;
        [SerializeField] private Selector gameModeSelector;
        [SerializeField] private Button createButton;
        [SerializeField] private LoadingBar createLoadingBar;
        [SerializeField] private TextMeshProUGUI createText;

        private string lobbyName = string.Empty;
        private bool isPrivate = false;
        private GameMode gameMode = GameMode.PvP;

        protected override void OnEnable()
        {
            base.OnEnable();
            base.UpdateInteractable(true);

            lobbyNameInput.onValueChanged.AddListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle += OnPrivacyChanged;
            gameModeSelector.onSelectionChanged += OnGameModeChanged;
            createButton.onClick.AddListener(OnCreateClicked);

            LobbyEvents.OnLobbyCreated += OnLobbyCreated;

            UpdateCreateButtonState();
            gameModeSelector.SetSelection((int)gameMode);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            lobbyNameInput.onValueChanged.RemoveListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle -= OnPrivacyChanged;
            gameModeSelector.onSelectionChanged -= OnGameModeChanged;
            createButton.onClick.RemoveListener(OnCreateClicked);

            LobbyEvents.OnLobbyCreated -= OnLobbyCreated;

            createLoadingBar.StopLoading();
        }

        private void OnLobbyNameChanged(string input)
        {
            lobbyName = input;
            UpdateCreateButtonState();
        }

        private void OnPrivacyChanged(bool input)
        {
            isPrivate = input;
        }

        private void OnGameModeChanged(int selectionIndex)
        {
            gameMode = (GameMode)selectionIndex;
        }

        private void UpdateCreateButtonState()
        {
            createText.text = "Create";
            createButton.interactable = Regex.IsMatch(lobbyName, @"^[a-zA-Z0-9]{1,14}$");
        }

        private async void OnCreateClicked()
        {
            base.UpdateInteractable(false);
            lobbyNameInput.interactable = false;
            isPrivateToggle.UpdateInteractable(false);
            gameModeSelector.UpdateInteractable(false);
            createButton.interactable = false;
            createText.text = "Creating...";
            createLoadingBar.StartLoading();

            await LobbyManager.CreateLobby(lobbyName, isPrivate, gameMode == GameMode.AI ? 1 : 2, new LobbyData(gameMode).Serialize());
        }

        private async void OnLobbyCreated(OperationResult result)
        {
            if (result.Status == ResultStatus.Error)
            {
                createText.text = "Error Creating";
                createLoadingBar.StopLoading();
                gameModeSelector.UpdateInteractable(true);
                isPrivateToggle.UpdateInteractable(true);
                lobbyNameInput.interactable = true;
                base.UpdateInteractable(true);

                await Task.Delay(1000);

                UpdateCreateButtonState();
            }
        }
    }
}