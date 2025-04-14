using UnityEngine;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Controllers.NotificationCanvas;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu;

namespace Assets.Scripts.Game.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = false;

        [Header("UI Components")]
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private ResultNotification resultNotification;

        private void OnEnable()
        {
            AuthEvents.OnInitializationError += ShowErrorPopup;

            SettingsPanelController.OnSettingsUpdated += ShowNotification;

            LobbyEvents.OnLobbiesQueried += ShowNotification;
            LobbyEvents.OnLobbyCreated += ShowNotification;
            LobbyEvents.OnLobbyJoined += ShowNotification;
            LobbyEvents.OnLobbyRejoined += ShowNotification;
            LobbyEvents.OnLobbyLeft += ShowNotification;
        }

        private void OnDisable()
        {
            AuthEvents.OnInitializationError -= ShowErrorPopup;

            SettingsPanelController.OnSettingsUpdated -= ShowNotification;

            LobbyEvents.OnLobbiesQueried -= ShowNotification;
            LobbyEvents.OnLobbyCreated -= ShowNotification;
            LobbyEvents.OnLobbyJoined -= ShowNotification;
            LobbyEvents.OnLobbyRejoined -= ShowNotification;
            LobbyEvents.OnLobbyLeft -= ShowNotification;
        }

        private void ShowNotification(OperationResult result)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            resultNotification.ShowNotification(result);
        }

        private void ShowErrorPopup(OperationResult result)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            errorPopup.ShowError(result, result.Retry);
        }
    }
}