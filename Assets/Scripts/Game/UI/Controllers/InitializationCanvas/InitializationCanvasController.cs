using System;
using UnityEngine;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using System.Collections.Generic;

namespace Assets.Scripts.Game.UI.Controllers.InitializationCanvas
{
    /// <summary>
    /// Handles the logic for the initialization scene.
    /// </summary>
    public class InitializationCanvasController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private LoadingStatus loadingStatusPanel;

        /// <summary>
        /// Authenticates the player and attempts to rejoin an active lobby.
        /// </summary>
        public async void Start()
        {
            loadingStatusPanel.UpdateStatus("Initializing...");
            loadingStatusPanel.StartLoading();

            OperationResult result = await AuthenticationManager.Instance.InitializeAsync();
            if (result.Status == ResultStatus.Error)
            {
                loadingStatusPanel.StopLoading();
                loadingStatusPanel.UpdateStatus("Initialization failed");

                AuthenticationEvents.InvokeAuthenticationError(result, () => Start());
            }
            else
            {
                loadingStatusPanel.UpdateStatus("Checking for active lobbies...");
                try
                {
                    List<string> joinedLobbyIds = await GameLobbyManager.Instance.GetJoinedLobbies();
                    if (joinedLobbyIds.Count > 0)
                    {
                        loadingStatusPanel.UpdateStatus("Rejoining lobby...");
                        GameLobbyManager.Instance.RejoinLobby(joinedLobbyIds);
                    }
                    else
                    {
                        loadingStatusPanel.UpdateStatus("No active lobbies found");
                        AuthenticationEvents.InvokeAuthenticated(result);
                    }

                    loadingStatusPanel.StopLoading();
                }
                catch (Exception ex)
                {
                    loadingStatusPanel.StopLoading();
                    loadingStatusPanel.UpdateStatus("Error during rejoin process");
                    AuthenticationEvents.InvokeLobbyRejoinError(OperationResult.ErrorResult("RejoinError", ex.Message));
                }
            }
        }
    }
}