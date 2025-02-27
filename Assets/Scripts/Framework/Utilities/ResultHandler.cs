using UnityEngine;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Framework.Utilities
{
    public class ResultHandler : MonoBehaviour
    {
        [SerializeField] private ResultNotification resultNotification;

        public void HandleResult(OperationResult operationResult)
        {
            if (operationResult.Success)
                Debug.Log($"{operationResult.Code} - {operationResult.Message}");
            else
            {
                Debug.LogError($"{operationResult.Code} - {operationResult.Message}");
                resultNotification.ShowNotification(operationResult);
            }
        }
    }
}