// using Assets.Scripts.Game.Types;
// using System.Collections.Generic;
// using Unity.Services.Lobbies.Models;

// namespace Assets.Scripts.Game.Events
// {
//     public static class GameEvents
//     {
//         #region Events
//         /// <summary>
//         /// Triggered when the game is ready to start.
//         /// </summary>
//         /// <param name="success">True if the game started successfully, false otherwise.</param>
//         /// <param name="relayJoinCode">The relay join code if applicable.</param>
//         public delegate void GameStartedHandler(bool success, string relayJoinCode);
//         public static event GameStartedHandler OnGameStarted;

//         /// <summary>
//         /// Triggered when the game is ended.
//         /// </summary>
//         public delegate void GameEndedHandler();
//         public static event GameEndedHandler OnGameEnded;
//         #endregion

//         #region Invocations
//         public static void InvokeGameStarted(bool success, string relayJoinCode) => OnGameStarted?.Invoke(success, relayJoinCode);
//         public static void InvokeGameEnded() => OnGameEnded?.Invoke();
//         #endregion
//     }
// }