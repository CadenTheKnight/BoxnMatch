using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
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
                    await Authenticate();
        }

        private async Task<bool> InitializeUnityServices()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Initializing Unity Services...");
            OperationResult result = await AuthenticationManager.Instance.InitializeUnityServices();
            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Initialization failed");
                AuthenticationEvents.InvokeAuthenticationError(result, async () => await InitializeUnityServices());
                return false;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Unity Services initialized");
                return true;
            }

        }

        private async Task<bool> InitializeSteam()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Initializing Steam...");
            OperationResult result = await AuthenticationManager.Instance.InitializeSteam();
            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Steam initialization failed");
                AuthenticationEvents.InvokeAuthenticationError(result, async () => await InitializeSteam());
                return false;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Steam initialized");
                return true;
            }
        }

        private async Task Authenticate()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Authenticating...");
            OperationResult result = await AuthenticationManager.Instance.Authenticate();
            loadingStatusPanel.StopLoading();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.UpdateStatus("Authentication failed");
                AuthenticationEvents.InvokeAuthenticationError(result, async () => await Authenticate());
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Authenticated");
                if (!await AttemptRejoin())
                {
                    loadingStatusPanel.UpdateStatus(result.Message);
                    AuthenticationEvents.InvokeAuthenticated(result);
                }
            }
        }

        private async Task<bool> AttemptRejoin()
        {
            loadingStatusPanel.StartLoading();
            loadingStatusPanel.UpdateStatus("Checking for active lobbies...");
            List<string> joinedLobbyIds = await LobbyManager.Instance.GetJoinedLobbies();
            loadingStatusPanel.StopLoading();
            if (joinedLobbyIds.Count > 0)
            {
                loadingStatusPanel.UpdateStatus("Rejoining lobby...");
                await LobbyManager.Instance.RejoinLobby(joinedLobbyIds);
                return true;
            }
            else
            {
                loadingStatusPanel.UpdateStatus("No active lobbies found");
                return false;
            }
        }
    }
}