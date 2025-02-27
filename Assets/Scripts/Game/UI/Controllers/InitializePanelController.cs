using UnityEngine;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class InitializePanelController : MonoBehaviour
    {
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private ErrorPanel errorPanel;

        void Start()
        {
            InitializeAsync();
        }

        /// <summary>
        /// Calls AuthManager.InitializeAsync and handles the result.
        /// On success, transitions to the main menu. On failure, shows an error popup.
        /// </summary>
        public async void InitializeAsync()
        {
            loadingBar.StartLoading();
            var result = await AuthManager.Instance.InitializeAsync();
            loadingBar.StopLoading();

            if (result.Success)
                Debug.Log($"{result.Code} - {result.Message}");
            else
            {
                Debug.LogError($"{result.Code} - {result.Message}");
                errorPanel.ShowError(result.Code, result.Message, InitializeAsync);
            }
        }
    }
}