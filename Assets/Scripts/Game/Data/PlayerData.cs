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
        public ReadyStatus ReadyStatus { get; set; } = ReadyStatus.NotReady;
        public ConnectionStatus ConnectionStatus { get; set; } = ConnectionStatus.Disconnected;

        public PlayerData(CSteamID steamId, Team team = Team.Blue, ReadyStatus readyStatus = ReadyStatus.NotReady, ConnectionStatus connectionStatus = ConnectionStatus.Disconnected)
        {
            SteamId = steamId;
            Team = team;
            ReadyStatus = readyStatus;
            ConnectionStatus = connectionStatus;
        }

        public Dictionary<string, PlayerDataObject> Serialize()
        {
            return new Dictionary<string, PlayerDataObject>
            {
                { "SteamId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, SteamId.ToString()) },
                { "Team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Team).ToString()) },
                { "ReadyStatus", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)ReadyStatus).ToString()) },
                { "ConnectionStatus", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)ConnectionStatus).ToString()) }
            };
        }
    }
}