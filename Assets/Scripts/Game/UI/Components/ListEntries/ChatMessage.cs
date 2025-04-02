using TMPro;
using System;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    /// <summary>
    /// Represents a chat message in the lobby and game UI.
    /// </summary>
    public class ChatMessage : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI senderText;
        [SerializeField] private TextMeshProUGUI messageText;

        // public ChatMessage(string sender, string message)
        // {
        //     timeText.text = DateTime.Now.ToString("[HH:mm]");
        //     senderText.text = sender;
        //     messageText.text = message;
        // }

        public void SetMessage(string sender, string message)
        {
            timeText.text = DateTime.Now.ToString("[HH:mm]");
            senderText.text = sender;
            // senderText.color = 
            messageText.text = message;
        }

        public void SetMessage(ChatMessage message)
        {
            timeText.text = DateTime.Now.ToString("[HH:mm]");
            senderText.text = message.senderText.text;
            // senderText.color = 
            messageText.text = message.messageText.text;
        }
    }
}
