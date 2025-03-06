// using System;
// using UnityEngine;
// using System.Linq;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Data;
// using System.Collections.Generic;
// using Assets.Scripts.Game.Events;
// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Framework.Managers;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Handles lobby chat functionality.
//     /// Manages player messages, system announcements, and chat formatting.
//     /// </summary>
//     public class LobbyChatManager : Singleton<LobbyChatManager>
//     {
//         #region Constants

//         public const string SYSTEM_SENDER_NAME = "SYSTEM";
//         public const string ERROR_SENDER_NAME = "ERROR";
//         public const string SYSTEM_MESSAGE_COLOR = "#FFAA00";
//         public const string ERROR_MESSAGE_COLOR = "#FF3333";
//         public const int MIN_MESSAGE_LENGTH = 1;
//         public const int MAX_MESSAGE_LENGTH = 300;
//         public const float MESSAGE_COOLDOWN_SECONDS = 0.5f;
//         public const int MAX_MESSAGE_HISTORY = 20;

//         #endregion

//         #region Private Fields

//         private readonly Dictionary<string, DateTime> lastMessageTime = new();
//         private readonly Queue<ChatMessageData> messageHistory = new(MAX_MESSAGE_HISTORY);

//         #endregion

//         #region Public Methods - Sending Messages

//         /// <summary>
//         /// Sends a player message to the lobby chat
//         /// </summary>
//         /// <param name="playerId">The ID of the player sending the message</param>
//         /// <param name="playerName">The name of the player sending the message</param>
//         /// <param name="message">The message to send</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public async Task<OperationResult> SendPlayerMessage(string playerId, string playerName, string message)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             if (!CanSendMessage(playerId))
//             {
//                 SendErrorMessage(playerId, "Please wait before sending another message.");
//                 return OperationResult.FailureResult("RateLimited", "Message rate limited.");
//             }

//             var validation = ValidateMessageLength(message);
//             if (!validation.Success)
//             {
//                 SendErrorMessage(playerId, validation.Message);
//                 return validation;
//             }

//             message = SanitizeMessage(message);
//             string timestampedMessage = FormatWithTimestamp(message);

//             var data = new Dictionary<string, string> { { "lastChatMessage", message } };
//             var result = await LobbyManager.Instance.UpdatePlayerData(playerId, data);

//             if (result.Success)
//             {
//                 lastMessageTime[playerId] = DateTime.Now;
//                 StoreMessage(playerName, timestampedMessage, ChatMessageData.ChatMessageType.Player);
//                 LobbyEvents.InvokeLobbyChat(playerName, timestampedMessage);
//             }

//             return result;
//         }

//         /// <summary>
//         /// Sends a system message to the lobby chat
//         /// </summary>
//         /// <param name="message">The message to send</param>
//         /// <param name="hostOnly">Whether only the host should see this message</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public OperationResult SendSystemMessage(string message, bool hostOnly = false)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             if (hostOnly && !GameLobbyManager.Instance.IsHost)
//                 return OperationResult.FailureResult("NotHost", "Not the lobby host.");

//             var validation = ValidateMessageLength(message);
//             if (!validation.Success)
//                 return validation;

//             string coloredMessage = $"<color={SYSTEM_MESSAGE_COLOR}>{message}</color>";
//             string timestampedMessage = FormatWithTimestamp(coloredMessage);

//             if (!hostOnly)
//                 StoreMessage(SYSTEM_SENDER_NAME, timestampedMessage, ChatMessageData.ChatMessageType.System);

//             LobbyEvents.InvokeLobbyChat(SYSTEM_SENDER_NAME, timestampedMessage);

//             return OperationResult.SuccessResult("SystemMessageSent", "System message sent.");
//         }

//         /// <summary>
//         /// Sends an error message visible only to a specific player
//         /// </summary>
//         /// <param name="playerId">The ID of the player to send the error to</param>
//         /// <param name="message">The error message</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public OperationResult SendErrorMessage(string playerId, string message)
//         {
//             if (!GameLobbyManager.Instance.IsInLobby)
//                 return OperationResult.FailureResult("NotInLobby", "Not in a lobby.");

//             string coloredMessage = $"<color={ERROR_MESSAGE_COLOR}>{message}</color>";
//             string timestampedMessage = FormatWithTimestamp(coloredMessage);

//             LobbyEvents.InvokePersonalMessage(playerId, ERROR_SENDER_NAME, timestampedMessage);

//             return OperationResult.SuccessResult("ErrorMessageSent", "Error message sent.");
//         }

