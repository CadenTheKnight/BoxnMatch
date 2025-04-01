namespace Assets.Scripts.Game.Events
{
    /// <summary>
    /// Contains game-specific events for lobby interactions.
    /// </summary>
    public static class LobbyEvents
    {
        #region Events
        /// <summary>
        /// Triggered when all players in the lobby are ready.
        /// </summary>
        public delegate void LobbyReadyHandler();
        public static event LobbyReadyHandler OnLobbyReady;

        /// <summary>
        /// Triggered when at least one player in the lobby is not ready.
        /// </summary>
        /// <param name="playersReady">The number of players that are ready.</param>
        /// <param name="maxPlayerCount">The maximum number of players in the lobby.</param>
        public delegate void LobbyNotReadyHandler(int playersReady, int maxPlayerCount);
        public static event LobbyNotReadyHandler OnLobbyNotReady;

        // /// <summary>
        // /// Triggered when the match has started.
        // /// </summary>
        // public delegate void MatchStartedHandler();
        // public static event MatchStartedHandler OnMatchStarted;
        #endregion

        #region Invocations
        public static void InvokeLobbyReady() => OnLobbyReady?.Invoke();
        public static void InvokeLobbyNotReady(int playersReady, int maxPlayerCount) => OnLobbyNotReady?.Invoke(playersReady, maxPlayerCount);
        // public static void InvokeMatchStarted() => OnMatchStarted?.Invoke();
        #endregion
    }
}