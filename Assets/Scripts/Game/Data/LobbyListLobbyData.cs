using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.Game.Data
{
    /// <summary>
    /// Represents the data of a lobby in the lobby list, including the lobby ID, name, current players, max players, host name, and joinability.
    /// </summary>
    [Serializable]
    public class LobbyListLobbyData
    {
        #region Fields and Properties

        private string lobbyId;
        private string lobbyName;
        private int currentPlayers;
        private int maxPlayers;
        private string hostName;
        private bool isPublic;
        private bool isJoinable;
        private string gameMode;
        private DateTime lastUpdated;

        /// <summary>
        /// Unique identifier for the lobby
        /// </summary>
        public string LobbyId { get => lobbyId; set => lobbyId = value; }

        /// <summary>
        /// Display name of the lobby
        /// </summary>
        public string LobbyName { get => lobbyName; set => lobbyName = value; }

        /// <summary>
        /// Number of players currently in the lobby
        /// </summary>
        public int CurrentPlayers
        {
            get => currentPlayers;
            set => currentPlayers = Math.Max(0, value);
        }

        /// <summary>
        /// Maximum number of players allowed in the lobby
        /// </summary>
        public int MaxPlayers
        {
            get => maxPlayers;
            set => maxPlayers = Math.Max(1, value);
        }

        /// <summary>
        /// Name of the lobby host
        /// </summary>
        public string HostName { get => hostName; set => hostName = value; }

        /// <summary>
        /// Whether the lobby is publicly visible
        /// </summary>
        public bool IsPublic { get => isPublic; set => isPublic = value; }

        /// <summary>
        /// Whether players can join this lobby
        /// </summary>
        public bool IsJoinable { get => isJoinable; set => isJoinable = value; }

        /// <summary>
        /// The game mode for this lobby (e.g., "Team Deathmatch")
        /// </summary>
        public string GameMode { get => gameMode; set => gameMode = value; }

        /// <summary>
        /// When this lobby data was last updated
        /// </summary>
        public DateTime LastUpdated { get => lastUpdated; set => lastUpdated = value; }

        #endregion

        #region Derived Properties

        /// <summary>
        /// Whether the lobby is currently full
        /// </summary>
        public bool IsFull => currentPlayers >= maxPlayers;

        /// <summary>
        /// Number of available player slots
        /// </summary>
        public int AvailableSlots => Math.Max(0, maxPlayers - currentPlayers);

        /// <summary>
        /// Whether this lobby can be joined (is joinable, not full, and has available slots)
        /// </summary>
        public bool CanJoin => isJoinable && !IsFull && AvailableSlots > 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new lobby list item with specified properties
        /// </summary>
        public LobbyListLobbyData(string lobbyId, string lobbyName, int currentPlayers, int maxPlayers,
                                 string hostName, bool isPublic, bool isJoinable, string gameMode = "Standard")
        {
            this.lobbyId = lobbyId ?? string.Empty;
            this.lobbyName = lobbyName ?? "Unnamed Lobby";
            this.currentPlayers = Math.Max(0, currentPlayers);
            this.maxPlayers = Math.Max(1, maxPlayers);
            this.hostName = hostName ?? "Unknown Host";
            this.isPublic = isPublic;
            this.isJoinable = isJoinable;
            this.gameMode = gameMode ?? "Standard";
            lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Creates a lobby list item from Unity Lobby service data
        /// </summary>
        public LobbyListLobbyData(Dictionary<string, DataObject> lobbyListLobbyData)
        {
            if (lobbyListLobbyData == null)
            {
                lobbyId = string.Empty;
                lobbyName = "Unnamed Lobby";
                currentPlayers = 0;
                maxPlayers = 4;
                hostName = "Unknown Host";
                isPublic = true;
                isJoinable = true;
                gameMode = "Standard";
                lastUpdated = DateTime.Now;
                return;
            }

            if (lobbyListLobbyData.TryGetValue("lobbyId", out var idData))
                lobbyId = idData.Value;
            else
                lobbyId = string.Empty;

            if (lobbyListLobbyData.TryGetValue("lobbyName", out var nameData))
                lobbyName = nameData.Value;
            else
                lobbyName = "Unnamed Lobby";

            if (lobbyListLobbyData.TryGetValue("currentPlayers", out var currentPlayersData) &&
                int.TryParse(currentPlayersData.Value, out int parsedCurrentPlayers))
                currentPlayers = Math.Max(0, parsedCurrentPlayers);
            else
                currentPlayers = 0;

            if (lobbyListLobbyData.TryGetValue("maxPlayers", out var maxPlayersData) &&
                int.TryParse(maxPlayersData.Value, out int parsedMaxPlayers))
                maxPlayers = Math.Max(1, parsedMaxPlayers);
            else
                maxPlayers = 4;

            if (lobbyListLobbyData.TryGetValue("hostName", out var hostNameData))
                hostName = hostNameData.Value;
            else
                hostName = "Unknown Host";

            if (lobbyListLobbyData.TryGetValue("isPublic", out var isPublicData) &&
                bool.TryParse(isPublicData.Value, out bool parsedIsPublic))
                isPublic = parsedIsPublic;
            else
                isPublic = true;

            if (lobbyListLobbyData.TryGetValue("isJoinable", out var isJoinableData) &&
                bool.TryParse(isJoinableData.Value, out bool parsedIsJoinable))
                isJoinable = parsedIsJoinable;
            else
                isJoinable = true;

            if (lobbyListLobbyData.TryGetValue("gameMode", out var gameModeData))
                gameMode = gameModeData.Value;
            else
                gameMode = "Standard";

            lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Creates a lobby list item from a Unity Lobby object 
        /// </summary>
        public static LobbyListLobbyData FromLobby(Lobby lobby)
        {
            if (lobby == null)
                return null;

            string hostName = "Unknown Host";
            if (lobby.Players != null && lobby.Players.Count > 0)
            {
                var hostPlayer = lobby.Players.Find(p => p.Id == lobby.HostId);
                if (hostPlayer != null && hostPlayer.Data != null &&
                    hostPlayer.Data.TryGetValue("PlayerName", out var nameData))
                    hostName = nameData.Value;
            }

            string gameMode = "Standard";
            if (lobby.Data != null && lobby.Data.TryGetValue("GameMode", out var modeData))
                gameMode = modeData.Value;


            return new LobbyListLobbyData(
                lobby.Id,
                lobby.Name,
                lobby.Players?.Count ?? 0,
                lobby.MaxPlayers,
                hostName,
                !lobby.IsPrivate,
                true,
                gameMode
            );
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Converts the lobby data to a dictionary for storage or transmission
        /// </summary>
        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "lobbyId", lobbyId ?? string.Empty },
                { "lobbyName", lobbyName ?? "Unnamed Lobby" },
                { "currentPlayers", currentPlayers.ToString() },
                { "maxPlayers", maxPlayers.ToString() },
                { "hostName", hostName ?? "Unknown Host" },
                { "isPublic", isPublic.ToString() },
                { "isJoinable", isJoinable.ToString() },
                { "gameMode", gameMode ?? "Standard" },
                { "lastUpdated", DateTime.Now.ToString("o") }
            };
        }

        /// <summary>
        /// Creates a LobbyData object compatible with Unity's Lobby service
        /// </summary>
        public Dictionary<string, DataObject> ToLobbyData()
        {
            return new Dictionary<string, DataObject>
            {
                { "lobbyName", new DataObject(DataObject.VisibilityOptions.Public, lobbyName ?? "Unnamed Lobby") },
                { "currentPlayers", new DataObject(DataObject.VisibilityOptions.Public, currentPlayers.ToString()) },
                { "maxPlayers", new DataObject(DataObject.VisibilityOptions.Public, maxPlayers.ToString()) },
                { "hostName", new DataObject(DataObject.VisibilityOptions.Public, hostName ?? "Unknown Host") },
                { "isPublic", new DataObject(DataObject.VisibilityOptions.Public, isPublic.ToString()) },
                { "isJoinable", new DataObject(DataObject.VisibilityOptions.Public, isJoinable.ToString()) },
                { "gameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode ?? "Standard") }
            };
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Updates the lobby data with the latest information
        /// </summary>
        public void Update(int currentPlayers, bool isJoinable)
        {
            this.currentPlayers = Math.Max(0, currentPlayers);
            this.isJoinable = isJoinable;
            lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns a string representation of the lobby for debugging
        /// </summary>
        public override string ToString()
        {
            return $"{lobbyName} ({currentPlayers}/{maxPlayers}) - Host: {hostName} - {(isPublic ? "Public" : "Private")} - {(isJoinable ? "Joinable" : "Closed")} - Mode: {gameMode}";
        }

        #endregion
    }
}