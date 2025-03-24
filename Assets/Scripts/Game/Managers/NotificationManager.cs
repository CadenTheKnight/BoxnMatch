using UnityEngine;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("UI Components")]
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private ResultNotification resultNotification;

        private void Start()
        {
            AuthenticationEvents.OnAuthenticated += HandleAuthenticated;

            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked += HandleLobbyKicked;
        }

        private void OnDestroy()
        {
            AuthenticationEvents.OnAuthenticated -= HandleAuthenticated;

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
                resultNotification.ShowNotification(operationResult, ResultStatus.Success);
            }
            else if (operationResult.Status == ResultStatus.Warning)
            {
                Debug.Log($"{operationResult.Code} - {operationResult.Message}");
                resultNotification.ShowNotification(operationResult, ResultStatus.Warning);
            }
            else
            {
                Debug.LogError($"{operationResult.Code} - {operationResult.Message}");
                if (operationResult.Code == "AuthenticationError")
                    errorPopup.ShowError(operationResult.Code, operationResult.Message, retryAction);
                else
                    resultNotification.ShowNotification(operationResult, ResultStatus.Error);
            }
        }

        private void HandleAuthenticated(string playerName)
        {
            HandleResult(OperationResult.SuccessResult("Authenticated", $"Signed in as: {playerName}"));
        }

        private void HandleLobbyCreated(Lobby lobby)
        {
            HandleResult(OperationResult.SuccessResult("LobbyCreated", $"Lobby created: {lobby.Name}"));
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            HandleResult(OperationResult.SuccessResult("LobbyJoined", $"Lobby joined: {lobby.Name}"));
        }

        private void HandleLobbyLeft(Lobby lobby)
        {
            HandleResult(OperationResult.SuccessResult("LobbyLeft", $"Left lobby: {lobby.Name}"));
        }

        private void HandleLobbyKicked(Lobby lobby)
        {
            HandleResult(OperationResult.WarningResult("LobbyKicked", $"Kicked from lobby: {lobby.Name}"));
        }

    }
}