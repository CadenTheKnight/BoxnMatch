using TMPro;
using UnityEngine;
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
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private LoadingBar initializeLoadingBar;


        /// <summary>
        /// Calls AuthenticationManager.InitializeAsync when the game starts.
        /// On success, transitions to the main menu. 
        /// On failure, shows an error popup that allows the user to quit or retry.
        /// </summary>
        public async void Start()
        {
            initializeLoadingBar.StartLoading();
            statusText.text = "Initializing...";
            await Tests.LoadingTest(1000);
            OperationResult result = await AuthenticationManager.Instance.InitializeAsync();
            initializeLoadingBar.StopLoading();

            if (result.Status == ResultStatus.Failure)
            {
                statusText.text = "Initialization failed.";
                NotificationManager.Instance.HandleResult(result, () => Start());
            }

        }
    }
}