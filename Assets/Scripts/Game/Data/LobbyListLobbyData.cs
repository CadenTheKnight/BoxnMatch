using System.Collections.Generic;

namespace Assets.Scripts.Game.Data
{
    public class LobbyListLobbyData
    {
        private string lobbyName;
        private int playerCount;
        private int maxPlayers;
        private string hostName;

        public string LobbyName => lobbyName;
        public int PlayerCount => playerCount;
        public int MaxPlayers => maxPlayers;
        public string HostName => hostName;

        public LobbyListLobbyData(string lobbyName, int playerCount, int maxPlayers, string hostName)
        {
            this.lobbyName = lobbyName;
            this.playerCount = playerCount;
            this.maxPlayers = maxPlayers;
            this.hostName = hostName;
        }

        public LobbyListLobbyData(Dictionary<string, LobbyListLobbyData> lobbyListLobbyData)
        {
            UpdateState(lobbyListLobbyData);
        }

        public void UpdateState(Dictionary<string, LobbyListLobbyData> lobbyListLobbyData)
        {
            if (lobbyListLobbyData.ContainsKey("lobbyName"))
            {
                lobbyName = lobbyListLobbyData["lobbyName"].LobbyName;
            }
            if (lobbyListLobbyData.ContainsKey("playerCount"))
            {
                playerCount = lobbyListLobbyData["playerCount"].PlayerCount;
            }
            if (lobbyListLobbyData.ContainsKey("maxPlayers"))
            {
                maxPlayers = lobbyListLobbyData["maxPlayers"].MaxPlayers;
            }
            if (lobbyListLobbyData.ContainsKey("hostName"))
            {
                hostName = lobbyListLobbyData["hostName"].HostName;
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                { "lobbyName", lobbyName },
                { "playerCount", playerCount.ToString() },
                { "maxPlayers", maxPlayers.ToString() },
                { "hostName", hostName }
            };
        }
    }
}
