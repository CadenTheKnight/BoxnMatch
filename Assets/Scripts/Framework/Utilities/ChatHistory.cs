using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Framework.Utilities
{
    public static class ChatHistory
    {
        private static readonly List<ChatMessage> messages = new(100);

        public static void AddMessage(ChatMessage message)
        {
            messages.Add(message);
            if (messages.Count > 100) messages.RemoveAt(0);
        }

        public static List<ChatMessage> GetRecentMessages(int count)
        {
            return messages.Skip(Math.Max(0, messages.Count - count)).ToList();
        }
    }
}