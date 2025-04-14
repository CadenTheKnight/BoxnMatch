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
        /// <param name="playerId">The id of the player whose status changed.</param>
        public delegate void PlayerStatusChangedHandler(string playerId);
        public static event PlayerStatusChangedHandler OnPlayerStatusChanged;

        /// <summary>
        /// Triggered when a player changes their team.
        /// </summary>
        /// <param name="playerId">The id of the player whose team changed.</param>
        public delegate void PlayerTeamChangedHandler(string playerId);
        public static event PlayerTeamChangedHandler OnPlayerTeamChanged;

        /// <summary>
        /// Triggered when game settings are updated by the host.
        /// </summary>
        public delegate void GameSettingsChangedHandler();
        public static event GameSettingsChangedHandler OnGameSettingsChanged;
        #endregion

        #region Invocations
        public static void InvokePlayerStatusChanged(string playerId) => OnPlayerStatusChanged?.Invoke(playerId);
        public static void InvokePlayerTeamChanged(string playerId) => OnPlayerTeamChanged?.Invoke(playerId);
        public static void InvokeGameSettingsChanged() => OnGameSettingsChanged?.Invoke();
        #endregion
    }
}