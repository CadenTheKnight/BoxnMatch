using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.InitializationCanvas
{
    /// <summary>
    /// Handles the logic for the initialization scene.
    /// </summary>
    public class InitializationCanvasController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private LoadingStatus loadingStatusPanel;

        public async void Start()
        {
            if (await InitializeUnityServices())
                if (await InitializeSteam())
                    if (await Authenticate())
                        await AttemptRejoin();
        }

        /// <summary>
        /// Initializes Unity Servicess.
        /// </summary>
        /// <returns>True if initialization was successful, false otherwise.</returns>
        private async Task<bool> InitializeUnityServices()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Initializing Unity Services...");

            OperationResult result = await AuthenticationManager.Instance.InitializeUnityServices();

            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Initialization failed");
                NotificationManager.Instance.ShowErrorPopup(result, async () => await InitializeUnityServices());
                return false;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Unity Services initialized");
                return true;
            }

        }

        /// <summary>
        /// Initializes Steam.
        /// </summary>
        /// <returns>True if initialization was successful, false otherwise.</returns>
        private async Task<bool> InitializeSteam()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Initializing Steam...");

            OperationResult result = await AuthenticationManager.Instance.InitializeSteam();

            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Steam initialization failed");
                NotificationManager.Instance.ShowErrorPopup(result, async () => await InitializeSteam());
                return false;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Steam initialized");
                return true;
            }
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <returns>True if authentication was successful, false otherwise.</returns>
        private async Task<bool> Authenticate()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Authenticating...");

            OperationResult result = await AuthenticationManager.Instance.Authenticate();

            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Authentication failed");
                NotificationManager.Instance.ShowErrorPopup(result, async () => await Authenticate());
                return false;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Authenticated");
                return true;
            }
        }

        /// <summary>
        /// Attempts to rejoin a lobby if the user was previously in one.
        /// </summary>
        private async Task AttemptRejoin()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Checking for active lobbies...");

            List<string> joinedLobbyIds = await LobbyManager.Instance.GetJoinedLobbies();

            loadingStatusPanel.StopLoading();
            if (joinedLobbyIds.Count > 0)
            {
                loadingStatusPanel.UpdateStatus("Rejoining lobby...");
                OperationResult result = await LobbyManager.Instance.RejoinLobby(joinedLobbyIds);
                NotificationManager.Instance.ShowNotification(result);
                if (result.Status == ResultStatus.Success)
                {
                    loadingStatusPanel.UpdateStatus("Rejoined lobby successfully");
                    SceneManager.LoadSceneAsync("Lobby");
                }
                else 
                {
                    loadingStatusPanel.UpdateStatus("Failed to rejoin lobby");
                    SceneManager.LoadSceneAsync("Main");
                }
            }
            else 
            {
                loadingStatusPanel.UpdateStatus("No active lobbies found");
                SceneManager.LoadSceneAsync("Main");
            }
        }
    }
}