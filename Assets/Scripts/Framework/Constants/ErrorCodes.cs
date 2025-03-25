namespace Assets.Scripts.Framework.Constants
{
    /// <summary>
    /// Defines error codes used throughout the application.
    /// </summary>
    public static class ErrorCodes
    {
        public static class Auth
        {
            public const string INITIALIZATION_TIMEOUT = "InitializationTimeout";
            public const string NETWORK_ERROR = "NetworkError";
            public const string AUTHENTICATION_ERROR = "AuthenticationError";
            public const string INVALID_CREDENTIALS = "InvalidCredentials";
            public const string SERVER_ERROR = "ServerError";
            public const string RATE_LIMIT_EXCEEDED = "RateLimitExceeded";
        }

        public static class Lobby
        {
            public const string LOBBY_NOT_FOUND = "LobbyNotFound";
            public const string LOBBY_FULL = "LobbyFull";
            public const string LOBBY_CLOSED = "LobbyClosed";
            public const string PERMISSION_DENIED = "PermissionDenied";
            public const string HOST_DISCONNECTED = "HostDisconnected";
        }

        public static class Network
        {
            public const string CONNECTION_TIMEOUT = "ConnectionTimeout";
            public const string CONNECTION_LOST = "ConnectionLost";
            public const string SERVER_UNREACHABLE = "ServerUnreachable";
        }

        public const string UNKNOWN_ERROR = "UnknownError";
        public const string INTERNAL_ERROR = "InternalError";
    }
}