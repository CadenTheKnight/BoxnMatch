using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.ToggleSwitch;
using Assets.Scripts.Framework.Managers;

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
        [SerializeField] private TextMeshProUGUI createText;

        private string lobbyName = string.Empty;
        private bool isPrivate = false;
        private int maxPlayers = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            lobbyNameInput.onValueChanged.AddListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle += OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged += OnMaxPlayersChanged;
            createButton.onClick.AddListener(OnCreateClicked);

            UpdateCreateButtonState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            lobbyNameInput.onValueChanged.RemoveListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle -= OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged -= OnMaxPlayersChanged;
            createButton.onClick.RemoveListener(OnCreateClicked);
        }

        private void OnLobbyNameChanged(string input)
        {
            lobbyName = input;
            UpdateCreateButtonState();
        }

        private void OnPrivacyChanged(bool input)
        {
            isPrivate = input;
            UpdateCreateButtonState();
        }

        private void OnMaxPlayersChanged(int selectionIndex)
        {
            maxPlayers = selectionIndex == 0 ? 2 : 4;
            UpdateCreateButtonState();
        }

        private bool IsFormValid()
        {
            return Regex.IsMatch(lobbyName, @"^[a-zA-Z0-9]{1,14}$") && (maxPlayers == 2 || maxPlayers == 4);
        }

        private void UpdateCreateButtonState()
        {
            createButton.interactable = IsFormValid();
        }

        protected override void Update()
        {
            base.Update();
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return) && createButton.interactable)
                OnCreateClicked();
        }

        private async void OnCreateClicked()
        {
            createButton.interactable = false;
            createText.text = "Creating...";

            await Task.Delay(1500);
            await GameLobbyManager.Instance.CreateLobby(lobbyName, isPrivate, maxPlayers);

            if (LobbyManager.Instance.Lobby != null)
                createText.text = "Created!";
            else
            {
                createText.text = "Create";
                createButton.interactable = true;
            }
        }
    }
}