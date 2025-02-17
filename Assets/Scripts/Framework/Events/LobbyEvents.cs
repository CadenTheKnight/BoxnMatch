using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Framework.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated(Lobby lobby);
        public static event LobbyUpdated OnLobbyUpdated;
        public delegate void PlayerJoined(string playerId);
        public static event PlayerJoined OnPlayerJoined;
        public delegate void PlayerLeft(string playerId);
        public static event PlayerLeft OnPlayerLeft;
        public delegate void LobbyClosed();
        public static event LobbyClosed OnLobbyClosed;

        public static void InvokeLobbyUpdated(Lobby lobby)
        {
            OnLobbyUpdated?.Invoke(lobby);
        }

        public static void InvokePlayerJoined(string playerId)
        {
            OnPlayerJoined?.Invoke(playerId);
        }

        public static void InvokePlayerLeft(string playerId)
        {
            OnPlayerLeft?.Invoke(playerId);
        }

        public static void InvokeLobbyClosed()
        {
            OnLobbyClosed?.Invoke();
        }
    }
}