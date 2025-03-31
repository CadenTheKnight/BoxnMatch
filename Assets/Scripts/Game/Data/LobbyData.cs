using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a lobby.
    /// </summary>
    public class LobbyData
    {
        private string _lobbyName;
        private bool _isPrivate;
        private int _maxPlayers;
        private int _roundCount;
        private int _roundTime;
        private int _mapIndex;
        private GameMode _gameMode;
        private LobbyStatus _status;
        private string _relayJoinCode;

        public string LobbyName
        {
            get => _lobbyName;
            set => SetValue(ref _lobbyName, value, nameof(LobbyName));
        }

        public bool IsPrivate
        {
            get => _isPrivate;
            set => SetValue(ref _isPrivate, value, nameof(IsPrivate));
        }

        public int MaxPlayers
        {
            get => _maxPlayers;
            set => SetValue(ref _maxPlayers, value, nameof(MaxPlayers));
        }

        public int RoundCount
        {
            get => _roundCount;
            set => SetValue(ref _roundCount, value, nameof(RoundCount));
        }

        public int RoundTime
        {
            get => _roundTime;
            set => SetValue(ref _roundTime, value, nameof(RoundTime));
        }

        public int MapIndex
        {
            get => _mapIndex;
            set => SetValue(ref _mapIndex, value, nameof(MapIndex));
        }

        public GameMode GameMode
        {
            get => _gameMode;
            set => SetValue(ref _gameMode, value, nameof(GameMode));
        }

        public LobbyStatus Status
        {
            get => _status;
            set => SetValue(ref _status, value, nameof(Status));
        }

        public string RelayJoinCode
        {
            get => _relayJoinCode;
            set => SetValue(ref _relayJoinCode, value, nameof(RelayJoinCode));
        }

        private void SetValue<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                string oldValue = field?.ToString();
                field = value;
                string newValue = value?.ToString();
                LobbyEvents.InvokeLobbyDataChanged(propertyName, oldValue, newValue);
            }
        }

        public void Initialize(string lobbyName, bool isPrivate, int maxPlayers)
        {
            LobbyName = lobbyName;
            IsPrivate = isPrivate;
            MaxPlayers = maxPlayers;
            RoundCount = 3;
            RoundTime = 90;
            MapIndex = 0;
            GameMode = GameMode.Teams;
            Status = LobbyStatus.InLobby;
        }

        public void Initialize(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.TryGetValue("LobbyName", out DataObject lobbyNameObj))
                LobbyName = lobbyNameObj.Value;

            if (lobbyData.TryGetValue("IsPrivate", out DataObject isPrivateObj))
                IsPrivate = isPrivateObj.Value == "true";

            if (lobbyData.TryGetValue("MaxPlayers", out DataObject maxPlayersObj))
                MaxPlayers = int.Parse(maxPlayersObj.Value);

            if (lobbyData.TryGetValue("RoundCount", out DataObject roundCountObj))
                RoundCount = int.Parse(roundCountObj.Value);

            if (lobbyData.TryGetValue("RoundTime", out DataObject roundTimeObj))
                RoundTime = int.Parse(roundTimeObj.Value);

            if (lobbyData.TryGetValue("MapIndex", out DataObject mapIndexObj))
                MapIndex = int.Parse(mapIndexObj.Value);

            if (lobbyData.TryGetValue("GameMode", out DataObject gameModeObj))
                GameMode = (GameMode)int.Parse(gameModeObj.Value);

            if (lobbyData.TryGetValue("Status", out DataObject statusObj))
                Status = (LobbyStatus)int.Parse(statusObj.Value);

            if (lobbyData.TryGetValue("RelayJoinCode", out DataObject relayJoinCodeObj))
                RelayJoinCode = relayJoinCodeObj.Value;
        }

        public Dictionary<string, string> Serialize()
        {
            var data = new Dictionary<string, string>
            {
                { "LobbyName", _lobbyName },
                { "IsPrivate", _isPrivate.ToString().ToLower() },
                { "MaxPlayers", _maxPlayers.ToString() },
                { "RoundCount", _roundCount.ToString() },
                { "RoundTime", _roundTime.ToString() },
                { "MapIndex", _mapIndex.ToString() },
                { "GameMode", ((int)_gameMode).ToString() },
                { "Status", ((int)_status).ToString() }
            };

            if (RelayJoinCode != default)
                data.Add("RelayJoinCode", RelayJoinCode);

            return data;
        }
    }
}