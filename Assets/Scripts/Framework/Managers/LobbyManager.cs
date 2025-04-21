using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;


namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Handles low-level interaction with Unity's Lobby service.
    /// </summary>
    public class LobbyManager : Singleton<LobbyManager>
    {
        #region Public Properties

        public Lobby lobby;
        public string LobbyName => lobby.Name;
        public bool IsPrivate => lobby.IsPrivate;
        public string LobbyCode => IsInLobby ? lobby.LobbyCode : string.Empty;
        public bool IsInLobby => lobby != null;
        public string LobbyId => IsInLobby ? lobby.Id : string.Empty;
        public bool IsHostId(string playerId) => lobby.HostId == playerId;
        public bool IsLobbyHost => lobby.HostId == AuthenticationService.Instance.PlayerId;
        public int PlayerCount => lobby.Players.Count;
        public int MaxPlayers => lobby.MaxPlayers;
        public string GameMode => lobby.Data["GameMode"].Value;
        public bool IsGameInProgress => lobby.Data["GameInProgress"].Value == "true";
        public string RelayJoinCode => lobby.Data["JoinCode"].Value ?? string.Empty;
        public IReadOnlyList<Player> Players => IsInLobby ? lobby.Players : new List<Player>();

        #endregion

        #region Lifecycle Methods

        private Coroutine heartbeatCoroutine;
        private Coroutine refreshCoroutine;

        /// <summary>
        /// Handles the lobby heartbeat, sending regular updates to the lobby service.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to send heartbeats to.</param>
        /// <param name="waitTimeSeconds">The time to wait between heartbeats.</param>
        /// <returns>Coroutine for the heartbeat process.</returns>
        private IEnumerator HeartbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        /// <summary>
        /// Handles the lobby refresh, updating the lobby data at regular intervals.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to refresh.</param>
        /// <param name="refreshIntervalSeconds">The time to wait between refreshes.</param>
        /// <returns>Coroutine for the refresh process.</returns>
        private IEnumerator RefreshCoroutine(string lobbyId, float refreshIntervalSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);

                lobby = task.Result;
                LobbyEvents.InvokeLobbyUpdated(lobby);
                yield return new WaitForSecondsRealtime(refreshIntervalSeconds);
            }
        }

        #endregion

        #region Lobby Management

        // void Start()
        // {
        //     Application.wantsToQuit += OnApplicationQuit;
        // }

        /// <summary>
        /// Creates a new lobby with the specified parameters.
        /// </summary>
        /// <param name="lobbyData">Dictionary of lobby data.</param>
        /// <returns>Operation result indicating success or failure</returns>
        public async Task<OperationResult> CreateLobby(Dictionary<string, string> lobbyData, Dictionary<string, string> playerData)
        {

            Dictionary<string, PlayerDataObject> serializedPlayerData = SerializePlayerData(playerData);
            Dictionary<string, DataObject> serializedLobbyData = SerializeLobbyData(lobbyData);

            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = serializedLobbyData["IsPrivate"].Value == "true",
                Player = new Player(AuthenticationService.Instance.PlayerId, null, serializedPlayerData),
                Data = serializedLobbyData
            };

            try
            {
                lobby = await LobbyService.Instance.CreateLobbyAsync(serializedLobbyData["LobbyName"].Value, int.Parse(serializedLobbyData["MaxPlayers"].Value), createLobbyOptions);

                LobbyEvents.InvokeLobbyCreated(lobby);
                heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine(lobby.Id, 6f));
                refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                return OperationResult.SuccessResult("LobbyCreated", $"Lobby: {lobby.Name} created");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }



        /// <summary>
        /// Joins a lobby using its code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        /// <param name="playerData">The data for the player joining the lobby.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyByCode(string lobbyCode, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new()
            {
                Player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData))
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

                refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                LobbyEvents.InvokeLobbyJoined(lobby);

                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by code: {lobbyCode}");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Joins a lobby using its ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        /// <param name="joinLobbyByIdOptions">Additional options for joining the lobby.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> JoinLobbyById(string lobbyId, Dictionary<string, string> playerData)
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new()
            {
                Player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData))
            };

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

                refreshCoroutine = StartCoroutine(RefreshCoroutine(lobby.Id, 1f));

                LobbyEvents.InvokeLobbyJoined(lobby);

                return OperationResult.SuccessResult("LobbyJoined", $"Joined lobby by Id: {lobbyId}");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);

                StopAllCoroutines();

                LobbyEvents.InvokeLobbyLeft();

                return OperationResult.SuccessResult("LeaveLobby", "Left lobby successfully");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby.
        /// </summary>
        /// <param name="playerId">The ID of the player being kicked.</param>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> KickPlayer(string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobby.Id,
                    lobby?.Players.Find(player => player.Id == playerId)?.Id ?? playerId);

                if (AuthenticationService.Instance.PlayerId == playerId)
                    StopAllCoroutines();

                LobbyEvents.InvokeLobbyKicked();

                return OperationResult.SuccessResult("KickPlayer", $"Player {playerId} kicked from lobby");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }
        // public bool OnApplicationQuit()
        // {
        //     if (IsInLobby)
        //     {
        //         StartCoroutine(LeaveBeforeQuit());
        //         return false;
        //     }
        //     else
        //         return true;
        // }

        // private IEnumerator LeaveBeforeQuit()
        // {
        //     var task = LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);

        //     yield return new WaitUntil(() => task.IsCompleted);
        //     yield return new WaitUntil(() => lobby == null);

        //     Application.Quit();
        // }

        #endregion

        /// <summary>
        /// Retrieves a list of active lobbies.
        /// </summary>
        /// <returns>Operation result indicating success or failure.</returns>
        public async Task<OperationResult> GetLobbies()
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Order = new List<QueryOrder> { new(false, QueryOrder.FieldOptions.Created) }
            };

            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                LobbyEvents.InvokeLobbyListChanged(queryResponse.Results);

                return OperationResult.SuccessResult("GetLobbies", $"Retrieved {queryResponse.Results.Count} lobbies");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        #region Data Management

        public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
        {
            List<Dictionary<string, PlayerDataObject>> playersData = new();

            foreach (Player player in lobby.Players)
                playersData.Add(player.Data);

            return playersData;
        }

        public async Task<OperationResult> UpdatePlayerData(string playerId, Dictionary<string, string> playerData, string allocationId = default, string connectionData = default)
        {
            UpdatePlayerOptions updatePlayerOptions = new()
            {
                Data = SerializePlayerData(playerData),
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, updatePlayerOptions);

                LobbyEvents.InvokeLobbyUpdated(lobby);

                return OperationResult.SuccessResult("UpdatePlayerData", $"Player {playerId} data updated");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        public async Task<OperationResult> UpdateAllPlayerData(List<Dictionary<string, string>> playersData)
        {
            foreach (Dictionary<string, string> playerData in playersData)
            {
                UpdatePlayerOptions updatePlayerOptions = new()
                {
                    Data = SerializePlayerData(playerData)
                };

                try
                {
                    lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, updatePlayerOptions);

                    LobbyEvents.InvokeLobbyUpdated(lobby);
                }
                catch (LobbyServiceException e)
                {
                    return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
                }
            }

            return OperationResult.SuccessResult("UpdateAllPlayerData", "All player data updated");
        }

        public async Task<OperationResult> UpdateLobbyData(Dictionary<string, string> lobbyData)
        {
            UpdateLobbyOptions updateLobbyOptions = new()
            {
                Data = SerializeLobbyData(lobbyData)
            };

            try
            {
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);

                LobbyEvents.InvokeLobbyUpdated(lobby);

                return OperationResult.SuccessResult("UpdateLobbyData", $"Lobby {lobby.Name} data updated");
            }
            catch (LobbyServiceException e)
            {
                return OperationResult.ErrorResult(e.ErrorCode.ToString(), e.Message);
            }
        }

        /// <summary>
        /// Serializes player data into a format that can be sent to the Unity Lobby service.
        /// </summary>
        /// <param name="playerData">The player data to serialize.</param>
        /// <returns>A dictionary of data objects.</returns>
        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> playerData)
        {
            Dictionary<string, PlayerDataObject> serializedPlayerData = new();

            foreach (KeyValuePair<string, string> data in playerData)
                serializedPlayerData.Add(data.Key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, data.Value));

            return serializedPlayerData;
        }

        /// <summary>
        /// Serializes lobby data into a format that can be sent to the Unity Lobby service.
        /// </summary>
        /// <param name="lobbyData">The lobby data to serialize.</param>
        /// <returns>A dictionary of data objects.</returns>
        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> lobbyData)
        {
            Dictionary<string, DataObject> serializedLobbyData = new();

            foreach (KeyValuePair<string, string> data in lobbyData)
                serializedLobbyData.Add(data.Key, new DataObject(DataObject.VisibilityOptions.Public, data.Value));

            return serializedLobbyData;
        }

        #endregion
    }
}