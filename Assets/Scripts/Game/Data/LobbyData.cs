using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a lobby.
    /// </summary>
    public class LobbyData
    {
        public int MapIndex { get; set; } = 0;
        public int RoundCount { get; set; } = 3;
        public int RoundTime { get; set; } = 90;
        public GameMode GameMode { get; set; } = GameMode.Teams;
        public LobbyStatus Status { get; set; } = LobbyStatus.InLobby;
        public string RelayJoinCode { get; set; } = default;

        public Dictionary<string, DataObject> Serialize()
        {
            return new Dictionary<string, DataObject>
            {
                { "MapIndex", new DataObject(DataObject.VisibilityOptions.Public, MapIndex.ToString()) },
                { "RoundCount", new DataObject(DataObject.VisibilityOptions.Member, RoundCount.ToString()) },
                { "RoundTime", new DataObject(DataObject.VisibilityOptions.Member, RoundTime.ToString()) },
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, GameMode.ToString()) },
                { "Status", new DataObject(DataObject.VisibilityOptions.Public, Status.ToString()) },
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, RelayJoinCode) }
            };
        }
    }
}