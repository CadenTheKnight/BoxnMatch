using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Events;


public class LobbyManager : Singleton<LobbyManager>
{
    private Lobby lobby;
    private Coroutine _heartbeatCoroutine;
    private Coroutine _refreshLobbyCoroutine;

    // public string GetLobbyName()
    // {
    //     return _lobby?.Name;
    // }

    public string GetLobbyCode()
    {
        return lobby?.LobbyCode;
    }

    public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        Player player = new(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

        CreateLobbyOptions options = new()
        {
            Data = SerializeLobbyData(lobbyData),
            IsPrivate = isPrivate,
            Player = player
        };

        try
        {
            lobby = await LobbyService.Instance.CreateLobbyAsync("Test Lobby", maxPlayers, options);
        }
        catch (System.Exception)
        {
            return false;
        }

        _heartbeatCoroutine = StartCoroutine(routine: HeartbeatLobbyCoroutine(lobby.Id, waitTimeSeconds: 6f));
        _refreshLobbyCoroutine = StartCoroutine(routine: RefreshLobbyCoroutine(lobby.Id, waitTimeSeconds: 1f));

        Debug.Log(message: $"Lobby created with lobby ID: {lobby.Id}");
        return true;
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            Debug.Log(message: "Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);
            Lobby newLobby = task.Result;
            if (newLobby.LastUpdated > lobby.LastUpdated)
            {
                lobby = newLobby;
                LobbyEvents.OnLobbyUpdated?.Invoke(lobby);
            }

            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = new();
        foreach (KeyValuePair<string, string> kvp in data)
        {
            playerData.Add(kvp.Key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, kvp.Value));
        }

        return playerData;
    }

    private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> lobbyData = new();
        foreach (KeyValuePair<string, string> kvp in data)
        {
            lobbyData.Add(kvp.Key, new DataObject(DataObject.VisibilityOptions.Member, kvp.Value));
        }

        return lobbyData;
    }

    public void OnApplicationQuit()
    {
        if (lobby != null && lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
        }
    }

    public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
    {
        JoinLobbyByCodeOptions options = new();
        Player player = new(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));

        options.Player = player;

        try
        {
            lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
        }
        catch (System.Exception)
        {
            return false;
        }

        _refreshLobbyCoroutine = StartCoroutine(routine: RefreshLobbyCoroutine(lobby.Id, waitTimeSeconds: 1f));

        Debug.Log(message: $"Lobby joined with lobby ID: {lobby.Id}");
        return true;
    }

    public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
    {
        List<Dictionary<string, PlayerDataObject>> data = new();

        foreach (Player player in lobby.Players)
        {
            data.Add(player.Data);
        }

        return data;
    }

    public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        UpdatePlayerOptions playerOptions = new()
        {
            Data = playerData
        };
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, playerOptions);
        }
        catch (System.Exception)
        {
            return false;
        }

        LobbyEvents.OnLobbyUpdated(lobby);
        return true;
    }

    public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
    {
        Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);
        UpdateLobbyOptions lobbyOptions = new()
        {
            Data = lobbyData
        };

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, lobbyOptions);
        }
        catch (System.Exception)
        {
            return false;
        }

        LobbyEvents.OnLobbyUpdated(lobby);
        return true;
    }

    public string GetHostId()
    {
        return lobby?.HostId;
    }
}