using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.JoinMenu
{
    /// <summary>
    /// Handles the UI for the join panel.
    /// </summary>
    public class JoinPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private Button joinButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private GameObject refreshingPanel;
        [SerializeField] private LoadingBar refeshingLoadingBar;

        [Header("List Manager")]
        [SerializeField] private LobbyListPanelController lobbyListPanelController;

        protected override void OnEnable()
        {
            base.OnEnable();

            // lobbyListPanelController.SpawnTestLobbies();

            joinButton.onClick.AddListener(OnJoinButtonClicked);
            refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.AddListener(OnLobbyCodeInputChanged);

            OnRefreshButtonClicked();
            UpdateJoinButtonState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            lobbyCodeInput.onValueChanged.RemoveListener(OnLobbyCodeInputChanged);

            lobbyListPanelController.ClearSelection();
            refeshingLoadingBar.StopLoading();
        }

        private void OnLobbyCodeInputChanged(string code)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            joinButton.interactable = lobbyCodeInput.text.Length == 6 || lobbyListPanelController.SelectedLobbyId != null;
        }

        /// <summary>
        /// Handles the join button click event. First tries to join by code, then by selected lobby.
        /// </summary>
        private void OnJoinButtonClicked()
        {
            joinButton.interactable = false;

            if (!string.IsNullOrEmpty(lobbyCodeInput.text) && lobbyCodeInput.text.Length == 6)
                GameLobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
            else
                GameLobbyManager.Instance.JoinLobbyById(lobbyListPanelController.SelectedLobbyId);

            joinButton.interactable = true;
        }


        /// <summary>
        /// Handles the refresh button click event.
        /// </summary>
        private async void OnRefreshButtonClicked()
        {
            refreshButton.interactable = false;
            refreshingPanel.SetActive(true);
            refeshingLoadingBar.StartLoading();

            await Task.Delay(1500);
            GameLobbyManager.Instance.GetLobbies();
            UpdateJoinButtonState();

            refeshingLoadingBar.StopLoading();
            refreshingPanel.SetActive(false);
            refreshButton.interactable = true;

        }
    }
}