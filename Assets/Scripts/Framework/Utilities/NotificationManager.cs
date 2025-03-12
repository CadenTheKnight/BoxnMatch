using UnityEngine;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Managers;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Framework.Utilities
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("Components")]
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private ResultNotification resultNotification;

        private void Start()
        {
            AuthenticationManager.OnAuthenticated += HandleAuthenticated;

            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked += HandleLobbyKicked;
        }

        private void OnDestroy()
        {

            AuthenticationManager.OnAuthenticated -= HandleAuthenticated;

            LobbyEvents.OnLobbyCreated -= HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined -= HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft -= HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked -= HandleLobbyKicked;
        }


        /// <summary>
        /// Handles the result of an operation.
        /// </summary>
        /// <param name="operationResult">The result of the operation.</param>
        /// <param name="retryAction">The action to retry the operation.</param>
        public void HandleResult(OperationResult operationResult, System.Action retryAction = null)
        {
            if (operationResult.Status == ResultStatus.Success)
            {
                Debug.Log($"{operationResult.Code} - {operationResult.Message}");
                resultNotification.ShowNotification(operationResult, NotificationType.Success);
            }
            else if (operationResult.Status == ResultStatus.Warning)
            {
                Debug.Log($"{operationResult.Code} - {operationResult.Message}");
                resultNotification.ShowNotification(operationResult, NotificationType.Warning);
            }
            else
            {
                Debug.LogError($"{operationResult.Code} - {operationResult.Message}");
                if (operationResult.Code == "AuthenticationError")
                    errorPopup.ShowError(operationResult.Code, operationResult.Message, retryAction);
                else
                    resultNotification.ShowNotification(operationResult, NotificationType.Error);
            }
        }

        private void HandleAuthenticated()
        {
            HandleResult(OperationResult.SuccessResult("Authenticated", $"Signed in as: {PlayerPrefs.GetString("PlayerName")}"));
        }

        private void HandleLobbyCreated(Lobby lobby)
        {
            HandleResult(OperationResult.SuccessResult("LobbyCreated", $"Lobby created: {lobby.Name}"));
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            HandleResult(OperationResult.SuccessResult("LobbyJoined", $"Lobby joined: {lobby.Name}"));
        }

        private void HandleLobbyLeft()
        {
            HandleResult(OperationResult.SuccessResult("LobbyLeft", "Left lobby"));
        }

        private void HandleLobbyKicked()
        {
            HandleResult(OperationResult.WarningResult("LobbyKicked", "Kicked from lobby"));
        }

    }
}