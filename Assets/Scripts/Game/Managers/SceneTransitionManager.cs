using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Events;
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
            AuthenticationEvents.OnAuthenticated += TransitionToMain;
            AuthenticationEvents.OnLobbyRejoined += TransitionToLobby;
            AuthenticationEvents.OnLobbyRejoinError += TransitionToMain;

            LobbyEvents.OnLobbyCreated += TransitionToLobby;
            LobbyEvents.OnLobbyJoined += TransitionToLobby;
            LobbyEvents.OnLobbyLeft += TransitionToMain;
            LobbyEvents.OnLobbyKicked += TransitionToMain;
        }

        private void OnDestroy()
        {
            AuthenticationEvents.OnAuthenticated -= TransitionToMain;
            AuthenticationEvents.OnLobbyRejoined -= TransitionToLobby;
            AuthenticationEvents.OnLobbyRejoinError -= TransitionToMain;

            LobbyEvents.OnLobbyCreated -= TransitionToLobby;
            LobbyEvents.OnLobbyJoined -= TransitionToLobby;
            LobbyEvents.OnLobbyLeft -= TransitionToMain;
            LobbyEvents.OnLobbyKicked -= TransitionToMain;
        }

        private void TransitionToMain(OperationResult result = null)
        {
            SceneManager.LoadSceneAsync("Main");
        }

        private void TransitionToLobby(OperationResult result = null)
        {
            SceneManager.LoadSceneAsync("Lobby");
        }
    }
}