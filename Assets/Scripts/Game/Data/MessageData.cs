namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents a chat message, including the sender name, formatted message, and message type.
    /// </summary>
    public class ChatMessageData
    {
        public string SenderName;
        public string FormattedMessage;
        public ChatMessageType MessageType;

        public enum ChatMessageType
        {
            Player,
            System,
            Error
        }

        public ChatMessageData(string senderName, string formattedMessage, ChatMessageType messageType = ChatMessageType.Player)
        {
            SenderName = senderName;
            FormattedMessage = formattedMessage;
            MessageType = messageType;
        }
    }
}