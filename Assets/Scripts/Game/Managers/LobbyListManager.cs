using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Manages the list of available lobbies and handles joining operations.
    /// </summary>
    public class LobbyListManager : MonoBehaviour
    {
        [Header("List Components")]
        [SerializeField] private GameObject lobbyItemPrefab;
        [SerializeField] private Transform lobbyListContainer;

        private string _selectedLobbyId;
        public string SelectedLobbyId => _selectedLobbyId;

        /// <summary>
        /// Sets the selected lobby.
        /// </summary>
        /// <param name="lobbyId">The ID of the selected lobby.</param>
        public void SelectLobby(string lobbyId)
        {
            _selectedLobbyId = lobbyId;
        }
        /// <summary>
        /// Clears the selected lobby.
        /// </summary>
        public void ClearSelection()
        {
            _selectedLobbyId = null;
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
        /// Updates the lobby list with the provided lobbies.
        /// </summary>
        /// <param name="lobbies">The list of lobbies to display.</param>
        public void UpdateLobbyList(List<Lobby> lobbies)
        {
            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>())
                Destroy(entry.gameObject);

            foreach (Lobby lobby in lobbies)
            {
                LobbyListEntry lobbyItem = Instantiate(lobbyItemPrefab, lobbyListContainer).GetComponent<LobbyListEntry>();
                lobbyItem.SetLobby(lobby);
            }
        }
    }
}