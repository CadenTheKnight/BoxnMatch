using TMPro;
using Steamworks;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.ListEntries;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Handles the chat panel in the lobby.
    /// </summary>
    public class ChatManager : NetworkSingleton<ChatManager>
    {
        // [Header("Debug")]
        // [SerializeField] private bool showDebugLogs = false;

        // [Header("UI Components")]
        // [SerializeField] private ChatMessage chatListEntry;
        // [SerializeField] private Transform chatListContainer;
        // [SerializeField] private TMP_InputField chatInputField;
        // [SerializeField] private Button sendButton;

        // public override void OnNetworkSpawn()
        // {
        //     base.OnNetworkSpawn();
        //     Debug.Log("ChatManager spawned on network!");
        // }

        // void Start()
        // {
        //     foreach (ChatMessage message in ChatHistory.GetRecentMessages(20))
        //     {
        //         if (showDebugLogs) Debug.Log("ChatManager: Adding message to chat history: " + message);

        //         ChatMessage chatMessage = Instantiate(chatListEntry, chatListContainer.transform);
        //         chatMessage.SetMessage(message);
        //     }

        //     sendButton.onClick.AddListener(SendPlayerMessage);
        // }

        // public override void OnDestroy()
        // {
        //     base.OnDestroy();
        //     sendButton.onClick.RemoveListener(SendPlayerMessage);
        // }

        // void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Return))
        //         Debug.Log("Return pressed - Input field has text: " + !string.IsNullOrEmpty(chatInputField.text.Trim()));

        //     // if (chatInputField.isFocused && !string.IsNullOrEmpty(chatInputField.text.Trim()) && Input.GetKeyDown(KeyCode.Return))
        //     if (!string.IsNullOrEmpty(chatInputField.text.Trim()) && Input.GetKeyDown(KeyCode.Return))
        //     {
        //         if (showDebugLogs) Debug.Log("ChatManager: Enter key pressed, sending message: " + chatInputField.text.Trim());

        //         SendPlayerMessage();
        //     }
        // }

        // /// <summary>
        // /// Handles the send button click event.
        // /// Sends the chat message to all players in the lobby.
        // /// /// </summary>
        // public void SendPlayerMessage()
        // {
        //     if (showDebugLogs) Debug.Log("ChatManager: Sending message  " + SteamFriends.GetPersonaName() + ": " + chatInputField.text.Trim());

        //     ChatMessage message = new();
        //     message.SetMessage(SteamFriends.GetPersonaName(), chatInputField.text.Trim());

        //     SendChatMessageServerRpc(message);
        //     chatInputField.text = string.Empty;
        // }

        // /// <summary>
        // /// Sends a chat message to all players in the lobby.
        // /// This is called from the client when the player sends a message.
        // /// </summary>
        // /// <param name="message">The message to send.</param>
        // [ServerRpc(RequireOwnership = false)]
        // void SendChatMessageServerRpc(ChatMessage message)
        // {
        //     if (showDebugLogs) Debug.Log("ChatManager: Server received message");

        //     ReceiveChatMessageClientRpc(message);
        // }

        // /// <summary>
        // /// Receives a chat message from the server and updates the chat history.
        // /// This is called on all clients when a message is sent.
        // /// </summary>
        // /// <param name="message">The message to receive.</param>
        // [ClientRpc]
        // void ReceiveChatMessageClientRpc(ChatMessage message)
        // {
        //     if (showDebugLogs) Debug.Log("ChatManager: Client received message");

        //     ChatHistory.AddMessage(message);
        //     ChatMessage chatMessage = Instantiate(chatListEntry, chatListContainer.transform);
        //     chatMessage.SetMessage(message);
        // }
    }
}