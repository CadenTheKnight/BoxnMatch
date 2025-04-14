using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
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
        [SerializeField] private Selector maxPlayersSelector;
        [SerializeField] private Button createButton;
        [SerializeField] private LoadingBar createLoadingBar;
        [SerializeField] private TextMeshProUGUI createText;

        private string lobbyName = string.Empty;
        private bool isPrivate = false;
        private int maxPlayers = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            base.UpdateInteractable(true);

            lobbyNameInput.onValueChanged.AddListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle += OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged += OnMaxPlayersChanged;
            createButton.onClick.AddListener(OnCreateClicked);

            LobbyEvents.OnLobbyCreated += OnLobbyCreated;

            UpdateCreateButtonState();
            lobbyNameInput.interactable = true;
            isPrivateToggle.UpdateInteractable(true);
            maxPlayersSelector.UpdateInteractable(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            lobbyNameInput.onValueChanged.RemoveListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle -= OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged -= OnMaxPlayersChanged;
            createButton.onClick.RemoveListener(OnCreateClicked);

            LobbyEvents.OnLobbyCreated -= OnLobbyCreated;

            createLoadingBar.StopLoading();
            createText.text = "Create";
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

        private void OnMaxPlayersChanged(int selectionIndex)
        {
            maxPlayers = selectionIndex == 0 ? 2 : selectionIndex == 1 ? 4 : 0;
            UpdateCreateButtonState();
        }

        private void UpdateCreateButtonState()
        {
            createText.text = "Create";
            createButton.interactable = Regex.IsMatch(lobbyName, @"^[a-zA-Z0-9]{1,14}$") && (maxPlayers == 2 || maxPlayers == 4);
        }

        protected override void Update()
        {
            base.Update();
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return) && createButton.interactable)
                OnCreateClicked();
        }

        private async void OnCreateClicked()
        {
            base.UpdateInteractable(false);
            lobbyNameInput.interactable = false;
            isPrivateToggle.UpdateInteractable(false);
            maxPlayersSelector.UpdateInteractable(false);
            createButton.interactable = false;
            createText.text = "Creating...";
            createLoadingBar.StartLoading();

            await Task.Delay(1000);

            await LobbyManager.CreateLobby(lobbyName, isPrivate, maxPlayers, new LobbyData().Serialize());
        }

        private void OnLobbyCreated(OperationResult result)
        {
            if (result.Status == ResultStatus.Error)
            {
                createText.text = "Create";
                createLoadingBar.StopLoading();
                maxPlayersSelector.UpdateInteractable(true);
                isPrivateToggle.UpdateInteractable(true);
                lobbyNameInput.interactable = true;
                base.UpdateInteractable(true);

                UpdateCreateButtonState();
            }
        }
    }
}