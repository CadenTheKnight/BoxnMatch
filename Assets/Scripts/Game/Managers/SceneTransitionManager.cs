using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages scene transitions based on the game state.
    /// </summary>
    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        private void Start()
        {
            LobbyEvents.OnLobbyCreated += OnLobbyCreated;
            LobbyEvents.OnLobbyJoined += OnLobbyJoined;
            LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            LobbyEvents.OnLobbyKicked += OnLobbyKicked;
        }

        private void OnDestroy()
        {
            LobbyEvents.OnLobbyCreated -= OnLobbyCreated;
            LobbyEvents.OnLobbyJoined -= OnLobbyJoined;
            LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            LobbyEvents.OnLobbyKicked -= OnLobbyKicked;
        }

        /// <summary>
        /// Attempts to rejoin the player to a lobby they were previously in.
        /// If the player is in multiple lobbies, they will be removed from all but the most recent lobby.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        public async Task<OperationResult> OnAuthenticated(string playerName)
        {
            LoadingStatus loadingStatus = FindObjectOfType<LoadingStatus>();
            if (loadingStatus != null)
            {
                loadingStatus.UpdateStatus("Checking for joined lobbies...");
                await Task.Yield();
            }

            try
            {
                bool hasActiveLobbies = await GameLobbyManager.Instance.HasActiveLobbies();
                if (!hasActiveLobbies)
                {
                    loadingStatus.UpdateStatus("No joined lobbies found");
                    loadingStatus.StopLoading();
                    SceneManager.LoadSceneAsync("Main");
                    return OperationResult.SuccessResult("SignedIn", $"Signed in as {playerName}");
                }
                else
                {
                    loadingStatus.UpdateStatus("Rejoining lobby...");
                    OperationResult result = await GameLobbyManager.Instance.RejoinLobby();

                    if (result.Status == ResultStatus.Success)
                    {
                        loadingStatus.UpdateStatus("Rejoined lobby");
                        loadingStatus.StopLoading();
                        SceneManager.LoadSceneAsync("Lobby");
                        return result;
                    }
                    else
                    {
                        loadingStatus.UpdateStatus("Error rejoining lobby");
                        loadingStatus.StopLoading();
                        SceneManager.LoadSceneAsync("Main");
                        return result;
                    }
                }
            }
            catch (System.Exception ex)
            {
                loadingStatus.UpdateStatus("Error in OnAuthenticated");
                loadingStatus.StopLoading();
                SceneManager.LoadSceneAsync("Main");
                return OperationResult.ErrorResult("OnAuthenticatedError", ex.Message);
            }
        }

        private void OnLobbyCreated(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }

        private void OnLobbyLeft(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Main");
        }

        private void OnLobbyKicked(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }
}