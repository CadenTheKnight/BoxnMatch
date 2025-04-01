namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible states of a game.
    /// </summary>
    public enum GameState
    {
        RoundStarting = 0,
        RoundInProgress = 1,
        RoundEnding = 2,
        GameEnding = 3,
        ReturnToLobby = 4,
    }
}