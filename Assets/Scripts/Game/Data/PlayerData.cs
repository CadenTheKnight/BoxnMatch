using Steamworks;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of player in the lobby system.
    /// </summary>
    public class PlayerData
    {
        public CSteamID SteamId { get; set; } = CSteamID.Nil;
        public Team Team { get; set; } = Team.Blue;
        public PlayerStatus Status { get; set; } = PlayerStatus.NotReady;

        public PlayerData(CSteamID steamId, Team team = Team.Blue, PlayerStatus status = PlayerStatus.NotReady)
        {
            SteamId = steamId;
            Team = team;
            Status = status;
        }

        public Dictionary<string, PlayerDataObject> Serialize()
        {
            return new Dictionary<string, PlayerDataObject>
            {
                { "SteamId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, SteamId.ToString()) },
                { "Team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Team).ToString()) },
                { "Status", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Status).ToString()) }
            };
        }
    }
}