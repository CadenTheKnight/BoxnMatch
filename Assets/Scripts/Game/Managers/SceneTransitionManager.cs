using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        [Header("Debug Options")]
        [SerializeField] private bool showDebugMessages = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            LobbyEvents.OnLobbyCreated += OnLobbyEntered;
            LobbyEvents.OnLobbyJoined += OnLobbyEntered;
            LobbyEvents.OnLobbyRejoined += OnLobbyRejoined;
            LobbyEvents.OnLobbyLeft += OnLobbyExited;

            // GameEvents.OnGameStarted += OnGameStarted;
            // GameEvents.OnGameEnded += OnGameEnded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            LobbyEvents.OnLobbyCreated -= OnLobbyEntered;
            LobbyEvents.OnLobbyJoined -= OnLobbyEntered;
            LobbyEvents.OnLobbyRejoined -= OnLobbyRejoined;
            LobbyEvents.OnLobbyLeft -= OnLobbyExited;

            // GameEvents.OnGameStarted -= OnGameStarted;
            // GameEvents.OnGameEnded -= OnGameEnded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (showDebugMessages) Debug.Log($"Scene Loaded: {scene.name} in mode {mode}");
        }

        private async void OnLobbyEntered(OperationResult result)
        {
            if (result.Status == ResultStatus.Success)
            {
                GameObject.Find("GameManager").AddComponent<GameLobbyManager>();
                await GameLobbyManager.Instance.Initialize((Lobby)result.Data);

                TransitionToScene("Lobby");
            }
        }

        private async void OnLobbyRejoined(OperationResult result)
        {
            if (result.Status == ResultStatus.Success)
            {
                GameObject.Find("GameManager").AddComponent<GameLobbyManager>();
                await GameLobbyManager.Instance.Initialize((Lobby)result.Data);

                TransitionToScene("Lobby");
            }
            else TransitionToScene("Main");
        }

        private async void OnLobbyExited(OperationResult result)
        {
            if (result.Status == ResultStatus.Success)
            {
                TransitionToScene("Main");

                await GameLobbyManager.Instance.Cleanup();
                Destroy(GameObject.Find("GameManager").GetComponent<GameLobbyManager>());
            }
        }

        private void OnGameStarted(OperationResult result)
        {
            if (result.Status == ResultStatus.Success) SceneManager.LoadSceneAsync((string)result.Data, LoadSceneMode.Additive);
        }

        private void OnGameEnded(OperationResult result)
        {
            if (result.Status == ResultStatus.Success) SceneManager.UnloadSceneAsync((string)result.Data);
        }

        private void TransitionToScene(string sceneName)
        {
            if (showDebugMessages) Debug.Log($"Transitioning to scene: {sceneName}");
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}