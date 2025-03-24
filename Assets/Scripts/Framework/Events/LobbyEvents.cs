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
        /// <param name="lobby">The lobby that was left.</param>
        public delegate void LobbyLeftHandler(Lobby lobby);
        public static event LobbyLeftHandler OnLobbyLeft;

        /// <summary>
        /// Triggered when kicked from a lobby.
        /// </summary>
        /// <param name="lobby">The lobby kicked from.</param>
        public delegate void LobbyKickedHandler(Lobby lobby);
        public static event LobbyKickedHandler OnLobbyKicked;

        /// <summary>
        /// Triggered when a player joins the lobby.
        /// </summary>
        /// <param name="playerName">Name of the player who joined.</param>
        public delegate void PlayerJoinedHandler(string playerName);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby.
        /// </summary>
        /// <param name="playerName">Name of the player who left.</param>
        public delegate void PlayerLeftHandler(string playerName);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="playerName">Name of the player who was kicked.</param>
        public delegate void PlayerKickedHandler(string playerName);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when the lobby host has migrated.
        /// </summary>
        /// <param name="newHostId">The ID of the new lobby host.</param>
        public delegate void HostMigratedHandler(string newHostId);
        public static event HostMigratedHandler OnHostMigrated;

        /// <summary>
        /// Triggered when any lobby data is updated.
        /// </summary>
        /// <param name="lobby">The updated lobby data.</param>
        public delegate void LobbyUpdated(Lobby lobby);
        public static event LobbyUpdated OnLobbyUpdated;

        /// <summary>
        /// Triggered when the list of lobbies is updated.
        /// </summary>
        /// <param name="lobbies">The updated list of lobbies.</param>
        public delegate void LobbyListUpdatedHandler(List<Lobby> lobbies);
        public static event LobbyListUpdatedHandler OnLobbyListUpdated;
        #endregion

        #region Invocations
        public static void InvokeLobbyCreated(Lobby lobby) => OnLobbyCreated?.Invoke(lobby);
        public static void InvokeLobbyJoined(Lobby lobby) => OnLobbyJoined?.Invoke(lobby);
        public static void InvokeLobbyLeft(Lobby lobby) => OnLobbyLeft?.Invoke(lobby);
        public static void InvokeLobbyKicked(Lobby lobby) => OnLobbyKicked?.Invoke(lobby);
        public static void InvokePlayerJoined(string playerName) => OnPlayerJoined?.Invoke(playerName);
        public static void InvokePlayerLeft(string playerName) => OnPlayerLeft?.Invoke(playerName);
        public static void InvokePlayerKicked(string playerName) => OnPlayerKicked?.Invoke(playerName);
        public static void InvokeHostMigrated(string newHostId) => OnHostMigrated?.Invoke(newHostId);
        public static void InvokeLobbyUpdated(Lobby lobby) => OnLobbyUpdated?.Invoke(lobby);
        public static void InvokeLobbyListUpdated(List<Lobby> lobbies) => OnLobbyListUpdated?.Invoke(lobbies);
        #endregion
    }
}