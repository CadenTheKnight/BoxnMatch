using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    public class LobbyPlayerData
    {
        private string playerId;
        private string playerName;
        private bool isReady;

        public string PlayerId => playerId;
        public string PlayerName => playerName;
        public bool IsReady
        {
            get => isReady;
            set => isReady = value;
        }

        public void Initialize(string playerId, string playerName)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            isReady = false;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.ContainsKey("playerId"))
            {
                playerId = playerData["playerId"].Value;
            }
            if (playerData.ContainsKey("playerName"))
            {
                playerName = playerData["playerName"].Value;
            }
            if (playerData.ContainsKey("isReady"))
            {
                isReady = playerData["isReady"].Value == "True";
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
        {
            { "playerId", playerId },
            { "playerName", playerName },
            { "isReady", isReady.ToString() }
        };
        }
    }
}
