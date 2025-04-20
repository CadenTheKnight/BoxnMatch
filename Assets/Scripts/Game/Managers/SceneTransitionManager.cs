// using UnityEngine;
// using Assets.Scripts.Game.Events;
// using UnityEngine.SceneManagement;
// using Unity.Services.Lobbies.Models;
// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Framework.Types;
// using Assets.Scripts.Framework.Events;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.Managers
// {
//     public class SceneTransitionManager : Singleton<SceneTransitionManager>
//     {
//         [Header("Debug Options")]
//         [SerializeField] private bool showDebugMessages = false;

//         private void OnEnable()
//         {
//             SceneManager.sceneLoaded += OnSceneLoaded;

//             LobbyEvents.OnLobbyCreated += OnLobbyEntered;
//             LobbyEvents.OnLobbyJoined += OnLobbyEntered;
//             LobbyEvents.OnLobbyRejoined += OnLobbyRejoined;
//             LobbyEvents.OnLobbyLeft += OnLobbyExited;
//             LobbyEvents.OnLobbyKicked += OnLobbyExited;

//             GameEvents.OnGameStarted += OnGameStarted;
//             GameEvents.OnGameEnded += OnGameEnded;
//         }

//         private void OnDisable()
//         {
//             SceneManager.sceneLoaded -= OnSceneLoaded;

//             LobbyEvents.OnLobbyCreated -= OnLobbyEntered;
//             LobbyEvents.OnLobbyJoined -= OnLobbyEntered;
//             LobbyEvents.OnLobbyRejoined -= OnLobbyRejoined;
//             LobbyEvents.OnLobbyLeft -= OnLobbyExited;
//             LobbyEvents.OnLobbyKicked -= OnLobbyExited;

//             GameEvents.OnGameStarted -= OnGameStarted;
//             GameEvents.OnGameEnded -= OnGameEnded;
//         }

//         private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//         {
//             if (showDebugMessages) Debug.Log($"Scene Loaded: {scene.name} in mode {mode}");
//         }

//         private void OnLobbyEntered(OperationResult result)
//         {
//             if (result.Status == ResultStatus.Success)
//             {
//                 GameObject gamelobbyManagerObject = new("GameLobbyManager");
//                 gamelobbyManagerObject.AddComponent<GameLobbyManager>();
//                 GameLobbyManager.Instance.Initialize((Lobby)result.Data);

//                 TransitionToScene("Lobby");
//             }
//         }

//         private void OnLobbyRejoined(OperationResult result)
//         {
//             if (result.Status == ResultStatus.Success)
//             {
//                 GameObject gamelobbyManagerObject = new("GameLobbyManager");
//                 gamelobbyManagerObject.AddComponent<GameLobbyManager>();
//                 GameLobbyManager.Instance.Initialize((Lobby)result.Data);

//                 TransitionToScene("Lobby");
//             }
//             else TransitionToScene("Main");
//         }

//         private void OnLobbyExited(OperationResult result)
//         {
//             if (result.Status == ResultStatus.Success)
//             {
//                 TransitionToScene("Main");

//                 GameLobbyManager.Instance.Cleanup();
//                 Destroy(GameLobbyManager.Instance.gameObject);
//             }
//         }

//         private void OnGameStarted(bool success, string relayJoinCode)
//         {
//             if (success) SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
//         }

//         private void OnGameEnded()
//         {
//             SceneManager.UnloadSceneAsync("Loading");
//         }

//         private void TransitionToScene(string sceneName)
//         {
//             if (showDebugMessages) Debug.Log($"Transitioning to scene: {sceneName}");
//             SceneManager.LoadSceneAsync(sceneName);
//         }
//     }
// }