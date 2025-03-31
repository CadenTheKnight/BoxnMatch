namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible statuses of a player in the lobby system.
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// The player is not connected to the lobby.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The player is connected to the lobby but not ready.
        /// </summary>
        NotReady,

        /// <summary>
        /// The player is connected to the lobby and ready.
        /// </summary>
        Ready,

        /// <summary>
        /// The player is in-game.
        /// </summary>
        InGame
    }
}