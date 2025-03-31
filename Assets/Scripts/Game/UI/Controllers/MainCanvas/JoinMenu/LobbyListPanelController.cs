using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;
using Assets.Scripts.Game.Managers;
using System.Threading.Tasks;

// fix scrolling not working

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.JoinMenu
{
    /// <summary>
    /// Manages the list of available lobbies and handles joining operations.
    /// </summary>
    public class LobbyListPanelController : MonoBehaviour
    {
        [Header("List Components")]
        [SerializeField] private LobbyListEntry lobbyListEntry;
        [SerializeField] private Transform lobbyListContainer;

        [Header("Entry Size Settings")]
        [SerializeField] private float entryStartAnchorX = 0.02f;
        [SerializeField] private float entryEndAnchorX = 0.98f;
        [SerializeField] private float entryHeight = 0.85f;
        [SerializeField] private float verticalGap = 0.15f;

        public string SelectedLobbyId { get; private set; }

        private void OnEnable()
        {
            LobbyEvents.OnLobbyQueryResponse += RefreshLobbyList;
        }

        private void OnDisable()
        {
            LobbyEvents.OnLobbyQueryResponse -= RefreshLobbyList;
        }

        /// <summary>
        /// Sets the selected lobby.
        /// </summary>
        /// <param name="lobbyId">The ID of the selected lobby.</param>
        public void SelectLobby(string lobbyId)
        {
            SelectedLobbyId = lobbyId;
        }

        /// <summary>
        /// Clears the selected lobby.
        /// </summary>
        public void ClearSelection()
        {
            SelectedLobbyId = null;
        }

        public async void SpawnTestLobbies()
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1500);
                GameLobbyManager.Instance.CreateLobby($"TestLobby {i}", false, 2);
            }
        }

        /// <summary>
        /// Refreshes the list of available lobbies.
        /// </summary>
        /// <param name="result">Used for notification.</param>
        /// <param name="lobbies">The list of lobbies to display.</param>
        public void RefreshLobbyList(OperationResult result, List<Lobby> lobbies)
        {
            ClearSelection();

            foreach (LobbyListEntry entry in lobbyListContainer.GetComponentsInChildren<LobbyListEntry>())
                Destroy(entry.gameObject);

            for (int i = 0; i < lobbies.Count; i++)
            {
                LobbyListEntry lobbyEntry = Instantiate(lobbyListEntry, lobbyListContainer);
                RectTransform entryRect = lobbyEntry.GetComponent<RectTransform>();

                entryRect.anchorMin = new Vector2(entryStartAnchorX, 1f - ((entryHeight + verticalGap) * (i + 1)));
                entryRect.anchorMax = new Vector2(entryEndAnchorX, entryRect.anchorMin.y + entryHeight);

                lobbyEntry.SetLobby(lobbies[i]);
            }
        }
    }
}