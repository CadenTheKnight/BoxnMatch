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
        public int MapIndex { get; set; }
        public int RoundCount { get; set; }
        public int RoundTime { get; set; }
        public GameMode GameMode { get; set; }

        public LobbyData(GameMode gameMode)
        {
            MapIndex = 0;
            RoundCount = 3;
            RoundTime = 60;
            GameMode = gameMode;
        }

        public Dictionary<string, DataObject> Serialize()
        {
            return new Dictionary<string, DataObject>
            {
                { "MapIndex", new DataObject(DataObject.VisibilityOptions.Public, MapIndex.ToString()) },
                { "RoundCount", new DataObject(DataObject.VisibilityOptions.Public, RoundCount.ToString()) },
                { "RoundTime", new DataObject(DataObject.VisibilityOptions.Public, RoundTime.ToString()) },
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, ((int)GameMode).ToString()) },
            };
        }
    }
}