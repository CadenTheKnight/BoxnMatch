using System;
using Assets.Scripts.Game.Types;

namespace Assets.Scripts.Framework.Utilities
{
    /// <summary>
    /// Represents a chat message, including the time stamp, message type, sender name, and message.
    /// </summary>
    public class ChatMessage
    {
        public DateTime TimeSent { get; }
        public ChatMessageType MessageType { get; }
        public string SenderName { get; }
        public string Message { get; }


        /// <summary>
        /// Creates a new ChatMessage with the provided message type, sender name, and formatted message.
        /// </summary>
        public ChatMessage(ChatMessageType messageType, string senderName, string message)
        {
            TimeSent = DateTime.Now;
            MessageType = messageType;
            SenderName = senderName;
            Message = message;
        }

        /// <summary>
        /// Creates a new Player ChatMessage with the provided sender name and message.
        /// </summary>
        public static ChatMessage PlayerMessage(string senderName, string message)
            => new(ChatMessageType.Player, senderName, message);

        /// <summary>
        /// Creates a new System ChatMessage with the provided message.
        /// </summary>
        public static ChatMessage SystemMessage(string message)
            => new(ChatMessageType.System, "SYSTEM", message);

        /// <summary>
        /// Creates a new Error ChatMessage with the provided message.
        /// </summary>
        public static ChatMessage ErrorMessage(string message)
            => new(ChatMessageType.Error, "ERROR", message);
    }
}