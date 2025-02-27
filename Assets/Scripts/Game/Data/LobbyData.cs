using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a lobby, including the team mode, map index, round count, and match time.
    /// </summary>
    public class LobbyData
    {
        private int teamMode; // 0 = FFA, 1 = 2TEAMS
        private int mapIndex;
        private int roundCount;
        private int matchTimeMinutes;

        public int TeamMode { get => teamMode; set => teamMode = value; }
        public int MapIndex { get => mapIndex; set => mapIndex = value; }
        public int RoundCount { get => roundCount; set => roundCount = value; }
        public int MatchTimeMinutes { get => matchTimeMinutes; set => matchTimeMinutes = value; }

        public void Initialize(int teamMode = 1, int mapIndex = 0, int roundCount = 1, int matchTimeMinutes = 3)
        {
            this.teamMode = teamMode;
            this.mapIndex = mapIndex;
            this.roundCount = roundCount;
            this.matchTimeMinutes = matchTimeMinutes;
        }

        public void Initialize(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData == null) return;

            if (lobbyData.TryGetValue("teamMode", out var teamModeData))
                int.TryParse(teamModeData.Value, out teamMode);

            if (lobbyData.TryGetValue("mapIndex", out var mapIndexData))
                int.TryParse(mapIndexData.Value, out mapIndex);

            if (lobbyData.TryGetValue("roundCount", out var roundCountData))
                int.TryParse(roundCountData.Value, out roundCount);

            if (lobbyData.TryGetValue("matchTimeMinutes", out var matchTimeData))
                int.TryParse(matchTimeData.Value, out matchTimeMinutes);
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "teamMode", teamMode.ToString() },
                { "mapIndex", mapIndex.ToString() },
                { "roundCount", roundCount.ToString() },
                { "matchTimeMinutes", matchTimeMinutes.ToString() },
            };
        }
    }
}