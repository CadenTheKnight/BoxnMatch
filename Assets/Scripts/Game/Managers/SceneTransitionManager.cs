using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;

namespace Assets.Scripts.Game.Managers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        private static SceneTransitionManager _instance;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                SubscribeToEvents();
            }
            else
                Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (_instance == this)
                UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            AuthEvents.OnAuthenticated += HandleAuthenticated;
            LobbyEvents.OnLobbyCreated += HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined += HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft += HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked += HandleLobbyKicked;
            // Add other events as needed
        }

        private void UnsubscribeFromEvents()
        {
            AuthEvents.OnAuthenticated -= HandleAuthenticated;
            LobbyEvents.OnLobbyCreated -= HandleLobbyCreated;
            LobbyEvents.OnLobbyJoined -= HandleLobbyJoined;
            LobbyEvents.OnLobbyLeft -= HandleLobbyLeft;
            LobbyEvents.OnLobbyKicked -= HandleLobbyKicked;
            // Remove other events as well
        }

        private void HandleAuthenticated()
        {
            SceneManager.LoadScene("Main");
        }

        private void HandleLobbyCreated(Lobby lobby)
        {
            SceneManager.LoadScene("Lobby");
        }

        private void HandleLobbyJoined(Lobby lobby)
        {
            SceneManager.LoadScene("Lobby");
        }

        private void HandleLobbyLeft()
        {
            SceneManager.LoadScene("Main");
        }

        private void HandleLobbyKicked()
        {
            SceneManager.LoadScene("Main");
        }
    }
}