//         /// <summary>
//         /// Announces match results in the lobby chat
//         /// </summary>
//         /// <param name="winningTeam">The ID of the winning team (0 or 1)</param>
//         /// <param name="teamAScore">The score of Team A</param>
//         /// <param name="teamBScore">The score of Team B</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public OperationResult AnnounceMatchResults(int winningTeam, int teamAScore, int teamBScore)
//         {
//             string teamAName = LobbyTeamManager.Instance != null ? LobbyTeamManager.Instance.GetTeamName(0) : "Team Red";
//             string teamBName = LobbyTeamManager.Instance != null ? LobbyTeamManager.Instance.GetTeamName(1) : "Team Blue";

//             string resultMessage;
//             if (winningTeam == 0)
//                 resultMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(LobbyTeamManager.Instance.GetTeamColor(0))}>{teamAName}</color> wins {teamAScore} - {teamBScore}!";
//             else
//                 resultMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(LobbyTeamManager.Instance.GetTeamColor(1))}>{teamBName}</color> wins {teamBScore} - {teamAScore}!";

//             return SendSystemMessage(resultMessage);
//         }

//         /// <summary>
//         /// Announces that teams have been balanced
//         /// </summary>
//         /// <param name="movedCount">The number of players moved to balance teams</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public OperationResult AnnounceTeamBalancing(int movedCount)
//         {
//             return SendSystemMessage($"Teams have been automatically balanced ({movedCount} players moved).");
//         }

//         #endregion

//         #region Chat History Methods

//         /// <summary>
//         /// Gets recent chat messages for a player who just joined
//         /// </summary>
//         /// <returns>Array of recent chat messages</returns>
//         public ChatMessageData[] GetRecentMessages()
//         {
//             return messageHistory.ToArray();
//         }

//         /// <summary>
//         /// Sends message history to a specific player
//         /// </summary>
//         /// <param name="playerId">The player ID to send history to</param>
//         public async void SendMessageHistoryToPlayer(string playerId)
//         {
//             if (messageHistory.Count == 0)
//                 return;

//             SendErrorMessage(playerId, $"Showing up to the last {messageHistory.Count} messages:");

//             await Task.Delay(100);

//             foreach (var message in messageHistory)
//                 LobbyEvents.InvokePersonalMessage(playerId, message.SenderName, message.FormattedMessage);
//         }

//         /// <summary>
//         /// Stores a message in the chat history
//         /// </summary>
//         private void StoreMessage(string senderName, string formattedMessage, ChatMessageData.ChatMessageType messageType)
//         {
//             var chatMessage = new ChatMessageData(senderName, formattedMessage, messageType);

//             messageHistory.Enqueue(chatMessage);
//             if (messageHistory.Count > MAX_MESSAGE_HISTORY)
//                 messageHistory.Dequeue();
//         }

//         /// <summary>
//         /// Clears chat history and message tracking
//         /// </summary>
//         public void ClearChatHistory()
//         {
//             messageHistory.Clear();
//             lastMessageTime.Clear();
//         }

//         #endregion

//         #region Helper Methods

//         /// <summary>
//         /// Checks if a player can send a message (rate limiting)
//         /// </summary>
//         /// <param name="playerId">The player ID to check</param>
//         /// <returns>True if the player can send a message, false otherwise</returns>
//         private bool CanSendMessage(string playerId)
//         {
//             if (!lastMessageTime.TryGetValue(playerId, out DateTime lastTime))
//                 return true;

//             TimeSpan elapsed = DateTime.Now - lastTime;
//             return elapsed.TotalSeconds >= MESSAGE_COOLDOWN_SECONDS;
//         }

//         /// <summary>
//         /// Formats a message with the current timestamp
//         /// </summary>
//         /// <param name="message">The message to format</param>
//         /// <returns>The formatted message</returns>
//         private string FormatWithTimestamp(string message)
//         {
//             return $"[{DateTime.Now:HH:mm:ss}] {message}";
//         }

//         /// <summary>
//         /// Validate a chat message on length
//         /// </summary>
//         /// <param name="message">The message to validate</param>
//         /// <returns>An OperationResult indicating success or failure.</returns>
//         public OperationResult ValidateMessageLength(string message)
//         {
//             if (string.IsNullOrEmpty(message) || message.Length < MIN_MESSAGE_LENGTH)
//                 return OperationResult.FailureResult("MessageTooShort", $"Message is too short (min {MIN_MESSAGE_LENGTH} characters).");

//             if (message.Length > MAX_MESSAGE_LENGTH)
//                 return OperationResult.FailureResult("MessageTooLong", $"Message is too long (max {MAX_MESSAGE_LENGTH} characters).");

//             return OperationResult.SuccessResult("MessageValid", "Message is valid.");
//         }

//         /// <summary>
//         /// Sanitizes message content to prevent HTML/rich text injection
//         /// </summary>
//         private string SanitizeMessage(string message)
//         {
//             return message.Replace("<", "&lt;").Replace(">", "&gt;");
//         }

//         #endregion
//     }
// }