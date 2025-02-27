using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a player in a lobby, including the player ID, name, ready status, character ID, color ID, and team ID.
    /// </summary>
    public class LobbyPlayerData
    {
        private string playerId;
        private string playerName;
        private bool isReady;
        private string characterId = "default";
        private string colorId = "default";
        private int teamId = 0;

        public string PlayerId => playerId;
        public string PlayerName => playerName;
        public bool IsReady { get => isReady; set => isReady = value; }
        public string CharacterId { get => characterId; set => characterId = value; }
        public string ColorId { get => colorId; set => colorId = value; }
        public int TeamId { get => teamId; set => teamId = value; }

        public LobbyPlayerData(string playerId, string playerName, bool isReady)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.isReady = isReady;

        }

        public LobbyPlayerData(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData == null) return;

            if (playerData.TryGetValue("playerId", out var idData))
                playerId = idData.Value;

            if (playerData.TryGetValue("playerName", out var nameData))
                playerName = nameData.Value;

            if (playerData.TryGetValue("isReady", out var readyData))
                bool.TryParse(readyData.Value, out isReady);

            if (playerData.TryGetValue("characterId", out var characterData))
                characterId = characterData.Value;

            if (playerData.TryGetValue("colorId", out var colorData))
                colorId = colorData.Value;

            if (playerData.TryGetValue("teamId", out var teamData))
                int.TryParse(teamData.Value, out teamId);
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "playerId", playerId },
                { "playerName", playerName },
                { "isReady", isReady.ToString() },
                { "characterId", characterId },
                { "colorId", colorId },
                { "teamId", teamId.ToString() }
            };
        }
    }
}