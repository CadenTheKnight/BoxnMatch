namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible statuses of a lobby in the lobby system.
    /// </summary>
    public enum LobbyStatus
    {
        /// <summary>
        /// The lobby is currently in the lobby phase, where players can join and prepare for the game.
        /// </summary>
        InLobby,

        /// <summary>
        /// The lobby is currently in the game phase, where the actual gameplay is taking place.
        /// </summary>
        InGame
    }
}