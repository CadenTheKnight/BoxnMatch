using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Events
{
    /// <summary>
    /// Contains game-specific events for lobby interactions.
    /// </summary>
    public static class GameLobbyEvents
    {
        #region Events
        /// <summary>
        /// Triggered when a player changes their ready status.
        /// </summary>
        /// <param name="playersReady">The number of players ready.</param>
        /// <param name="maxPlayers">The maximum number of players in the lobby.</param>
        public delegate void LobbyReadyStatusHandler(int playersReady, int maxPlayers);
        public static event LobbyReadyStatusHandler OnLobbyReadyStatusUpdated;

        /// <summary>
        /// Triggered when the lobby host presses the start button.
        /// </summary>
        /// <param name="mapName">The name of the map to be loaded.</param>
        public delegate void LobbyGameStartingHandler(string mapName);
        public static event LobbyGameStartingHandler OnGameStarting;
        #endregion

        #region Invocations
        public static void InvokeLobbyReadyStatus(int playersReady, int maxPlayers) => OnLobbyReadyStatusUpdated?.Invoke(playersReady, maxPlayers);
        public static void InvokeGameStarting(string mapName) => OnGameStarting?.Invoke(mapName);
        #endregion
    }
}