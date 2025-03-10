using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents player data used in the lobby system.
    /// </summary>
    public class LobbyPlayerData
    {
        private string playerId;
        private string playerName;
        private bool isReady;
        private int teamId;

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
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.ContainsKey("PlayerId"))
                playerId = playerData["PlayerId"].Value;
            if (playerData.ContainsKey("PlayerName"))
                playerName = playerData["PlayerName"].Value;
            if (playerData.ContainsKey("IsReady"))
                isReady = playerData["IsReady"].Value == "true";
            if (playerData.ContainsKey("TeamId"))
                teamId = int.Parse(playerData["TeamId"].Value);
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "PlayerId", playerId },
                { "PlayerName", playerName },
                { "IsReady", isReady.ToString().ToLower() },
                { "TeamId", teamId.ToString() }
            };
        }
    }
}