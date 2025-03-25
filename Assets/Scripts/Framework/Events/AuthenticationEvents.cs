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
        public delegate Task<OperationResult> AuthenticatedHandler(string playerName);
        public static event AuthenticatedHandler OnAuthenticated;

        /// <summary>
        /// Triggered when the player is authenticated (for notification and UI updates).
        /// </summary>
        public delegate void AuthenticatedNotificationHandler(string playerName);
        public static event AuthenticatedNotificationHandler OnAuthenticatedNotification;
        #endregion

        #region Invocations
        public static async Task<OperationResult> InvokeOnAuthenticated(string playerName)
        {
            var result = await OnAuthenticated?.Invoke(playerName);
            OnAuthenticatedNotification?.Invoke(playerName);
            return result;
        }
        #endregion
    }
}