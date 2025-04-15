using Assets.Scripts.Game.Types;
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
        /// Triggered when the list of lobbies is queried.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbiesQueriedHandler(OperationResult result);
        public static event LobbiesQueriedHandler OnLobbiesQueried;

        /// <summary>
        /// Triggered when the lobby is created or there is an error in creating the lobby.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyCreatedHandler(OperationResult result);
        public static event LobbyCreatedHandler OnLobbyCreated;

        /// <summary>
        /// Triggered when the lobby is joined or there is an error in joining the lobby.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyJoinedHandler(OperationResult result);
        public static event LobbyJoinedHandler OnLobbyJoined;


        /// <summary>
        /// Triggered when the lobby is rejoined or there is an error in rejoining the lobby.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyRejoinedHandler(OperationResult result);
        public static event LobbyRejoinedHandler OnLobbyRejoined;

        /// <summary>
        /// Triggered when the lobby is left or there is an error in leaving the lobby.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyLeftHandler(OperationResult result);
        public static event LobbyLeftHandler OnLobbyLeft;

        /// <summary>
        /// Triggered when the player is kicked from the lobby or there is an error in kicking the player.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyKickedHandler(OperationResult result);
        public static event LobbyKickedHandler OnLobbyKicked;

        /// <summary>
        /// Triggered when a new host is assigned to the lobby.
        /// </summary>
        /// <param name="playerId">The id of the new host.</param>
        public delegate void HostMigratedHandler(string playerId);
        public static event HostMigratedHandler OnHostMigrated;

        /// <summary>
        /// Triggered when the maximum number of players in the lobby is changed.
        /// </summary>
        /// <param name="maxPlayers">The new maximum number of players.</param>
        public delegate void MaxPlayersChangedHandler(int maxPlayers);
        public static event MaxPlayersChangedHandler OnMaxPlayersChanged;

        /// <summary>
        /// Triggered when the privacy setting of the lobby is changed.
        /// </summary>
        /// <param name="isPrivate">The new state of lobby privacy.</param>
        public delegate void PrivacyChangedHandler(bool isPrivate);
        public static event PrivacyChangedHandler OnPrivacyChanged;

        /// <summary>
        /// Triggered when the name of the lobby is changed.
        /// </summary>
        /// <param name="name">The new name of the lobby.</param>
        public delegate void NameChangedHandler(string name);
        public static event NameChangedHandler OnNameChanged;

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
        /// Triggered when a player is kicked from the lobby or there is an error in kicking the player.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void PlayerKickedHandler(OperationResult result);
        public static event PlayerKickedHandler OnPlayerKicked;
        #endregion

        #region Invocations
        public static void InvokeLobbiesQueried(OperationResult result) => OnLobbiesQueried?.Invoke(result);
        public static void InvokeLobbyCreated(OperationResult result) => OnLobbyCreated?.Invoke(result);
        public static void InvokeLobbyJoined(OperationResult result) => OnLobbyJoined?.Invoke(result);
        public static void InvokeLobbyRejoined(OperationResult result) => OnLobbyRejoined?.Invoke(result);
        public static void InvokeLobbyLeft(OperationResult result) => OnLobbyLeft?.Invoke(result);
        public static void InvokeLobbyKicked(OperationResult result) => OnLobbyKicked?.Invoke(result);
        public static void InvokeHostMigrated(string playerId) => OnHostMigrated?.Invoke(playerId);
        public static void InvokeMaxPlayersChanged(int maxPlayers) => OnMaxPlayersChanged?.Invoke(maxPlayers);
        public static void InvokePrivacyChanged(bool isPrivate) => OnPrivacyChanged?.Invoke(isPrivate);
        public static void InvokeNameChanged(string name) => OnNameChanged?.Invoke(name);
        public static void InvokePlayerJoined(string playerId) => OnPlayerJoined?.Invoke(playerId);
        public static void InvokePlayerLeft(string playerId) => OnPlayerLeft?.Invoke(playerId);
        public static void InvokePlayerKicked(OperationResult result) => OnPlayerKicked?.Invoke(result);
        #endregion
    }
}