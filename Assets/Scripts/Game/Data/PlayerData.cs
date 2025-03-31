using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents player data used throughout the game.
    /// </summary>
    public class PlayerData
    {
        private int _level;
        private float _experience;

        public int Level
        {
            get => _level;
            set => SetValue(ref _level, value, nameof(Level));
        }

        public float Experience
        {
            get => _experience;
            set => SetValue(ref _experience, value, nameof(Experience));
        }

        private void SetValue<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                string oldValue = field?.ToString();
                field = value;
                string newValue = value?.ToString();
                // LobbyEvents.InvokePlayerDataChanged(_playerId, propertyName, oldValue, newValue);
            }
        }

        public void Initialize()
        {
            Level = 0;
            Experience = 0f;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.TryGetValue("Level", out PlayerDataObject levelObj))
                Level = int.Parse(levelObj.Value);

            if (playerData.TryGetValue("Experience", out PlayerDataObject experienceObj))
                Experience = float.Parse(experienceObj.Value);
        }

        public Dictionary<string, PlayerDataObject> Serialize()
        {
            return new Dictionary<string, PlayerDataObject>
            {
                { "Level", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Level.ToString()) },
                { "Experience", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Experience.ToString()) }
            };
        }
    }
}