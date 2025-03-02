using Assets.Scripts.Game.Managers;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents strongly-typed player data for a lobby player.
    /// </summary>
    public class LobbyPlayerData
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string CharacterId { get; set; }
        public string ColorId { get; set; }
        public int TeamId { get; set; }
        public bool IsReady { get; set; }
        public bool IsHost { get; set; }

        /// <summary>
        /// Creates a new instance of LobbyPlayerData.
        /// </summary>
        public LobbyPlayerData(string playerId, string playerName, bool isHost = false)
            : this(playerId, playerName,
                   PlayerDataManager.PlayerConstants.DEFAULT_CHARACTER_ID,
                   PlayerDataManager.PlayerConstants.DEFAULT_COLOR_ID,
                   PlayerDataManager.PlayerConstants.DEFAULT_TEAM_ID,
                   PlayerDataManager.PlayerConstants.DEFAULT_READY_STATUS,
                   isHost)
        {
        }

        /// <summary>
        /// Creates a new instance of LobbyPlayerData with full parameters.
        /// </summary>
        public LobbyPlayerData(string playerId, string playerName, string characterId,
                             string colorId, int teamId, bool isReady, bool isHost)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            CharacterId = characterId;
            ColorId = colorId;
            TeamId = teamId;
            IsReady = isReady;
            IsHost = isHost;
        }
    }
}