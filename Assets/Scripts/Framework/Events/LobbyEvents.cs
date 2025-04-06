using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to the lobby service in Unity Services.
    /// </summary>
    public static class LobbyEvents
    {
        #region Events

        /// <summary>
        /// Triggered when a new host is assigned to the lobby.
        /// </summary>
        /// <param name="playerId">The id of the new host.</param>
        public delegate void LobbyHostMigratedHandler(string playerId);
        public static event LobbyHostMigratedHandler OnLobbyHostMigrated;

        /// <summary>
        /// Triggered when a player joins the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who joined.</param>
        public delegate void PlayerJoinedHandler(string playerId);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who left.</param>
        public delegate void PlayerLeftHandler(string playerId);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who was kicked.</param>
        public delegate void PlayerKickedHandler(string playerId);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when the lobby map index is changed.
        /// </summary>
        /// <param name="index">The new map index.</param>
        public delegate void LobbyMapIndexChangedHandler(int index);
        public static event LobbyMapIndexChangedHandler OnLobbyMapIndexChanged;

        /// <summary>
        /// Triggered when the lobby round count is changed.
        /// </summary>
        /// <param name="count">The new round count.</param>
        public delegate void LobbyRoundCountChangedHandler(int count);
        public static event LobbyRoundCountChangedHandler OnLobbyRoundCountChanged;

        /// <summary>
        /// Triggered when the lobby round time is changed.
        /// </summary>
        /// <param name="time">The new round time.</param>
        public delegate void LobbyRoundTimeChangedHandler(int time);
        public static event LobbyRoundTimeChangedHandler OnLobbyRoundTimeChanged;

        /// <summary>
        /// Triggered when the lobby game mode is changed.
        /// </summary>
        /// <param name="mode">The new game mode.</param>
        public delegate void LobbyGameModeChangedHandler(GameMode mode);
        public static event LobbyGameModeChangedHandler OnLobbyGameModeChanged;

        /// <summary>
        /// Triggered when the lobby status is changed.
        /// </summary>
        /// <param name="status">The new status of the lobby.</param>
        public delegate void LobbyStatusChangedHandler(LobbyStatus status);
        public static event LobbyStatusChangedHandler OnLobbyStatusChanged;

        /// <summary>
        /// Triggered when a player is connecting to the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who is connecting.</param>
        public delegate void PlayerConnectingHandler(string playerId);
        public static event PlayerConnectingHandler OnPlayerConnecting;

        /// <summary>
        /// Triggered when a player is connected to the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who is connected.</param>
        public delegate void PlayerConnectedHandler(string playerId);
        public static event PlayerConnectedHandler OnPlayerConnected;

        /// <summary>
        /// Triggered when a player is disconnected from the lobby.
        /// </summary>
        /// <param name="playerId">The id of the player who is disconnected.</param>
        public delegate void PlayerDisconnectedHandler(string playerId);
        public static event PlayerDisconnectedHandler OnPlayerDisconnected;
        #endregion

        #region Invocations
        public static void InvokeLobbyHostMigrated(string playerId) => OnLobbyHostMigrated?.Invoke(playerId);
        public static void InvokePlayerJoined(string playerId) => OnPlayerJoined?.Invoke(playerId);
        public static void InvokePlayerLeft(string playerId) => OnPlayerLeft?.Invoke(playerId);
        public static void InvokePlayerKicked(string playerId) => OnPlayerKicked?.Invoke(playerId);
        public static void InvokeLobbyMapIndexChanged(int index) => OnLobbyMapIndexChanged?.Invoke(index);
        public static void InvokeLobbyRoundCountChanged(int count) => OnLobbyRoundCountChanged?.Invoke(count);
        public static void InvokeLobbyRoundTimeChanged(int time) => OnLobbyRoundTimeChanged?.Invoke(time);
        public static void InvokeLobbyGameModeChanged(GameMode mode) => OnLobbyGameModeChanged?.Invoke(mode);
        public static void InvokeLobbyStatusChanged(LobbyStatus status) => OnLobbyStatusChanged?.Invoke(status);
        public static void InvokePlayerConnecting(string playerId) => OnPlayerConnecting?.Invoke(playerId);
        public static void InvokePlayerConnected(string playerId) => OnPlayerConnected?.Invoke(playerId);
        public static void InvokePlayerDisconnected(string playerId) => OnPlayerDisconnected?.Invoke(playerId);
        #endregion
    }
}