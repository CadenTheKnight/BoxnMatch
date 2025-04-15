namespace Assets.Scripts.Game.Types
{
    /// <summary>
    /// Defines the possible statuses of a player's connection to the lobby service.
    /// </summary>
    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Disconnected,
        Error
    }
}