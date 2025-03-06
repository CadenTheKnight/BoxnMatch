namespace Assets.Scripts.Framework.Models
{
    /// <summary>
    /// Options for creating a new lobby
    /// </summary>
    public class LobbyCreationOptions
    {
        /// <summary>
        /// Whether the lobby is private (not discoverable in lobby list)
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Selected game mode
        /// </summary>
        public string GameMode { get; set; }

        /// ... other options
    }
}