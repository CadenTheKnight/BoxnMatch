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
        /// Triggered when a new lobby is successfully created.
        /// </summary>
        /// <param name="result"> The result of the lobby creation operation.</param>
        public delegate void LobbyCreatedHandler(OperationResult result);
        public static event LobbyCreatedHandler OnLobbyCreated;

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
        /// <param name="result">The result of the player join operation.</param>
        public delegate void PlayerJoinedHandler(OperationResult result);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby.
        /// </summary>
        /// <param name="result">The result of the player leave operation.</param>
        public delegate void PlayerLeftHandler(OperationResult result);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="result">The result of the player kick operation.</param>
        public delegate void PlayerKickedHandler(OperationResult result);
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
        /// Triggered when the lobby is refreshed by coroutine.
        /// </summary>
        public delegate void LobbyRefreshedHandler();
        public static event LobbyRefreshedHandler OnLobbyRefreshed;

        /// <summary>
        /// Triggered when the player data is updated.
        /// </summary>
        /// <param name="result">The result of the player data update operation.</param>
        public delegate void PlayerDataUpdatedHandler(OperationResult result);
        public static event PlayerDataUpdatedHandler OnPlayerDataUpdated;

        /// <summary>
        /// Triggered when the lobby data is updated.
        /// </summary>
        /// <param name="result">The result of the lobby data update operation.</param>
        public delegate void LobbyDataUpdatedHandler(OperationResult result);
        public static event LobbyDataUpdatedHandler OnLobbyDataUpdated;
        #endregion

        #region Invocations
        public static void InvokeLobbyCreated(OperationResult result) => OnLobbyCreated?.Invoke(result);
        public static void InvokeLobbyJoined(OperationResult result) => OnLobbyJoined?.Invoke(result);
        public static void InvokeLobbyLeft(OperationResult result) => OnLobbyLeft?.Invoke(result);
        public static void InvokeLobbyKicked(OperationResult result) => OnLobbyKicked?.Invoke(result);
        public static void InvokePlayerJoined(OperationResult result) => OnPlayerJoined?.Invoke(result);
        public static void InvokePlayerLeft(OperationResult result) => OnPlayerLeft?.Invoke(result);
        public static void InvokePlayerKicked(OperationResult result) => OnPlayerKicked?.Invoke(result);
        public static void InvokeLobbyQueryResponse(OperationResult result) => OnLobbyQueryResponse?.Invoke(result);
        public static void InvokeLobbyError(OperationResult result) => OnLobbyError?.Invoke(result);
        public static void InvokeLobbyRefreshed() => OnLobbyRefreshed?.Invoke();
        public static void InvokePlayerDataUpdated(OperationResult result) => OnPlayerDataUpdated?.Invoke(result);
        public static void InvokeLobbyDataUpdated(OperationResult result) => OnLobbyDataUpdated?.Invoke(result);
        #endregion
    }
}