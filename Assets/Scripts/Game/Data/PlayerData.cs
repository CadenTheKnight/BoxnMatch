using UnityEngine;
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
        public string Name { get; set; } = "BoxnPlayer";
        public int Level { get; set; } = 1;
        public float Experience { get; set; } = 0f;
        public Team Team { get; set; } = Team.Blue;
        public PlayerStatus Status { get; set; } = PlayerStatus.NotReady;

        public void Initialize(string playerName)
        {
            Name = playerName;
            Level = PlayerPrefs.GetInt("PlayerLevel", 1);
            Experience = PlayerPrefs.GetFloat("PlayerExperience", 0f);
            Team = Team.Blue;
            Status = PlayerStatus.NotReady;
        }

        public Dictionary<string, PlayerDataObject> Serialize()
        {
            return new Dictionary<string, PlayerDataObject>
            {
                { "Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Name) },
                { "Level", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Level.ToString()) },
                { "Experience", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Experience.ToString()) },
                { "Team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Team).ToString()) },
                { "Status", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)Status).ToString()) }
            };
        }
    }
}