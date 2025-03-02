using UnityEngine;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Framework.Utilities
{
    public class ResultHandler : MonoBehaviour
    {
        [SerializeField] private ErrorPanel errorPanel;
        [SerializeField] private ResultNotification resultNotification;

        /// <summary>
        /// Handles the result of an operation.
        /// </summary>
        /// <param name="operationResult">The result of the operation.</param>
        /// <param name="retryAction">The action to retry the operation.</param>
        public void HandleResult(OperationResult operationResult, System.Action retryAction = null)
        {
            if (operationResult.Success)
            {
                Debug.Log($"{operationResult.Code} - {operationResult.Message}");
                resultNotification.ShowNotification(operationResult, NotificationType.Success);
            }
            else
            {
                Debug.LogError($"{operationResult.Code} - {operationResult.Message}");
                if (operationResult.Code == "AuthenticationError")
                    errorPanel.ShowError(operationResult.Code, operationResult.Message, retryAction);
                else
                    resultNotification.ShowNotification(operationResult, NotificationType.Error);
            }
        }
    }
}