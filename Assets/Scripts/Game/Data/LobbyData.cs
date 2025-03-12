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
        private int roundCount;
        private int mapIndex;
        public string gameMode;
        public string relayJoinCode;
        public bool inGame;
        public bool gameStarted;

        public int RoundCount
        {
            get => roundCount;
            set => roundCount = value;
        }

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

        public bool InGame
        {
            get => inGame;
            set => inGame = value;
        }

        public bool GameStarted
        {
            get => gameStarted;
            set => gameStarted = value;
        }

        public void Initialize(string lobbyName, int maxPlayers, bool isPrivate, int roundCount)
        {
            this.lobbyName = lobbyName;
            this.maxPlayers = maxPlayers;
            this.isPrivate = isPrivate;
            this.roundCount = roundCount;
            mapIndex = 0;
            gameMode = "Standard";
            inGame = false;
            gameStarted = false;
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
            if (lobbyData.ContainsKey("RoundCount"))
                roundCount = int.Parse(lobbyData["RoundCount"].Value);
            if (lobbyData.ContainsKey("MapIndex"))
                mapIndex = int.Parse(lobbyData["MapIndex"].Value);
            if (lobbyData.ContainsKey("GameMode"))
                gameMode = lobbyData["GameMode"].Value;
            if (lobbyData.ContainsKey("RelayJoinCode"))
                relayJoinCode = lobbyData["RelayJoinCode"].Value;
            if (lobbyData.ContainsKey("InGame"))
                inGame = lobbyData["InGame"].Value == "true";
            if (lobbyData.ContainsKey("GameStarted"))
                gameStarted = lobbyData["GameStarted"].Value == "true";
        }

        public Dictionary<string, string> Serialize()
        {
            var data = new Dictionary<string, string>
            {
                { "LobbyName", lobbyName },
                { "MaxPlayers", maxPlayers.ToString() },
                { "IsPrivate", isPrivate.ToString().ToLower() },
                { "RoundCount", roundCount.ToString() },
                { "MapIndex", mapIndex.ToString() },
                { "GameMode", gameMode },
                { "InGame", inGame ? "true" : "false" },
                { "GameStarted", gameStarted ? "true" : "false" }
            };

            if (RelayJoinCode != default)
                data.Add("RelayJoinCode", RelayJoinCode);

            return data;
        }
    }
}