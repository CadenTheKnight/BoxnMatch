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
        private string _playerId;
        private string _playerName;
        private bool _isReady;
        private int _teamId;
        private bool _inGame;

        public string PlayerId
        {
            get => _playerId;
            private set => SetValue(ref _playerId, value, nameof(PlayerId));
        }

        public string PlayerName
        {
            get => _playerName;
            set => SetValue(ref _playerName, value, nameof(PlayerName));
        }

        public bool IsReady
        {
            get => _isReady;
            set => SetValue(ref _isReady, value, nameof(IsReady));
        }

        public int TeamId
        {
            get => _teamId;
            set => SetValue(ref _teamId, value, nameof(TeamId));
        }

        public bool InGame
        {
            get => _inGame;
            set => SetValue(ref _inGame, value, nameof(InGame));
        }

        private void SetValue<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                string oldValue = field?.ToString();
                field = value;
                string newValue = value?.ToString();
                LobbyEvents.InvokePlayerDataChanged(_playerId, propertyName, oldValue, newValue);
            }
        }

        public void Initialize(string playerId, string playerName)
        {
            _playerId = playerId;
            _playerName = playerName;
            _isReady = false;
            _teamId = 0;
            _inGame = false;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.TryGetValue("PlayerId", out PlayerDataObject playerIdObj))
                PlayerId = playerIdObj.Value;

            if (playerData.TryGetValue("PlayerName", out PlayerDataObject playerNameObj))
                PlayerName = playerNameObj.Value;

            if (playerData.TryGetValue("IsReady", out PlayerDataObject isReadyObj))
                IsReady = isReadyObj.Value == "true";

            if (playerData.TryGetValue("TeamId", out PlayerDataObject teamIdObj) &&
                int.TryParse(teamIdObj.Value, out int teamId))
                TeamId = teamId;

            if (playerData.TryGetValue("InGame", out PlayerDataObject inGameObj))
                InGame = inGameObj.Value == "true";
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "PlayerId", _playerId },
                { "PlayerName", _playerName },
                { "IsReady", _isReady.ToString().ToLower() },
                { "TeamId", _teamId.ToString() },
                { "InGame", _inGame.ToString().ToLower() }
            };
        }
    }
}