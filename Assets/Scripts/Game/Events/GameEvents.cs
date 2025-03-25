using Assets.Scripts.Game.Enums;

namespace Assets.Scripts.Game.Events
{
    /// <summary>
    /// Contains game-specific events for game state changes.
    /// </summary>
    public static class GameEvents
    {
        #region Events
        public delegate void GameStateChangedHandler(GameState previousState, GameState newState);
        public static event GameStateChangedHandler OnGameStateChanged;

        public delegate void RoundChangedHandler(int previousRound, int newRound);
        public static event RoundChangedHandler OnRoundChanged;

        public delegate void ScoreChangedHandler(int leftTeamScore, int rightTeamScore);
        public static event ScoreChangedHandler OnScoreChanged;

        public delegate void CountdownUpdatedHandler(float remainingTime);
        public static event CountdownUpdatedHandler OnCountdownUpdated;
        #endregion

        #region Invocations
        public static void InvokeGameStateChanged(GameState previousState, GameState newState) => OnGameStateChanged?.Invoke(previousState, newState);
        public static void InvokeRoundChanged(int previousRound, int newRound) => OnRoundChanged?.Invoke(previousRound, newRound);
        public static void InvokeScoreChanged(int leftTeamScore, int rightTeamScore) => OnScoreChanged?.Invoke(leftTeamScore, rightTeamScore);
        public static void InvokeCountdownUpdated(float remainingTime) => OnCountdownUpdated?.Invoke(remainingTime);
        #endregion
    }
}