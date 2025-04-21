// using Assets.Scripts.Game.Types;
// using System.Collections.Generic;
// using Unity.Services.Lobbies.Models;

// namespace Assets.Scripts.Game.Events
// {
//     /// <summary>
//     /// Contains game-specific events for lobby interactions.
//     /// </summary>
//     public static class GameLobbyEvents
//     {
//         #region Events
//         /// <summary>
//         /// Triggered when the game settings change.
//         /// </summary>
//         /// <param name="success">Indicates if the changes were successful.</param>
//         /// <param name="gameSettings">A dictionary of changes made to the game settings.</param>
//         public delegate void GameSettingsChangedHandler(bool success, Dictionary<string, DataObject> gameSettings);
//         public static event GameSettingsChangedHandler OnGameSettingsChanged;

//         /// <summary>
//         /// Triggered when a player changes their team.
//         /// </summary>
//         /// <param name="success">Indicates if the team change was successful.</param>
//         /// <param name="playerId">The id of the player whose team changed.</param>
//         /// <param name="team">The new team of the player.</param>
//         public delegate void PlayerTeamChangedHandler(bool success, string playerId, Team team);
//         public static event PlayerTeamChangedHandler OnPlayerTeamChanged;

//         /// <summary>
//         /// Triggered when a player changes their ready status.
//         /// </summary>
//         /// <param name="success">Indicates if the ready status change was successful.</param>
//         /// <param name="playerId">The id of the player whose status changed.</param>
//         /// <param name="readyStatus">The new ready status of the player.</param>
//         public delegate void PlayerReadyStatusChangedHandler(bool success, string playerId, ReadyStatus readyStatus);
//         public static event PlayerReadyStatusChangedHandler OnPlayerReadyStatusChanged;

//         /// <summary>
//         /// Triggered when a player's connection status changes.
//         /// </summary>
//         /// <param name="success">Indicates if the connection status change was successful.</param>
//         /// <param name="playerId">The id of the player who is connecting.</param>
//         /// <param name="connectionStatus">The connection status of the player.</param>
//         public delegate void PlayerConnectionStatusChangedHandler(bool success, string playerId, ConnectionStatus connectionStatus);
//         public static event PlayerConnectionStatusChangedHandler OnPlayerConnectionStatusChanged;
//         #endregion

//         #region Invocations
//         public static void InvokeGameSettingsChanged(bool success, Dictionary<string, DataObject> gameSettings) => OnGameSettingsChanged?.Invoke(success, gameSettings);
//         public static void InvokePlayerTeamChanged(bool success, string playerId, Team team) => OnPlayerTeamChanged?.Invoke(success, playerId, team);
//         public static void InvokePlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus) => OnPlayerReadyStatusChanged?.Invoke(success, playerId, readyStatus);
//         public static void InvokePlayerConnectionStatusChanged(bool success, string playerId, ConnectionStatus connectionStatus) => OnPlayerConnectionStatusChanged?.Invoke(success, playerId, connectionStatus);
//         #endregion
//     }
// }