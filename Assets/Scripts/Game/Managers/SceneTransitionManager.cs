using UnityEngine;
using Unity.Services.Lobbies;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using System.Linq;
using Assets.Scripts.Framework.Managers;
using System.Collections.Generic;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Framework.Types;
using System.Threading.Tasks;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages scene transitions based on the game state.
    /// </summary>
    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        private void Start()
        {
            AuthenticationEvents.OnAuthenticated += OnAuthenticated;

            LobbyEvents.OnLobbyCreated += OnLobbyCreated;
            LobbyEvents.OnLobbyJoined += OnLobbyJoined;
            LobbyEvents.OnLobbyLeft += OnLobbyLeft;
            LobbyEvents.OnLobbyKicked += OnLobbyKicked;
        }

        private void OnDestroy()
        {
            AuthenticationEvents.OnAuthenticated -= OnAuthenticated;

            LobbyEvents.OnLobbyCreated -= OnLobbyCreated;
            LobbyEvents.OnLobbyJoined -= OnLobbyJoined;
            LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
            LobbyEvents.OnLobbyKicked -= OnLobbyKicked;
        }

        private async void OnAuthenticated(string playerName)
        {
            LoadingStatus loadingStatus = FindObjectOfType<LoadingStatus>();
            if (loadingStatus != null)
            {
                loadingStatus.UpdateStatus("Checking for joined lobbies...");
                await Task.Yield();
            }

            try
            {
                await Task.Delay(1500);

                var lobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();

                if (lobbyIds.Count == 0)
                {
                    loadingStatus.StopLoading();
                    SceneManager.LoadSceneAsync("Main");
                    return;
                }

                Debug.Log($"Found {lobbyIds.Count} joined" + (lobbyIds.Count > 1 ? " lobbies" : " lobby") + $"{string.Join(", ", lobbyIds)}");
                loadingStatus.UpdateStatus("Found joined lobby");

                if (lobbyIds.Count > 1)
                {
                    Debug.LogWarning($"Player is in multiple lobbies. Cleaning up old lobbies...");
                    loadingStatus.UpdateStatus("Cleaning up old lobbies...");

                    var mostRecentLobby = lobbyIds[0];
                    var oldLobbies = lobbyIds.Skip(1).ToList();

                    foreach (var oldLobbyId in oldLobbies)
                    {
                        try
                        {
                            Debug.Log($"Leaving old lobby: {oldLobbyId}");
                            loadingStatus.UpdateStatus($"Leaving old lobby: {oldLobbyId}");
                            await Task.Delay(1500);

                            await LobbyService.Instance.RemovePlayerAsync(oldLobbyId, AuthenticationManager.Instance.PlayerId);
                        }
                        catch (System.Exception ex)
                        {
                            loadingStatus.UpdateStatus("Error leaving old lobby");
                            Debug.LogError($"Error leaving old lobby {oldLobbyId}: {ex.Message}");
                        }
                    }

                    lobbyIds = new List<string> { mostRecentLobby };
                }

                if (lobbyIds.Count > 0)
                {
                    string lobbyId = lobbyIds[0];

                    loadingStatus.UpdateStatus("Attempting to rejoin lobby...");
                    Debug.Log($"Attempting to rejoin lobby: {lobbyId}");

                    try
                    {
                        await Task.Delay(1500);

                        var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);

                        loadingStatus.UpdateStatus($"Successfully found lobby {lobby.Name} with {lobby.Players.Count} players");
                        Debug.Log($"Successfully found lobby {lobby.Name} with {lobby.Players.Count} players");

                        await Task.Delay(1500);

                        OperationResult result = await GameLobbyManager.Instance.JoinLobbyById(lobbyId);
                        if (result.Status == ResultStatus.Success)
                        {
                            loadingStatus.UpdateStatus($"Rejoining lobby: {lobbyId}");
                            loadingStatus.StopLoading();
                            Debug.Log($"Rejoining lobby: {lobbyId}");
                            SceneManager.LoadSceneAsync("Lobby");
                        }
                        else
                        {
                            loadingStatus.UpdateStatus($"Failed to rejoin lobby: {result.Message}");
                            loadingStatus.StopLoading();
                            Debug.LogError($"Failed to rejoin lobby: {result.Message}");
                            SceneManager.LoadSceneAsync("Main");
                        }
                    }
                    catch (LobbyServiceException ex)
                    {
                        loadingStatus.UpdateStatus($"Error getting lobby details: {ex.Message}. Code: {ex.ErrorCode}");
                        loadingStatus.StopLoading();
                        Debug.LogError($"Error getting lobby details: {ex.Message}. Code: {ex.ErrorCode}");
                        SceneManager.LoadSceneAsync("Main");
                    }
                }
                else
                {
                    loadingStatus.UpdateStatus("No lobbies found");
                    loadingStatus.StopLoading();
                    SceneManager.LoadSceneAsync("Main");
                }
            }
            catch (System.Exception ex)
            {
                loadingStatus.UpdateStatus("Error in OnAuthenticated");
                loadingStatus.StopLoading();
                Debug.LogError($"Error in OnAuthenticated: {ex.Message}");
                SceneManager.LoadSceneAsync("Main");
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