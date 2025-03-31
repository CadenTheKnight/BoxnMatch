namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible game modes for a game.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Team based game mode, where players are divided into teams. (2 for now)
        /// /// </summary>
        Teams,

        /// <summary>
        /// Free for all game mode, where players compete against each other without teams.
        /// </summary>
        FreeForAll,

        /// <summary>
        /// AI / capture the flag / others to be implemented later.
        /// /// </summary>
        Other,
    }
}