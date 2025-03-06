using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to the lobby service in Unity Services.
    /// </summary>
    public static class LobbyEvents
    {
        #region Events

        /// <summary>
        /// Triggered when a new lobby is successfully created.
        /// </summary>
        /// <param name="lobby">The created lobby data.</param>
        public delegate void LobbyCreatedHandler(Lobby lobby);
        public static event LobbyCreatedHandler OnLobbyCreated;

        /// <summary>
        /// Triggered when successfully joining an existing lobby.
        /// </summary>
        /// <param name="lobby">The joined lobby data.</param>
        public delegate void LobbyJoinedHandler(Lobby lobby);
        public static event LobbyJoinedHandler OnLobbyJoined;

        /// <summary>
        /// Triggered when successfully leaving a lobby.
        /// </summary>
        public delegate void LobbyLeftHandler();
        public static event LobbyLeftHandler OnLobbyLeft;

        /// <summary>
        /// Triggered when successfully kicked from a lobby.
        /// </summary>
        public delegate void LobbyKickedHandler();
        public static event LobbyKickedHandler OnLobbyKicked;

        /// <summary>
        /// Triggered when any lobby operation fails.
        /// </summary>
        /// <param name="errorCode">The error code from the lobby service.</param>
        /// <param name="errorMessage">The error message.</param>
        public delegate void LobbyErrorHandler(string errorCode, string errorMessage);
        public static event LobbyErrorHandler OnLobbyError;

        /// <summary>
        /// Triggered when a new player joins the lobby.
        /// </summary>
        /// <param name="lobbyId">ID of the lobby the player joined.</param>
        /// <param name="player">Data of the player who joined.</param>
        public delegate void PlayerJoinedHandler(string lobbyId, Player player);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby voluntarily.
        /// </summary>
        /// <param name="playerId">ID of the player who left.</param>
        public delegate void PlayerLeftHandler(string playerId);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="playerId">ID of the player who was kicked.</param>
        public delegate void PlayerKickedHandler(string playerId);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when any lobby data is updated.
        /// </summary>
        /// <param name="lobby">The updated lobby data.</param>
        public delegate void LobbyUpdatedHandler(Lobby lobby);
        public static event LobbyUpdatedHandler OnLobbyUpdated;


        /// <summary>
        /// Triggered when specific lobby data fields change.
        /// </summary>
        /// <param name="lobby">The updated lobby data.</param>
        /// <param name="changedData">Dictionary of changed data fields.</param>
        public delegate void LobbyDataChangedHandler(Lobby lobby, Dictionary<string, DataObject> changedData);
        public static event LobbyDataChangedHandler OnLobbyDataChanged;


        public delegate void PlayerUpdatedHandler(Player player);
        public static event PlayerUpdatedHandler OnPlayerUpdated;

        /// <summary>
        /// Triggered when a player's data is updated.
        /// </summary>
        /// <param name="playerId">ID of the player whose data changed.</param>
        /// <param name="changedData">Dictionary of changed player data fields.</param>
        public delegate void PlayerDataChangedHandler(string playerId, Dictionary<string, PlayerDataObject> changedData);
        public static event PlayerDataChangedHandler OnPlayerDataChanged;

        /// <summary>
        /// Triggered when the list of lobbies changes.
        /// </summary>
        /// <param name="lobbies">The updated list of lobbies.</param>
        public delegate void LobbyListChangedHandler(List<Lobby> lobbies);
        public static event LobbyListChangedHandler OnLobbyListChanged;

        /// <summary>
        /// Triggered when the host of the lobby migrates to a new player.
        /// </summary>
        /// <param name="newHostId">The PlayerId of the new host.</param>
        public delegate void LobbyHostMigratedHandler(string newHostId);
        public static event LobbyHostMigratedHandler OnLobbyHostMigrated;

        #endregion

        #region Invocations
        public static void InvokeLobbyCreated(Lobby lobby) => OnLobbyCreated?.Invoke(lobby);
        public static void InvokeLobbyJoined(Lobby lobby) => OnLobbyJoined?.Invoke(lobby);
        public static void InvokeLobbyLeft() => OnLobbyLeft?.Invoke();
        public static void InvokeLobbyKicked() => OnLobbyKicked?.Invoke();
        public static void InvokeLobbyError(string errorCode, string errorMessage) => OnLobbyError?.Invoke(errorCode, errorMessage);
        public static void InvokePlayerJoined(string lobbyId, Player player) => OnPlayerJoined?.Invoke(lobbyId, player);
        public static void InvokePlayerLeft(string playerId) => OnPlayerLeft?.Invoke(playerId);
        public static void InvokePlayerKicked(string playerId) => OnPlayerKicked?.Invoke(playerId);
        public static void InvokeLobbyUpdated(Lobby lobby) => OnLobbyUpdated?.Invoke(lobby);
        public static void InvokeLobbyDataChanged(Lobby lobby, Dictionary<string, DataObject> changedData) =>
            OnLobbyDataChanged?.Invoke(lobby, changedData);
        public static void InvokePlayerDataChanged(string playerId, Dictionary<string, PlayerDataObject> changedData) =>
            OnPlayerDataChanged?.Invoke(playerId, changedData);

        public static void InvokePlayerUpdated(Player player) => OnPlayerUpdated?.Invoke(player);
        public static void InvokeLobbyListChanged(List<Lobby> lobbies) => OnLobbyListChanged?.Invoke(lobbies);
        public static void InvokeLobbyHostMigrated(string newHostId) => OnLobbyHostMigrated?.Invoke(newHostId);

        #endregion

        #region Cleanup

        /// <summary>
        /// Clears all event subscriptions to prevent memory leaks.
        /// </summary>
        public static void ClearAllEvents()
        {
            OnLobbyCreated = null;
            OnLobbyJoined = null;
            OnLobbyLeft = null;
            OnLobbyKicked = null;
            OnLobbyError = null;
            OnPlayerJoined = null;
            OnPlayerLeft = null;
            OnPlayerKicked = null;
            OnLobbyUpdated = null;
            OnLobbyDataChanged = null;
            OnPlayerDataChanged = null;
            OnPlayerUpdated = null;
            OnLobbyListChanged = null;
            OnLobbyHostMigrated = null;
        }

        #endregion
    }
}