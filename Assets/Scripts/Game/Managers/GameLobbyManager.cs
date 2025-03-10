using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles high-level lobby operations and game-specific lobby functionality.
    /// Acts as a bridge between the framework's LobbyManager and the game's specific needs.
    /// </summary>
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private List<LobbyPlayerData> lobbyPlayersData = new();
        private LobbyPlayerData localLobbyPlayerData;
        private LobbyData lobbyData;
        private bool inGame = false;

        private void OnEnable()
        {
            Framework.Events.LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            Framework.Events.LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        /// <summary>
        /// Creates a new lobby with the given parameters.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby.</param>
        /// <param name="isPrivate">Whether the lobby is private or not.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));

            lobbyData = new();
            lobbyData.Initialize(lobbyName, maxPlayers, isPrivate);

            return await LobbyManager.Instance.CreateLobby(lobbyData.Serialize(), playerData.Serialize());
        }

        /// <summary>
        /// Joins a lobby using the lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
            return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode, playerData.Serialize());
        }

        /// <summary>
        /// Joins the selected lobby using the lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId)
        {
            LobbyPlayerData playerData = new();
            playerData.Initialize(AuthenticationManager.Instance.PlayerId, PlayerPrefs.GetString("PlayerName"));
            return await LobbyManager.Instance.JoinLobbyById(lobbyId, playerData.Serialize());
        }

        /// <summary>
        /// Checks if the given player is ready.
        /// </summary>
        /// <param name="playerId">The ID of the player to check.</param>
        /// <returns>True if the player is ready, false otherwise.</returns>
        public bool IsPlayerReady(string playerId)
        {
            return lobbyPlayersData.FirstOrDefault(player => player.PlayerId == playerId)?.IsReady ?? false;
        }

        /// <summary>
        /// Toggle the ready status of the current player.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> TogglePlayerReady()
        {
            localLobbyPlayerData.IsReady = !localLobbyPlayerData.IsReady;
            return await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize());
        }

        private async void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playersData = LobbyManager.Instance.GetPlayersData();
            lobbyPlayersData.Clear();

            int playersReady = 0;

            foreach (Dictionary<string, PlayerDataObject> playerData in playersData)
            {
                LobbyPlayerData lobbyPlayerData = new();
                lobbyPlayerData.Initialize(playerData);

                if (lobbyPlayerData.IsReady)
                    playersReady++;

                if (lobbyPlayerData.PlayerId == AuthenticationManager.Instance.PlayerId)
                    localLobbyPlayerData = lobbyPlayerData;

                lobbyPlayersData.Add(lobbyPlayerData);
            }

            lobbyData = new();
            lobbyData.Initialize(lobby.Data);

            LobbyEvents.InvokeLobbyUpdated();

            if (playersReady == LobbyManager.Instance.MaxPlayers)
                LobbyEvents.InvokeAllPlayersReady();
            else
                LobbyEvents.InvokeNotAllPlayersReady(playersReady, LobbyManager.Instance.MaxPlayers);

            if (lobbyData.RelayJoinCode != default && !inGame)
            {
                await JoinRelayServer(lobbyData.RelayJoinCode);
                SceneManager.LoadSceneAsync(lobbyData.MapSceneName);
            }
        }

        public List<LobbyPlayerData> GetPlayers()
        {
            return lobbyPlayersData;
        }

        public int GetMapIndex()
        {
            return lobbyData.MapIndex;
        }

        public async Task<OperationResult> SetSelectedMap(int mapIndex, string mapSceneName)
        {
            lobbyData.MapIndex = mapIndex;
            lobbyData.MapSceneName = mapSceneName;
            return await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());
        }

        public async Task StartGame()
        {
            string relayJoinCode = await RelayManager.Instance.CreateRelay(LobbyManager.Instance.MaxPlayers);
            inGame = true;

            lobbyData.RelayJoinCode = relayJoinCode;
            await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize(), allocationId, connectionData);

            SceneManager.LoadSceneAsync(lobbyData.MapSceneName);
        }

        private async Task<bool> JoinRelayServer(string relayJoinCode)
        {
            inGame = true;
            await RelayManager.Instance.JoinRelay(relayJoinCode);

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.Instance.UpdatePlayerData(localLobbyPlayerData.PlayerId, localLobbyPlayerData.Serialize(), allocationId, connectionData);

            return true;
        }

        public async void SetAllPlayersUnready()
        {
            foreach (LobbyPlayerData playerData in lobbyPlayersData)
                playerData.IsReady = false;

            await LobbyManager.Instance.UpdateAllPlayerData(lobbyPlayersData.Select(player => player.Serialize()).ToList());
        }
    }
}