using UnityEngine;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.InitializationMenu
{
    /// <summary>
    /// Handles the logic for the initialization scene.
    /// </summary>
    public class InitializationPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private LoadingStatus loadingStatus;

        /// <summary>
        /// Calls AuthenticationManager.InitializeAsync when the game starts.
        /// On success, transitions to the main menu. 
        /// On failure, shows an error popup that allows the user to quit or retry.
        /// </summary>
        public async void Start()
        {
            loadingStatus.UpdateStatus("Initializing...");
            loadingStatus.StartLoading();

            OperationResult result = await AuthenticationManager.Instance.InitializeAsync();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatus.StopLoading();
                loadingStatus.UpdateStatus("Initialization failed");

                NotificationManager.Instance.HandleResult(result, () => Start());
            }
            else
            {
                loadingStatus.UpdateStatus("Initialization successful");

                NotificationManager.Instance.HandleResult(await SceneTransitionManager.Instance.OnAuthenticated(PlayerPrefs.GetString("PlayerName")));
            }

        }
    }
}