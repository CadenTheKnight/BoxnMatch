using UnityEngine;
using Assets.Scripts.Testing;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.Initialization
{
    /// <summary>
    /// Handles the logic for the initialization scene.
    /// </summary>
    public class InitializationController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private ResultHandler resultHandler;

        /// <summary>
        /// Calls AuthenticationManager.InitializeAsync when the game starts.
        /// On success, transitions to the main menu. 
        /// On failure, shows an error popup that allows the user to quit or retry.
        /// </summary>
        public async void Start()
        {
            loadingBar.StartLoading();
            await Tests.TestDelay(1000);
            OperationResult result = await AuthenticationManager.Instance.InitializeAsync();
            loadingBar.StopLoading();

            if (result.Success)
                SceneTransitionManager.Instance.SetPendingNotification(result, NotificationType.Success);
            else
                resultHandler.HandleResult(result);
        }
    }
}