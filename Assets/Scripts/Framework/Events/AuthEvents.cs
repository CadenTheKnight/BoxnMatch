namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to authentication.
    /// </summary>
    public static class AuthEvents
    {
        public delegate void AuthenticatedHandler();
        public static event AuthenticatedHandler OnAuthenticated;

        /// <summary>
        /// Invokes the OnAuthenticated event.
        /// </summary>
        public static void InvokeAuthenticated() => OnAuthenticated?.Invoke();

        public static void ClearAllEvents()
        {
            OnAuthenticated = null;
        }
    }
}