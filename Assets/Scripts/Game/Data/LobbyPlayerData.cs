using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents player data used in the lobby system.
    /// </summary>
    public class LobbyPlayerData
    {
        private string _id;
        private string _name;
        private Team _team;
        private PlayerStatus _status;

        public string Id
        {
            get => _id;
            set => SetValue(ref _id, value, nameof(Id));
        }

        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value, nameof(Name));
        }

        public Team Team
        {
            get => _team;
            set => SetValue(ref _team, value, nameof(Team));
        }

        public PlayerStatus Status
        {
            get => _status;
            set => SetValue(ref _status, value, nameof(Status));
        }

        private void SetValue<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                string oldValue = field?.ToString();
                field = value;
                string newValue = value?.ToString();
                LobbyEvents.InvokePlayerDataChanged(_id, propertyName, oldValue, newValue);
            }
        }

        public void Initialize(string playerId, string playerName)
        {
            _id = playerId;
            _name = playerName;
            _team = Team.Blue;
            _status = PlayerStatus.NotReady;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> lobbyPlayerData)
        {
            UpdateState(lobbyPlayerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> lobbyPlayerData)
        {
            if (lobbyPlayerData.TryGetValue("PlayerId", out PlayerDataObject playerIdObj))
                Id = playerIdObj.Value;

            if (lobbyPlayerData.TryGetValue("PlayerName", out PlayerDataObject playerNameObj))
                Name = playerNameObj.Value;

            if (lobbyPlayerData.TryGetValue("Team", out PlayerDataObject teamObj))
                Team = (Team)int.Parse(teamObj.Value);

            if (lobbyPlayerData.TryGetValue("Status", out PlayerDataObject statusObj))
                _status = (PlayerStatus)int.Parse(statusObj.Value);
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "PlayerId", _id },
                { "PlayerName", _name },
                { "Team", ((int)_team).ToString() },
                { "Status", ((int)_status).ToString() }
            };
        }
    }
}