using UnityEngine;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.Managers;

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// Checks for pending notifications when a scene loads and displays them
    /// </summary>
    public class SceneNotificationHandler : MonoBehaviour
    {
        [SerializeField] private ResultHandler resultHandler;

        private void Start()
        {
            if (SceneTransitionManager.Instance.TryGetPendingNotification(out OperationResult result, out NotificationType type))
                resultHandler.HandleResult(result);
        }
    }
}