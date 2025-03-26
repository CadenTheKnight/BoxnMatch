using System.Threading.Tasks;
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
        /// Triggered when the player is authenticated (for core functionality that returns a result).
        /// </summary>
        public delegate void AuthenticatedHandler(string playerName);
        public static event AuthenticatedHandler OnAuthenticated;
        #endregion

        #region Invocations
        public static void InvokeOnAuthenticated(string playerName) => OnAuthenticated?.Invoke(playerName);
        #endregion
    }
}