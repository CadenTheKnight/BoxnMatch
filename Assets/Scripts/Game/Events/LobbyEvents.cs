// using System.Collections.Generic;
// using Unity.Services.Lobbies.Models;
// using Assets.Scripts.Game.UI.Components.ListEntries;


// namespace Assets.Scripts.Game.Events
// {
//     /// <summary>
//     /// Contains game-specific events for lobby interactions.
//     /// </summary>
//     public static class LobbyEvents
//     {
//         #region Events

//         public delegate void LobbyUpdated();
//         public static event LobbyUpdated OnLobbyUpdated;

//         /// <summary>
//         /// Fired when a lobby is single clicked in the lobby list.
//         /// </summary>
//         /// <param name="lobby">The selected lobby.</param>
//         /// <param name="lobbyListEntry">The list entry of the selected lobby.</param>
//         public delegate void LobbySelectedHandler(Lobby lobby, LobbyListEntry lobbyListEntry);
//         public static event LobbySelectedHandler OnLobbySelected;

//         /// <summary>
//         /// Fired when a lobby is double clicked in the lobby list.
//         /// </summary>
//         /// <param name="lobby">The selected lobby.</param>
//         public delegate void LobbyDoubleClickedHandler(Lobby lobby);
//         public static event LobbyDoubleClickedHandler OnLobbyDoubleClicked;

//         /// <summary>
//         /// Fired when a player selects a character in the lobby.
//         /// </summary>
//         /// <param name="playerId">ID of the player who selected the character.</param>
//         /// <param name="characterId">ID of the selected character.</param>
//         public delegate void CharacterSelectedHandler(string playerId, string characterId);
//         public static event CharacterSelectedHandler OnCharacterSelected;

//         /// <summary>
//         ///  Fired when a player marks themselves as ready or unready.
//         /// </summary>
//         /// <param name="playerId">ID of the player who changed their ready status.</param>
//         /// <param name="isReady">True if the player is ready, false if not.</param>
//         public delegate void PlayerReadyChangedHandler(string playerId, bool isReady);
//         public static event PlayerReadyChangedHandler OnPlayerReadyChanged;

//         /// <summary>
//         /// Fired when all players in the lobby have marked themselves as ready.
//         /// </summary>
//         public delegate void AllPlayersReadyHandler();
//         public static event AllPlayersReadyHandler OnAllPlayersReady;

//         /// <summary>
//         /// Fired when at least one player in the lobby is not ready.
//         /// </summary>
//         public delegate void NotAllPlayersReadyHandler(int playersReady, int maxPlayerCount);
//         public static event NotAllPlayersReadyHandler OnNotAllPlayersReady;

//         /// <summary>
//         /// Fired when the selected arena/stage changes.
//         /// </summary>
//         /// <param name="arenaId">ID of the selected arena.</param>
//         public delegate void ArenaSelectedHandler(string arenaId);
//         public static event ArenaSelectedHandler OnArenaSelected;

//         /// <summary>
//         /// Fired when match settings are updated.
//         /// </summary>
//         /// <param name="settings">Dictionary of updated match settings.</param>
//         public delegate void MatchSettingsUpdatedHandler(Dictionary<string, string> settings);
//         public static event MatchSettingsUpdatedHandler OnMatchSettingsUpdated;

//         /// <summary>
//         /// Fired when the match has officially started.
//         /// </summary>
//         public delegate void MatchStartedHandler();
//         public static event MatchStartedHandler OnMatchStarted;

//         /// <summary>
//         /// Fired when a player sends a chat message in the lobby.
//         /// </summary>
//         /// <param name="playerId">ID of the player who sent the message.</param>
//         /// <param name="message">Content of the chat message.</param>
//         public delegate void LobbyChatHandler(string playerId, string message);
//         public static event LobbyChatHandler OnLobbyChat;


//         /// <summary>
//         /// Fired when a player is sent a personal message in the lobby.
//         /// </summary>
//         /// <param name="playerId">ID of the player receiving the message.</param>
//         /// <param name="senderName">Name of the player receiving the message.</param>
//         /// <param name="message">Content of the personal message.</param>
//         public delegate void PersonalMessageHandler(string playerId, string senderName, string message);
//         public static event PersonalMessageHandler OnPersonalMessage;

//         /// <summary>
//         /// Fired when color selection changes for a player.
//         /// </summary>
//         /// <param name="playerId">ID of the player who changed color.</param>
//         /// <param name="colorId">ID of the selected color.</param>
//         public delegate void PlayerColorSelectedHandler(string playerId, string colorId);
//         public static event PlayerColorSelectedHandler OnPlayerColorSelected;

//         /// <summary>
//         /// Fired when a player selects a team.
//         /// </summary>
//         /// <param name="playerId">ID of the player who selected a team.</param>
//         /// <param name="teamId">ID of the selected team.</param>
//         public delegate void TeamSelectedHandler(string playerId, int teamId);
//         public static event TeamSelectedHandler OnTeamSelected;

//         #endregion

//         #region Invocations
//         public static void InvokeLobbyUpdated() => OnLobbyUpdated?.Invoke();
//         public static void InvokeLobbySelected(Lobby lobby, LobbyListEntry lobbyListEntry) => OnLobbySelected?.Invoke(lobby, lobbyListEntry);
//         public static void InvokeLobbyDoubleClicked(Lobby lobby) => OnLobbyDoubleClicked?.Invoke(lobby);
//         public static void InvokeCharacterSelected(string playerId, string characterId) => OnCharacterSelected?.Invoke(playerId, characterId);
//         public static void InvokePlayerReadyChanged(string playerId, bool isReady) => OnPlayerReadyChanged?.Invoke(playerId, isReady);
//         public static void InvokeAllPlayersReady() => OnAllPlayersReady?.Invoke();
//         public static void InvokeNotAllPlayersReady(int playersReady, int maxPlayerCount) => OnNotAllPlayersReady?.Invoke(playersReady, maxPlayerCount);
//         public static void InvokeArenaSelected(string arenaId) => OnArenaSelected?.Invoke(arenaId);
//         public static void InvokeMatchSettingsUpdated(Dictionary<string, string> settings) => OnMatchSettingsUpdated?.Invoke(settings);
//         public static void InvokeMatchStarted() => OnMatchStarted?.Invoke();
//         public static void InvokeLobbyChat(string playerId, string message) => OnLobbyChat?.Invoke(playerId, message);
//         public static void InvokePersonalMessage(string playerId, string senderName, string message) => OnPersonalMessage?.Invoke(playerId, senderName, message);
//         public static void InvokePlayerColorSelected(string playerId, string colorId) => OnPlayerColorSelected?.Invoke(playerId, colorId);
//         public static void InvokeTeamSelected(string playerId, int teamId) => OnTeamSelected?.Invoke(playerId, teamId);

//         #endregion

//         #region Cleanup

//         /// <summary>
//         /// Clears all event subscriptions to prevent memory leaks.
//         /// </summary>
//         public static void ClearAllEventHandlers()
//         {
//             OnLobbyUpdated = null;
//             OnLobbySelected = null;
//             OnLobbyDoubleClicked = null;
//             OnCharacterSelected = null;
//             OnPlayerReadyChanged = null;
//             OnAllPlayersReady = null;
//             OnNotAllPlayersReady = null;
//             OnArenaSelected = null;
//             OnMatchSettingsUpdated = null;
//             OnMatchStarted = null;
//             OnLobbyChat = null;
//             OnPersonalMessage = null;
//             OnPlayerColorSelected = null;
//             OnTeamSelected = null;
//         }

//         #endregion
//     }
// }