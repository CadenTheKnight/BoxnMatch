using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
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
            AuthenticationManager.OnAuthenticated += HandleAuthenticated;

            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked += HandleLobbyKicked;
        }

        private void OnDestroy()
        {

            AuthenticationManager.OnAuthenticated -= HandleAuthenticated;

            LobbyEvents.OnLobbyCreated -= HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined -= HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft -= HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked -= HandleLobbyKicked;
        }

        private void HandleAuthenticated()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        private void HandleLobbyCreated(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }

        private void HandleLobbyLeft()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        private void HandleLobbyKicked()
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }
}