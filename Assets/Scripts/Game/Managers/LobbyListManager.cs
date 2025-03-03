using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Unity.Services.Lobbies;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages the list of available lobbies and handles joining operations.
    /// </summary>
    public class LobbyListManager : Singleton<LobbyListManager>
    {
        public event Action<Lobby> OnLobbySelected;
        public event Action OnLobbySelectionCleared;

        private Lobby _selectedLobby;

        /// <summary>
        /// Gets the currently selected lobby.
        /// </summary>
        public Lobby SelectedLobby => _selectedLobby;

        private List<Lobby> _availableLobbies = new();

        /// <summary>
        /// Gets the list of available lobbies.
        /// </summary>
        public IReadOnlyList<Lobby> AvailableLobbies => _availableLobbies;

        /// <summary>
        /// Sets the selected lobby.
        /// </summary>
        /// <param name="lobby">The lobby to select.</param>
        public void SelectLobby(Lobby lobby)
        {
            _selectedLobby = lobby;
            OnLobbySelected?.Invoke(lobby);
        }

        /// <summary>
        /// Clears the selected lobby.
        /// </summary>
        public void ClearSelection()
        {
            _selectedLobby = null;
            OnLobbySelectionCleared?.Invoke();
        }

        /// <summary>
        /// Refreshes the list of available lobbies.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> RefreshLobbyList()
        {
            ClearSelection();

            return await LobbyManager.Instance.GetLobbies();
        }

        /// <summary>
        /// Joins a lobby using the lobby code.
        /// </summary>
        /// <param name="lobbyCode">The lobby code to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new() { Player = PlayerDataManager.Instance.GetPlayer() };

            return await LobbyManager.Instance.JoinLobbyByCode(lobbyCode, joinLobbyByCodeOptions);
        }

        /// <summary>
        /// Joins the selected lobby using the lobby ID.
        /// </summary>
        /// <param name="lobbyId">The lobby ID to join.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinSelectedLobby(string lobbyId)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new() { Player = PlayerDataManager.Instance.GetPlayer() };

            return await LobbyManager.Instance.JoinLobbyById(lobbyId, joinLobbyByIdOptions);
        }
    }
}