using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to authentication and user sign-in processes.
    /// </summary>
    public static class AuthEvents
    {
        #region Events
        /// <summary>
        /// Triggered when there is an error during initialization.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public delegate void InitializationErrorHandler(OperationResult result);
        public static InitializationErrorHandler OnInitializationError;
        #endregion

        #region Invocations
        public static void InvokeInitializationError(OperationResult result) => OnInitializationError?.Invoke(result);
        #endregion
    }
}