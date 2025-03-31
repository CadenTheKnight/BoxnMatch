using System;
using UnityEngine;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Controllers.NotificationCanvas;

namespace Assets.Scripts.Game.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("UI Components")]
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private ResultNotification resultNotification;

        private void Start()
        {
            // AuthenticationEvents.OnAuthenticated += ShowNotification;
            AuthenticationEvents.OnAuthenticationError += ShowErrorPopup;
            // AuthenticationEvents.OnLobbyRejoined += ShowNotification;
            AuthenticationEvents.OnLobbyRejoinError += ShowNotification;

            // LobbyEvents.OnLobbyCreated += ShowNotification;
            // LobbyEvents.OnLobbyJoined += ShowNotification;
            // LobbyEvents.OnLobbyLeft += ShowNotification;
            // LobbyEvents.OnLobbyKicked += ShowNotification;
            // LobbyEvents.OnPlayerJoined += ShowNotification;
            // LobbyEvents.OnPlayerLeft += ShowNotification;
            // LobbyEvents.OnPlayerKicked += ShowNotification;
            // LobbyEvents.OnLobbyQueryResponse += ShowNotification;
            LobbyEvents.OnLobbyError += ShowNotification;
        }

        private void OnDestroy()
        {
            // AuthenticationEvents.OnAuthenticated -= ShowNotification;
            AuthenticationEvents.OnAuthenticationError -= ShowErrorPopup;
            // AuthenticationEvents.OnLobbyRejoined -= ShowNotification;
            AuthenticationEvents.OnLobbyRejoinError -= ShowNotification;

            // LobbyEvents.OnLobbyCreated -= ShowNotification;
            // LobbyEvents.OnLobbyJoined -= ShowNotification;
            // LobbyEvents.OnLobbyLeft -= ShowNotification;
            // LobbyEvents.OnLobbyKicked -= ShowNotification;
            // LobbyEvents.OnPlayerJoined -= ShowNotification;
            // LobbyEvents.OnPlayerLeft -= ShowNotification;
            // LobbyEvents.OnPlayerKicked -= ShowNotification;
            // LobbyEvents.OnLobbyQueryResponse -= ShowNotification;
            LobbyEvents.OnLobbyError -= ShowNotification;
        }

        private void ShowNotification(OperationResult result)
        {
            resultNotification.ShowNotification(result);
        }

        private void ShowErrorPopup(OperationResult result, Action retryAction)
        {
            errorPopup.ShowError(result, retryAction);
        }
    }
}