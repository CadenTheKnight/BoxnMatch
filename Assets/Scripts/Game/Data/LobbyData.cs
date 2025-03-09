using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.XR;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a lobby.
    /// </summary>
    public class LobbyData
    {
        private string lobbyName;
        private int maxPlayers;
        private bool isPrivate;
        private int mapIndex;
        public string gameMode;
        public string relayJoinCode;
        private string mapSceneName;

        public int MapIndex
        {
            get => mapIndex;
            set => mapIndex = value;
        }

        public string RelayJoinCode
        {
            get => relayJoinCode;
            set => relayJoinCode = value;
        }

        public string MapSceneName
        {
            get => mapSceneName;
            set => mapSceneName = value;
        }

        public void Initialize(string lobbyName, int maxPlayers, bool isPrivate)
        {
            this.lobbyName = lobbyName;
            this.maxPlayers = maxPlayers;
            this.isPrivate = isPrivate;
            mapIndex = 0;
            gameMode = "Standard";
        }

        public void Initialize(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.ContainsKey("LobbyName"))
                lobbyName = lobbyData["LobbyName"].Value;
            if (lobbyData.ContainsKey("MaxPlayers"))
                maxPlayers = int.Parse(lobbyData["MaxPlayers"].Value);
            if (lobbyData.ContainsKey("IsPrivate"))
                isPrivate = lobbyData["IsPrivate"].Value == "true";
            if (lobbyData.ContainsKey("MapIndex"))
                mapIndex = int.Parse(lobbyData["MapIndex"].Value);
            if (lobbyData.ContainsKey("GameMode"))
                gameMode = lobbyData["GameMode"].Value;
            if (lobbyData.ContainsKey("RelayJoinCode"))
                relayJoinCode = lobbyData["RelayJoinCode"].Value;
            if (lobbyData.ContainsKey("MapSceneName"))
                mapSceneName = lobbyData["MapSceneName"].Value;
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "LobbyName", lobbyName },
                { "MaxPlayers", maxPlayers.ToString() },
                { "IsPrivate", isPrivate.ToString().ToLower() },
                { "MapIndex", mapIndex.ToString() },
                { "GameMode", gameMode },
                { "RelayJoinCode", relayJoinCode },
                { "MapSceneName", mapSceneName }
            };
        }
    }
}