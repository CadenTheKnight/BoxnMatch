namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible statuses of a player in the lobby system.
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// The player is connected to the lobby.
        /// </summary>
        Connected = 0,

        /// <summary>
        /// The player is not connected to the lobby.
        /// </summary>
        Disconnected = 1,

        /// <summary>
        /// The player is connected to the lobby but not ready.
        /// </summary>
        NotReady = 2,

        /// <summary>
        /// The player is connected to the lobby and ready.
        /// </summary>
        Ready = 3,

        /// <summary>
        /// The player is in-game.
        /// </summary>
        InGame = 4,
    }
}