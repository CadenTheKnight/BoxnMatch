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
        /// Triggered when the list of joined lobbies is retrieved.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void JoinedLobbiesRetrievedHandler(OperationResult result);
        public static event JoinedLobbiesRetrievedHandler OnJoinedLobbiesRetrieved;

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
        /// <param name="playerIndex">The index of the player who left.</param>
        public delegate void PlayerLeftHandler(int playerIndex);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby or there is an error in kicking the player.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void PlayerKickedHandler(OperationResult result);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when the lobby data is updated or there is an error in updating the lobby data.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void LobbyDataUpdatedHandler(OperationResult result);
        public static event LobbyDataUpdatedHandler OnLobbyDataUpdated;

        /// <summary>
        /// Triggered when the player data is updated or there is an error in updating the player data.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void PlayerDataUpdatedHandler(OperationResult result);
        public static event PlayerDataUpdatedHandler OnPlayerDataUpdated;


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
        public static void InvokeJoinedLobbiesRetrieved(OperationResult result) => OnJoinedLobbiesRetrieved?.Invoke(result);
        public static void InvokeLobbiesQueried(OperationResult result) => OnLobbiesQueried?.Invoke(result);
        public static void InvokeLobbyCreated(OperationResult result) => OnLobbyCreated?.Invoke(result);
        public static void InvokeLobbyJoined(OperationResult result) => OnLobbyJoined?.Invoke(result);
        public static void InvokeLobbyRejoined(OperationResult result) => OnLobbyRejoined?.Invoke(result);
        public static void InvokeLobbyLeft(OperationResult result) => OnLobbyLeft?.Invoke(result);
        public static void InvokeLobbyHostMigrated(string playerId) => OnLobbyHostMigrated?.Invoke(playerId);
        public static void InvokePlayerJoined(string playerId) => OnPlayerJoined?.Invoke(playerId);
        public static void InvokePlayerLeft(int playerIndex) => OnPlayerLeft?.Invoke(playerIndex);
        public static void InvokePlayerKicked(OperationResult result) => OnPlayerKicked?.Invoke(result);
        public static void InvokeLobbyDataUpdated(OperationResult result) => OnLobbyDataUpdated?.Invoke(result);
        public static void InvokePlayerDataUpdated(OperationResult result) => OnPlayerDataUpdated?.Invoke(result);
        public static void InvokePlayerConnecting(string playerId) => OnPlayerConnecting?.Invoke(playerId);
        public static void InvokePlayerConnected(string playerId) => OnPlayerConnected?.Invoke(playerId);
        public static void InvokePlayerDisconnected(string playerId) => OnPlayerDisconnected?.Invoke(playerId);
        #endregion
    }
}