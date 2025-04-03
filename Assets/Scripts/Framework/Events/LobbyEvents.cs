using Assets.Scripts.Framework.Utilities;
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
        /// <param name="result"> The result of the lobby creation operation.</param>
        public delegate void LobbyCreatedHandler(OperationResult result);
        public static event LobbyCreatedHandler OnLobbyCreated;


        public delegate void LobbyChangedHandler();
        public static event LobbyChangedHandler OnLobbyChanged;




        /// <summary>
        /// Triggered when successfully joining an existing lobby.
        /// </summary>
        /// <param name="result">The result of the lobby join operation.</param>
        public delegate void LobbyJoinedHandler(OperationResult result);
        public static event LobbyJoinedHandler OnLobbyJoined;

        /// <summary>
        /// Triggered when successfully leaving a lobby.
        /// </summary>
        /// <param name="result">The result of the lobby leave operation.</param>
        public delegate void LobbyLeftHandler(OperationResult result);
        public static event LobbyLeftHandler OnLobbyLeft;

        /// <summary>
        /// Triggered when kicked from a lobby.
        /// </summary>
        /// <param name="result">The result of the lobby kick operation.</param>
        public delegate void LobbyKickedHandler(OperationResult result);
        public static event LobbyKickedHandler OnLobbyKicked;

        /// <summary>
        /// Triggered when a player joins the lobby.
        /// </summary>
        /// <param name="player">The player who joined the lobby.</param>
        public delegate void PlayerJoinedHandler(Player player);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby.
        /// </summary>
        /// <param name="player">The player who left the lobby.</param>
        public delegate void PlayerLeftHandler(Player player);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="player">The player who was kicked from the lobby.</param>
        public delegate void PlayerKickedHandler(Player player);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when a lobby query is successfully completed.
        /// </summary>
        /// <param name="result">The result of the lobby query operation.</param>
        public delegate void InvokeLobbyQueryResponseHandler(OperationResult result);
        public static event InvokeLobbyQueryResponseHandler OnLobbyQueryResponse;

        /// <summary>
        /// Triggered when there is an error during a lobby operation.
        /// </summary>
        /// <param name="result">The result of the lobby operation.</param>
        public delegate void LobbyErrorHandler(OperationResult result);
        public static event LobbyErrorHandler OnLobbyError;

        /// <summary>
        /// Triggered when the lobby data is updated.
        /// </summary>
        /// <param name="result">The result of the lobby data update operation.</param>
        public delegate void LobbyDataUpdatedHandler(OperationResult result);
        public static event LobbyDataUpdatedHandler OnLobbyDataUpdated;

        /// <summary>
        /// Triggered when the lobby data is changed.
        /// </summary>
        /// <param name="key">The key of the changed data.</param>
        /// <param name="value">The new value of the changed data.</param>
        public delegate void LobbyDataChangedHandler(string key, string value);
        public static event LobbyDataChangedHandler OnLobbyDataChanged;

        /// <summary>
        /// Triggered when the lobby data is added.
        /// </summary>
        /// <param name="key">The key of the added data.</param>
        /// <param name="value">The value of the added data.</param>
        public delegate void LobbyDataAddedHandler(string key, string value);
        public static event LobbyDataAddedHandler OnLobbyDataAdded;

        /// <summary>
        /// Triggered when the lobby data is removed.
        /// </summary>
        /// <param name="key">The key of the removed data.</param>
        /// <param name="value">The value of the removed data.</param>
        public delegate void LobbyDataRemovedHandler(string key, string value);
        public static event LobbyDataRemovedHandler OnLobbyDataRemoved;

        /// <summary>
        /// Triggered when the player data is updated.
        /// </summary>
        /// <param name="result">The result of the player data update operation.</param>
        public delegate void PlayerDataUpdatedHandler(OperationResult result);
        public static event PlayerDataUpdatedHandler OnPlayerDataUpdated;

        /// <summary>
        /// Triggered when the player data is changed.
        /// </summary>
        /// <param name="player">The player whose data has changed.</param>
        /// <param name="key">The key of the changed data.</param>
        /// <param name="value">The new value of the changed data.</param>
        public delegate void PlayerDataChangedHandler(Player player, string key, string value);
        public static event PlayerDataChangedHandler OnPlayerDataChanged;

        /// <summary>
        /// Triggered when the player data is added.
        /// </summary>
        /// <param name="player">The player whose data has been added.</param>
        /// <param name="key">The key of the added data.</param>
        /// <param name="value">The value of the added data.</param>
        public delegate void PlayerDataAddedHandler(Player player, string key, string value);
        public static event PlayerDataAddedHandler OnPlayerDataAdded;

        /// <summary>
        /// Triggered when the player data is removed.
        /// </summary>
        /// <param name="player">The player whose data has been removed.</param>
        /// <param name="key">The key of the removed data.</param>
        /// <param name="value">The value of the removed data.</param>
        public delegate void PlayerDataRemovedHandler(Player player, string key, string value);
        public static event PlayerDataRemovedHandler OnPlayerDataRemoved;
        #endregion

        #region Invocations
        public static void InvokeLobbyCreated(OperationResult result) => OnLobbyCreated?.Invoke(result);

        public static void InvokeLobbyChanged() => OnLobbyChanged?.Invoke();

        public static void InvokeLobbyJoined(OperationResult result) => OnLobbyJoined?.Invoke(result);
        public static void InvokeLobbyLeft(OperationResult result) => OnLobbyLeft?.Invoke(result);
        public static void InvokeLobbyKicked(OperationResult result) => OnLobbyKicked?.Invoke(result);
        public static void InvokePlayerJoined(Player player) => OnPlayerJoined?.Invoke(player);
        public static void InvokePlayerLeft(Player player) => OnPlayerLeft?.Invoke(player);
        public static void InvokePlayerKicked(Player player) => OnPlayerKicked?.Invoke(player);
        public static void InvokeLobbyQueryResponse(OperationResult result) => OnLobbyQueryResponse?.Invoke(result);
        public static void InvokeLobbyError(OperationResult result) => OnLobbyError?.Invoke(result);
        public static void InvokeLobbyDataUpdated(OperationResult result) => OnLobbyDataUpdated?.Invoke(result);
        public static void InvokeLobbyDataChanged(string key, string value) => OnLobbyDataChanged?.Invoke(key, value);
        public static void InvokeLobbyDataAdded(string key, string value) => OnLobbyDataAdded?.Invoke(key, value);
        public static void InvokeLobbyDataRemoved(string key, string value) => OnLobbyDataRemoved?.Invoke(key, value);
        public static void InvokePlayerDataUpdated(OperationResult result) => OnPlayerDataUpdated?.Invoke(result);
        public static void InvokePlayerDataChanged(Player player, string key, string value) => OnPlayerDataChanged?.Invoke(player, key, value);
        public static void InvokePlayerDataAdded(Player player, string key, string value) => OnPlayerDataAdded?.Invoke(player, key, value);
        public static void InvokePlayerDataRemoved(Player player, string key, string value) => OnPlayerDataRemoved?.Invoke(player, key, value);
        #endregion
    }
}