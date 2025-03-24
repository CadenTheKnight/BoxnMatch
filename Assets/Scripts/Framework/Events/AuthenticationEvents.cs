using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

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
        /// <param name="playerName">The name of the authenticated player.</param>
        public delegate void AuthenticatedHandler(string playerName);
        public static event AuthenticatedHandler OnAuthenticated;
        #endregion

        #region Invocations
        public static void InvokeOnAuthenticated(string playerName) => OnAuthenticated?.Invoke(playerName);
        #endregion
    }
}