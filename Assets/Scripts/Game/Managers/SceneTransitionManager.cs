using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages scene transitions based on the game state.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        private static SceneTransitionManager _instance;
        public static SceneTransitionManager Instance => _instance;

        private OperationResult pendingNotification;
        private NotificationType pendingNotificationType;
        private bool hasPendingNotification = false;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                SubscribeToEvents();
            }
            else
                Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (_instance == this)
                UnsubscribeFromEvents();
        }

        /// <summary>
        /// Sets a notification to be displayed in the next scene.
        /// </summary>
        /// <param name="result">Operation result containing message data.</param>
        /// <param name="type">The notification type.</param>
        public void SetPendingNotification(OperationResult result, NotificationType type)
        {
            pendingNotification = result;
            pendingNotificationType = type;
            hasPendingNotification = true;
        }

        /// <summary>
        /// Checks if there is a pending notification.
        /// </summary>
        public bool HasPendingNotification()
        {
            return hasPendingNotification;
        }

        /// <summary>
        /// Retrieves the pending notification and clears it.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="type">The notification type.</param>
        /// <returns>True if a notification was available.</returns>
        public bool TryGetPendingNotification(out OperationResult result, out NotificationType type)
        {
            result = pendingNotification;
            type = pendingNotificationType;

            if (hasPendingNotification)
            {
                hasPendingNotification = false;
                return true;
            }

            return false;
        }

        private void SubscribeToEvents()
        {
            AuthenticationManager.OnAuthenticated += HandleAuthenticated;

            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked += HandleLobbyKicked;
        }

        private void UnsubscribeFromEvents()
        {
            AuthenticationManager.OnAuthenticated -= HandleAuthenticated;

            LobbyEvents.OnLobbyCreated -= HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined -= HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft -= HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked -= HandleLobbyKicked;
        }

        private void HandleAuthenticated()
        {
            SetPendingNotification(OperationResult.SuccessResult("AUTH_SUCCESS", "Authentication successful!"), NotificationType.Success);
            SceneManager.LoadScene("Main");
        }

        private void HandleLobbyCreated(Lobby lobby)
        {
            SetPendingNotification(OperationResult.SuccessResult("LOBBY_CREATED", $"Created lobby '{lobby.Name}'!"), NotificationType.Success);
            SceneManager.LoadScene("Lobby");
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            SetPendingNotification(OperationResult.SuccessResult("LOBBY_JOINED", $"Joined lobby '{lobby.Name}'!"), NotificationType.Success);
            SceneManager.LoadScene("Lobby");
        }

        private void HandleLobbyLeft()
        {
            SetPendingNotification(OperationResult.SuccessResult("LOBBY_LEFT", "Left the lobby."), NotificationType.Success);
            SceneManager.LoadScene("Main");
        }

        private void HandleLobbyKicked()
        {
            SetPendingNotification(OperationResult.WarningResult("LOBBY_KICKED", "Kicked from the lobby."), NotificationType.Warning);
            SceneManager.LoadScene("Main");
        }
    }
}