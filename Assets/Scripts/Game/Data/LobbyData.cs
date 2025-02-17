using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    public class LobbyData
    {
        public int mapIndex;

        public int MapIndex
        {
            get => mapIndex;
            set => mapIndex = value;
        }

        public void Initialize(int mapIndex)
        {
            MapIndex = mapIndex;
        }

        public void Initialize(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.ContainsKey("MapIndex"))
            {
                MapIndex = Int32.Parse(lobbyData["MapIndex"].Value);
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "MapIndex", MapIndex.ToString() }
            };
        }
    }
}
