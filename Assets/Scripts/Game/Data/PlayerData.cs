using Steamworks;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of player.
    /// </summary>
    public class PlayerData
    {
        public CSteamID Id { get; set; } = CSteamID.Nil;
        public Team Team { get; set; } = Team.Blue;
        public PlayerStatus Status { get; set; } = PlayerStatus.NotReady;

        public Dictionary<string, PlayerDataObject> Serialize()
        {
            return new Dictionary<string, PlayerDataObject>
            {
                { "Id", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, Id.ToString()) },
                { "Team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Team).ToString()) },
                { "Status", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Status).ToString()) }
            };
        }
    }
}