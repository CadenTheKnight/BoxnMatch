using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options;
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
        [SerializeField] private Button createLobbyButton;

        private string lobbyName = string.Empty;
        private bool isPrivate = false;
        private int maxPlayers = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            lobbyNameInput.onValueChanged.AddListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle += OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged += OnMaxPlayersChanged;
            createLobbyButton.onClick.AddListener(OnCreateClicked);

            UpdateCreateButtonState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            lobbyNameInput.onValueChanged.RemoveListener(OnLobbyNameChanged);
            isPrivateToggle.onToggle -= OnPrivacyChanged;
            maxPlayersSelector.onSelectionChanged -= OnMaxPlayersChanged;
            createLobbyButton.onClick.RemoveListener(OnCreateClicked);
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

        private void OnMaxPlayersChanged(List<int> selections)
        {
            if (selections.Count == 0) maxPlayers = 0;
            else maxPlayers = (selections[0] + 1) * 2;

            UpdateCreateButtonState();
        }

        private bool IsFormValid()
        {
            return Regex.IsMatch(lobbyName, @"^[a-zA-Z0-9]{1,14}$") && (maxPlayers == 2 || maxPlayers == 4);
        }

        private void UpdateCreateButtonState()
        {
            createLobbyButton.interactable = IsFormValid();
        }

        protected override void Update()
        {
            base.Update();
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return) && createLobbyButton.interactable)
                OnCreateClicked();
        }

        private void OnCreateClicked()
        {
            createLobbyButton.interactable = false;

            GameLobbyManager.Instance.CreateLobby(lobbyName, isPrivate, maxPlayers);

            createLobbyButton.interactable = true;
        }
    }
}