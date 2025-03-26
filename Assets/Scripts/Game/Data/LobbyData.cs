using System.Collections.Generic;
using Assets.Scripts.Game.Events;
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
        private int _mapIndex;
        private string _gameMode;
        private string _relayJoinCode;
        private bool _inGame;
        private bool _gameStarted;

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

        public int MapIndex
        {
            get => _mapIndex;
            set => SetValue(ref _mapIndex, value, nameof(MapIndex));
        }

        public string GameMode
        {
            get => _gameMode;
            set => SetValue(ref _gameMode, value, nameof(GameMode));
        }

        public string RelayJoinCode
        {
            get => _relayJoinCode;
            set => SetValue(ref _relayJoinCode, value, nameof(RelayJoinCode));
        }

        public bool InGame
        {
            get => _inGame;
            set => SetValue(ref _inGame, value, nameof(InGame));
        }

        public bool GameStarted
        {
            get => _gameStarted;
            set => SetValue(ref _gameStarted, value, nameof(GameStarted));
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
            MapIndex = 0;
            GameMode = "Standard";
            InGame = false;
            GameStarted = false;
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

            if (lobbyData.TryGetValue("MaxPlayers", out DataObject maxPlayersObj) &&
                int.TryParse(maxPlayersObj.Value, out int maxPlayers))
                MaxPlayers = maxPlayers;

            if (lobbyData.TryGetValue("RoundCount", out DataObject roundCountObj) &&
                int.TryParse(roundCountObj.Value, out int roundCount))
                RoundCount = roundCount;

            if (lobbyData.TryGetValue("MapIndex", out DataObject mapIndexObj) &&
                int.TryParse(mapIndexObj.Value, out int mapIndex))
                MapIndex = mapIndex;

            if (lobbyData.TryGetValue("GameMode", out DataObject gameModeObj))
                GameMode = gameModeObj.Value;

            if (lobbyData.TryGetValue("RelayJoinCode", out DataObject relayJoinCodeObj))
                RelayJoinCode = relayJoinCodeObj.Value;

            if (lobbyData.TryGetValue("InGame", out DataObject inGameObj))
                InGame = inGameObj.Value == "true";

            if (lobbyData.TryGetValue("GameStarted", out DataObject gameStartedObj))
                GameStarted = gameStartedObj.Value == "true";
        }

        public Dictionary<string, string> Serialize()
        {
            var data = new Dictionary<string, string>
            {
                { "LobbyName", _lobbyName },
                { "IsPrivate", _isPrivate.ToString().ToLower() },
                { "MaxPlayers", _maxPlayers.ToString() },
                { "RoundCount", _roundCount.ToString() },
                { "MapIndex", _mapIndex.ToString() },
                { "GameMode", _gameMode },
                { "InGame", _inGame ? "true" : "false" },
                { "GameStarted", _gameStarted ? "true" : "false" }
            };

            if (RelayJoinCode != default)
                data.Add("RelayJoinCode", RelayJoinCode);

            return data;
        }
    }
}