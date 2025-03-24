using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;

namespace Assets.Scripts.Game.UI.Controllers.MainMenu
{
    /// <summary>
    /// Handles the base logic for the main menu.
    /// </summary>
    public class MainPanelController : MonoBehaviour
    {
        [SerializeField] private Button joinButton;
        [SerializeField] private Button createButton;
        [SerializeField] private JoinPanelController joinPanelController;
        [SerializeField] private CreatePanelController createPanelController;

        public void OnEnable()
        {
            joinButton.onClick.AddListener(OnJoinClicked);
            createButton.onClick.AddListener(OnCreateClicked);

            LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            LobbyEvents.OnLobbyKicked += OnLobbyKicked;
        }

        public void OnDestroy()
        {
            joinButton.onClick.RemoveListener(OnJoinClicked);
            createButton.onClick.RemoveListener(OnCreateClicked);

            LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            LobbyEvents.OnLobbyKicked -= OnLobbyKicked;
        }

        private void OnCreateClicked()
        {
            createPanelController.ShowPanel();
        }

        private void OnJoinClicked()
        {
            joinPanelController.ShowPanel();
        }

        private void OnLobbyLeft(Lobby lobby)
        {
            joinPanelController.ShowPanel();
        }

        private void OnLobbyKicked(Lobby lobby)
        {
            joinPanelController.ShowPanel();
        }
    }
}