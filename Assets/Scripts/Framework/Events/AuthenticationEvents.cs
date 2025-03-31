using System;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to the authentication process.
    /// </summary>
    public static class AuthenticationEvents
    {
        #region Events
        /// <summary>
        /// Triggered when the player is authenticated.
        /// </summary>
        /// <param name="result">The result of the authentication operation.</param>
        public delegate void AuthenticatedHandler(OperationResult result);
        public static event AuthenticatedHandler OnAuthenticated;

        /// <summary>
        /// Triggered when there is an error during the authentication process.
        /// </summary>
        /// <param name="result">The result of the authentication operation.</param>
        public delegate void AuthenticationErrorHandler(OperationResult result, Action retryAction);
        public static event AuthenticationErrorHandler OnAuthenticationError;

        /// <summary>
        /// Triggered when the player is rejoining a lobby.
        /// </summary>
        /// <param name="result">The result of the rejoin operation.</param>
        public delegate void LobbyRegionHandler(OperationResult result);
        public static event LobbyRegionHandler OnLobbyRejoined;

        /// <summary>
        /// Triggered when there is an error during the rejoin process.
        /// </summary>
        /// <param name="result">The result of the rejoin operation.</param>
        public delegate void LobbyRejoinErrorHandler(OperationResult result);
        public static event LobbyRejoinErrorHandler OnLobbyRejoinError;
        #endregion

        #region Invocations
        public static void InvokeAuthenticated(OperationResult result) => OnAuthenticated?.Invoke(result);
        public static void InvokeAuthenticationError(OperationResult result, Action retryAction) => OnAuthenticationError?.Invoke(result, retryAction);
        public static void InvokeLobbyRejoined(OperationResult result) => OnLobbyRejoined?.Invoke(result);
        public static void InvokeLobbyRejoinError(OperationResult result) => OnLobbyRejoinError?.Invoke(result);
        #endregion
    }
}