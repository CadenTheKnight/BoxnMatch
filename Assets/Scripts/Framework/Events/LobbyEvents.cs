using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Events
{
    /// <summary>
    /// Contains events related to the lobby service in Unity Services.
    /// </summary>
    public static class LobbyEvents
    {
        #region Events
        /// <summary>
        /// Triggered when a new lobby is successfully created.
        /// </summary>
        /// <param name="result"> The result of the lobby creation operation.</param>
        public delegate void LobbyCreatedHandler(OperationResult result);
        public static event LobbyCreatedHandler OnLobbyCreated;

        /// <summary>
        /// Triggered when the lobby data is changed.
        /// </summary>
        public delegate void LobbyChangedHandler();
        public static event LobbyChangedHandler OnLobbyChanged;

        /// <summary>
        /// Triggered when a player becomes the new lobby host.
        /// </summary>
        /// <param name="player">The player who is the new host.</param>
        public delegate void NewLobbyHostHandler(Player player);
        public static event NewLobbyHostHandler OnNewLobbyHost;

        /// <summary>
        /// Triggered when successfully joining an existing lobby.
        /// </summary>
        /// <param name="result">The result of the lobby join operation.</param>
        public delegate void LobbyJoinedHandler(OperationResult result);
        public static event LobbyJoinedHandler OnLobbyJoined;

        /// <summary>
        /// Triggered when successfully leaving a lobby.
        /// </summary>
        /// <param name="result">The result of the lobby leave operation.</param>
        public delegate void LobbyLeftHandler(OperationResult result);
        public static event LobbyLeftHandler OnLobbyLeft;

        /// <summary>
        /// Triggered when kicked from a lobby.
        /// </summary>
        /// <param name="result">The result of the lobby kick operation.</param>
        public delegate void LobbyKickedHandler(OperationResult result);
        public static event LobbyKickedHandler OnLobbyKicked;

        /// <summary>
        /// Triggered when a player joins the lobby.
        /// </summary>
        /// <param name="player">The player who joined the lobby.</param>
        public delegate void PlayerJoinedHandler(Player player);
        public static event PlayerJoinedHandler OnPlayerJoined;

        /// <summary>
        /// Triggered when a player leaves the lobby.
        /// </summary>
        /// <param name="player">The player who left the lobby.</param>
        public delegate void PlayerLeftHandler(Player player);
        public static event PlayerLeftHandler OnPlayerLeft;

        /// <summary>
        /// Triggered when a player is kicked from the lobby.
        /// </summary>
        /// <param name="player">The player who was kicked from the lobby.</param>
        public delegate void PlayerKickedHandler(Player player);
        public static event PlayerKickedHandler OnPlayerKicked;

        /// <summary>
        /// Triggered when a lobby query is successfully completed.
        /// </summary>
        /// <param name="result">The result of the lobby query operation.</param>
        public delegate void InvokeLobbyQueryResponseHandler(OperationResult result);
        public static event InvokeLobbyQueryResponseHandler OnLobbyQueryResponse;

        /// <summary>
        /// Triggered when there is an error during a lobby operation.
        /// </summary>
        /// <param name="result">The result of the lobby operation.</param>
        public delegate void LobbyErrorHandler(OperationResult result);
        public static event LobbyErrorHandler OnLobbyError;

        /// <summary>
        /// Triggered when the lobby data is changed.
        /// </summary>
        /// <param name="result">The result of the lobby data change operation.</param>
        public delegate void LobbyDataChangedHandler(OperationResult result);
        public static event LobbyDataChangedHandler OnLobbyDataChanged;

        /// <summary>
        /// Triggered when the lobby map index is changed.
        /// </summary>
        /// <param name="index">The new map index.</param>
        public delegate void LobbyMapIndexChangedHandler(int index);
        public static event LobbyMapIndexChangedHandler OnLobbyMapIndexChanged;

        /// <summary>
        /// Triggered when the lobby round count is changed.
        /// </summary>
        /// <param name="count">The new round count.</param>
        public delegate void LobbyRoundCountChangedHandler(int count);
        public static event LobbyRoundCountChangedHandler OnLobbyRoundCountChanged;

        /// <summary>
        /// Triggered when the lobby round time is changed.
        /// </summary>
        /// <param name="time">The new round time.</param>
        public delegate void LobbyRoundTimeChangedHandler(int time);
        public static event LobbyRoundTimeChangedHandler OnLobbyRoundTimeChanged;

        /// <summary>
        /// Triggered when the lobby game mode is changed.
        /// </summary>
        /// <param name="mode">The new game mode.</param>
        public delegate void LobbyGameModeChangedHandler(GameMode mode);
        public static event LobbyGameModeChangedHandler OnLobbyGameModeChanged;

        /// <summary>
        /// Triggered when the lobby status is changed.
        /// </summary>
        /// <param name="status">The new status of the lobby.</param>
        public delegate void LobbyStatusChangedHandler(LobbyStatus status);
        public static event LobbyStatusChangedHandler OnLobbyStatusChanged;

        /// <summary>
        /// Triggered when the player data is changed.
        /// </summary>
        /// <param name="result">The result of the player data change operation.</param>
        public delegate void PlayerDataChangedHandler(OperationResult result);
        public static event PlayerDataChangedHandler OnPlayerDataChanged;

        /// <summary>
        /// Triggered when the player's status is changed.
        /// </summary>
        /// <param name="player">The player whose status has changed.</param>
        /// <param name="newStatus">The new status of the player.</param>
        public delegate void PlayerStatusChangedHandler(Player player, PlayerStatus newStatus);
        public static event PlayerStatusChangedHandler OnPlayerStatusChanged;

        /// <summary>
        /// Triggered when the player's team is changed.
        /// </summary>
        /// <param name="player">The player whose team has changed.</param>
        /// <param name="newTeam">The new team of the player.</param>
        public delegate void PlayerTeamChangedHandler(Player player, Team newTeam);
        public static event PlayerTeamChangedHandler OnPlayerTeamChanged;

        /// <summary>
        /// Triggered when a player is connecting to the lobby.
        /// </summary>
        /// <param name="player">The player who is connecting.</param>
        public delegate void PlayerConnectingHandler(Player player);
        public static event PlayerConnectingHandler OnPlayerConnecting;

        /// <summary>
        /// Triggered when a player is connected to the lobby.
        /// </summary>
        /// <param name="player">The player who is connected.</param>
        public delegate void PlayerConnectedHandler(Player player);
        public static event PlayerConnectedHandler OnPlayerConnected;

        /// <summary>
        /// Triggered when a player is disconnected from the lobby.
        /// </summary>
        /// <param name="player">The player who is disconnected.</param>
        public delegate void PlayerDisconnectedHandler(Player player);
        public static event PlayerDisconnectedHandler OnPlayerDisconnected;
        #endregion

        #region Invocations
        public static void InvokeLobbyCreated(OperationResult result) => OnLobbyCreated?.Invoke(result);
        public static void InvokeLobbyChanged() => OnLobbyChanged?.Invoke();
        public static void InvokeNewLobbyHost(Player player) => OnNewLobbyHost?.Invoke(player);
        public static void InvokeLobbyJoined(OperationResult result) => OnLobbyJoined?.Invoke(result);
        public static void InvokeLobbyLeft(OperationResult result) => OnLobbyLeft?.Invoke(result);
        public static void InvokeLobbyKicked(OperationResult result) => OnLobbyKicked?.Invoke(result);
        public static void InvokePlayerJoined(Player player) => OnPlayerJoined?.Invoke(player);
        public static void InvokePlayerLeft(Player player) => OnPlayerLeft?.Invoke(player);
        public static void InvokePlayerKicked(Player player) => OnPlayerKicked?.Invoke(player);
        public static void InvokeLobbyQueryResponse(OperationResult result) => OnLobbyQueryResponse?.Invoke(result);
        public static void InvokeLobbyError(OperationResult result) => OnLobbyError?.Invoke(result);
        public static void InvokeLobbyDataChanged(OperationResult result) => OnLobbyDataChanged?.Invoke(result);
        public static void InvokeLobbyMapIndexChanged(int index) => OnLobbyMapIndexChanged?.Invoke(index);
        public static void InvokeLobbyRoundCountChanged(int count) => OnLobbyRoundCountChanged?.Invoke(count);
        public static void InvokeLobbyRoundTimeChanged(int time) => OnLobbyRoundTimeChanged?.Invoke(time);
        public static void InvokeLobbyGameModeChanged(GameMode mode) => OnLobbyGameModeChanged?.Invoke(mode);
        public static void InvokeLobbyStatusChanged(LobbyStatus status) => OnLobbyStatusChanged?.Invoke(status);
        public static void InvokePlayerDataChanged(OperationResult result) => OnPlayerDataChanged?.Invoke(result);
        public static void InvokePlayerStatusChanged(Player player, PlayerStatus newStatus) => OnPlayerStatusChanged?.Invoke(player, newStatus);
        public static void InvokePlayerTeamChanged(Player player, Team newTeam) => OnPlayerTeamChanged?.Invoke(player, newTeam);
        public static void InvokePlayerConnecting(Player player) => OnPlayerConnecting?.Invoke(player);
        public static void InvokePlayerConnected(Player player) => OnPlayerConnected?.Invoke(player);
        public static void InvokePlayerDisconnected(Player player) => OnPlayerDisconnected?.Invoke(player);
        #endregion
    }
}