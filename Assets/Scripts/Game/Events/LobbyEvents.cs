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
        public delegate void LobbyNotReadyHandler(int playersReady);
        public static event LobbyNotReadyHandler OnLobbyNotReady;

        /// <summary>
        /// Triggered when the lobby host presses the start button.
        /// </summary>
        /// <param name="mapName">The name of the map to be loaded.</param>
        public delegate void LobbyGameStartingHandler(string mapName);
        public static event LobbyGameStartingHandler OnGameStarting;
        #endregion

        #region Invocations
        public static void InvokeLobbyReady() => OnLobbyReady?.Invoke();
        public static void InvokeLobbyNotReady(int playersReady) => OnLobbyNotReady?.Invoke(playersReady);
        public static void InvokeGameStarting(string mapName) => OnGameStarting?.Invoke(mapName);
        #endregion
    }
}