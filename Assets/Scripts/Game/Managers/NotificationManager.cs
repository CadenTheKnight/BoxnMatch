using System;
using UnityEngine;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Controllers.NotificationCanvas;

namespace Assets.Scripts.Game.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = false;

        [Header("UI Components")]
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private ResultNotification resultNotification;

        private void Start()
        {
            AuthenticationEvents.OnAuthenticated += ShowNotification;
            AuthenticationEvents.OnAuthenticationError += ShowErrorPopup;
            AuthenticationEvents.OnLobbyRejoined += ShowNotification;
            AuthenticationEvents.OnLobbyRejoinError += ShowNotification;

            LobbyEvents.OnLobbyCreated += ShowNotification;
            LobbyEvents.OnLobbyJoined += ShowNotification;
            LobbyEvents.OnLobbyLeft += ShowNotification;
            LobbyEvents.OnLobbyKicked += ShowNotification;
            // LobbyEvents.OnPlayerJoined += ShowNotification;
            // LobbyEvents.OnPlayerLeft += ShowNotification;
            // LobbyEvents.OnPlayerKicked += ShowNotification;
            LobbyEvents.OnLobbyQueryResponse += ShowNotification;
            LobbyEvents.OnLobbyError += ShowNotification;

            LobbyEvents.OnLobbyDataUpdated += ShowNotification;
            LobbyEvents.OnPlayerDataUpdated += ShowNotification;

        }

        private void OnDestroy()
        {
            AuthenticationEvents.OnAuthenticated -= ShowNotification;
            AuthenticationEvents.OnAuthenticationError -= ShowErrorPopup;
            AuthenticationEvents.OnLobbyRejoined -= ShowNotification;
            AuthenticationEvents.OnLobbyRejoinError -= ShowNotification;

            LobbyEvents.OnLobbyCreated -= ShowNotification;
            LobbyEvents.OnLobbyJoined -= ShowNotification;
            LobbyEvents.OnLobbyLeft -= ShowNotification;
            LobbyEvents.OnLobbyKicked -= ShowNotification;
            // LobbyEvents.OnPlayerJoined -= ShowNotification;
            // LobbyEvents.OnPlayerLeft -= ShowNotification;
            // LobbyEvents.OnPlayerKicked -= ShowNotification;
            LobbyEvents.OnLobbyQueryResponse -= ShowNotification;
            LobbyEvents.OnLobbyError -= ShowNotification;

            LobbyEvents.OnLobbyDataUpdated -= ShowNotification;

            LobbyEvents.OnPlayerDataUpdated -= ShowNotification;
        }

        private void ShowNotification(OperationResult result)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            resultNotification.ShowNotification(result);
        }

        private void ShowErrorPopup(OperationResult result, Action retryAction)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            errorPopup.ShowError(result, retryAction);
        }
    }
}