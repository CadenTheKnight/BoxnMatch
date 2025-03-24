using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.Events
{
    /// <summary>
    /// Contains game-specific events for lobby interactions.
    /// </summary>
    public static class LobbyEvents
    {
        #region Events
        public delegate void LobbyUpdated();
        public static event LobbyUpdated OnLobbyUpdated;

        /// <summary>
        /// Triggered when a lobby is single clicked in the lobby list.
        /// </summary>
        /// <param name="lobbyId">The ID of the selected lobby.</param>
        /// <param name="lobbyListEntry">The list entry of the selected lobby.</param>
        public delegate void LobbySelectedHandler(string lobbyId, LobbyListEntry lobbyListEntry);
        public static event LobbySelectedHandler OnLobbySelected;

        /// <summary>
        /// Triggered when a lobby is double clicked in the lobby list.
        /// </summary>
        /// <param name="lobbyId">The ID of the selected lobby.</param>
        public delegate void LobbyDoubleClickedHandler(string lobbyId);
        public static event LobbyDoubleClickedHandler OnLobbyDoubleClicked;

        /// <summary>
        /// Triggered when a player toggles their ready status.
        /// </summary>
        /// <param name="playerId">ID of the player who changed their ready status.</param>
        /// <param name="isReady">True if the player is ready, false if not.</param>
        public delegate void PlayerReadyChangedHandler(string playerId, bool isReady);
        public static event PlayerReadyChangedHandler OnPlayerReadyChanged;

        /// <summary>
        /// Triggered when all players in the lobby are ready.
        /// </summary>
        public delegate void AllPlayersReadyHandler();
        public static event AllPlayersReadyHandler OnAllPlayersReady;

        /// <summary>
        /// Triggered when at least one player in the lobby is not ready.
        /// </summary>
        /// <param name="playersReady">The number of players that are ready.</param>
        /// <param name="maxPlayerCount">The maximum number of players in the lobby.</param>
        public delegate void NotAllPlayersReadyHandler(int playersReady, int maxPlayerCount);
        public static event NotAllPlayersReadyHandler OnNotAllPlayersReady;

        /// <summary>
        /// Triggered when the match has started.
        /// </summary>
        public delegate void MatchStartedHandler();
        public static event MatchStartedHandler OnMatchStarted;

        /// <summary>
        /// Triggered when a lobby data property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public delegate void LobbyDataChangedHandler(string propertyName, object oldValue, object newValue);
        public static event LobbyDataChangedHandler OnLobbyDataChanged;

        /// <summary>
        /// Triggered when a player data property has changed.
        /// </summary>
        /// <param name="playerId">The ID of the player who changed their data.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public delegate void LobbyPlayerDataChangedHandler(string playerId, string propertyName, string oldValue, string newValue);
        public static event LobbyPlayerDataChangedHandler OnLobbyPlayerDataChanged;
        #endregion

        #region Invocations
        public static void InvokeLobbyUpdated() => OnLobbyUpdated?.Invoke();
        public static void InvokeLobbySelected(string lobbyId, LobbyListEntry lobbyListEntry) => OnLobbySelected?.Invoke(lobbyId, lobbyListEntry);
        public static void InvokeLobbyDoubleClicked(string lobbyId) => OnLobbyDoubleClicked?.Invoke(lobbyId);
        public static void InvokePlayerReadyChanged(string playerId, bool isReady) => OnPlayerReadyChanged?.Invoke(playerId, isReady);
        public static void InvokeAllPlayersReady() => OnAllPlayersReady?.Invoke();
        public static void InvokeNotAllPlayersReady(int playersReady, int maxPlayerCount) => OnNotAllPlayersReady?.Invoke(playersReady, maxPlayerCount);
        public static void InvokeMatchStarted() => OnMatchStarted?.Invoke();
        public static void InvokeLobbyDataChanged(string propertyName, object oldValue, object newValue) => OnLobbyDataChanged?.Invoke(propertyName, oldValue, newValue);
        public static void InvokePlayerDataChanged(string playerId, string propertyName, string oldValue, string newValue) => OnLobbyPlayerDataChanged?.Invoke(playerId, propertyName, oldValue, newValue);
        #endregion
    }
}