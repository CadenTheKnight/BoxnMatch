namespace Assets.Scripts.Game.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated();
        public static event LobbyUpdated OnLobbyUpdated;
        public delegate void PlayerReady(string playerId);
        public static event PlayerReady OnPlayerReady;
        public delegate void PlayerNotReady(string playerId);
        public static event PlayerNotReady OnPlayerNotReady;
        public delegate void LobbyReady();
        public static event LobbyReady OnLobbyReady;
        public delegate void LobbyNotReady();
        public static event LobbyNotReady OnLobbyNotReady;

        public static void InvokeLobbyUpdated()
        {
            OnLobbyUpdated?.Invoke();
        }

        public static void InvokePlayerReady(string playerId)
        {
            OnPlayerReady?.Invoke(playerId);
        }

        public static void InvokePlayerNotReady(string playerId)
        {
            OnPlayerNotReady?.Invoke(playerId);
        }

        public static void InvokeLobbyReady()
        {
            OnLobbyReady?.Invoke();
        }

        public static void InvokeLobbyNotReady()
        {
            OnLobbyNotReady?.Invoke();
        }
    }
}
