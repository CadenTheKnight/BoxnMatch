using System;
using UnityEngine;
using Assets.Scripts.Framework.Core;
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

        public void ShowNotification(OperationResult result)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            resultNotification.ShowNotification(result);
        }

        public void ShowErrorPopup(OperationResult result, Action retryAction)
        {
            if (showDebugMessages) Debug.Log($"{result.Code} - {result.Message}");
            errorPopup.ShowError(result, retryAction);
        }
    }
}