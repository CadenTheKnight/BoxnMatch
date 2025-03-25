using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Enums;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Colors;

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the logic for the create lobby panel.
    /// </summary>
    public class CreatePanelController : BasePanel
    {
        [Header("Options Components")]
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private Button twoPlayerButton;
        [SerializeField] private Button fourPlayerButton;
        [SerializeField] private Button privateButton;
        [SerializeField] private Button publicButton;
        [SerializeField] private NumberSelector roundCountSelector;

        [Header("Footer Components")]
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private LoadingBar createLoadingBar;

        private int maxPlayers = 0;
        private int isPrivate = 0;

        /// <summary>
        /// Regex pattern for a valid lobby name. Must be alphanumeric and between 1 and 14 characters.
        /// </summary>
        private readonly Regex lobbyNameRegex = new(@"^[a-zA-Z0-9]{1,14}$");

        protected override void OnEnable()
        {
            base.OnEnable();

            createLobbyButton.onClick.AddListener(OnCreateClicked);
            lobbyNameInput.onValueChanged.AddListener(CheckForCompletion);
            twoPlayerButton.onClick.AddListener(OnTwoPlayersClicked);
            fourPlayerButton.onClick.AddListener(OnFourPlayersClicked);
            privateButton.onClick.AddListener(OnPrivateClicked);
            publicButton.onClick.AddListener(OnPublicClicked);

            CheckForCompletion(lobbyNameInput.text);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            createLobbyButton.onClick.RemoveListener(OnCreateClicked);
            lobbyNameInput.onValueChanged.RemoveListener(CheckForCompletion);
            twoPlayerButton.onClick.RemoveListener(OnTwoPlayersClicked);
            fourPlayerButton.onClick.RemoveListener(OnFourPlayersClicked);
            privateButton.onClick.RemoveListener(OnPrivateClicked);
            publicButton.onClick.RemoveListener(OnPublicClicked);
        }

        protected override void Update()
        {
            base.Update();
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return) && createLobbyButton.interactable)
                OnCreateClicked();
        }

        private void CheckForCompletion(string lobbyName)
        {
            createLobbyButton.interactable = lobbyNameRegex.IsMatch(lobbyNameInput.text) && maxPlayers != 0 && isPrivate != 0;
        }

        private void OnTwoPlayersClicked()
        {
            maxPlayers = 2;
            ToggleButtons(twoPlayerButton, fourPlayerButton);
            CheckForCompletion(lobbyNameInput.text);
        }

        private void OnFourPlayersClicked()
        {
            maxPlayers = 4;
            ToggleButtons(fourPlayerButton, twoPlayerButton);
            CheckForCompletion(lobbyNameInput.text);
        }

        private void OnPrivateClicked()
        {
            isPrivate = 1;
            ToggleButtons(privateButton, publicButton);
            CheckForCompletion(lobbyNameInput.text);
        }

        private void OnPublicClicked()
        {
            isPrivate = 2;
            ToggleButtons(publicButton, privateButton);
            CheckForCompletion(lobbyNameInput.text);
        }

        private void ToggleButtons(Button selectedButton, Button unselectedButton)
        {
            ColorBlock selectedColors = selectedButton.colors;
            ColorBlock unselectedColors = unselectedButton.colors;

            selectedColors.normalColor = UIColors.primaryHoverColor;
            unselectedColors.normalColor = UIColors.primaryDefaultColor;

            selectedButton.colors = selectedColors;
            unselectedButton.colors = unselectedColors;
        }

        private async void OnCreateClicked()
        {
            createLobbyButton.interactable = false;

            createLoadingBar.StartLoading();
            OperationResult result = await GameLobbyManager.Instance.CreateLobby(lobbyNameInput.text.Trim(), maxPlayers, isPrivate == 1, roundCountSelector.Value);
            createLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Error)
                NotificationManager.Instance.HandleResult(result);

            createLobbyButton.interactable = true;
        }
    }
}