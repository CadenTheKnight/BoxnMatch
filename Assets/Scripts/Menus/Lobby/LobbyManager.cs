using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing.Server;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance; // Singleton for global access

    private Dictionary<int, LobbyData> lobbies = new Dictionary<int, LobbyData>(); // Track lobbies by ID
    private int lobbyIdCounter = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CreateLobby(string lobbyName, int maxPlayers, NetworkConnection host)
    {
        int lobbyId = lobbyIdCounter++;
        LobbyData newLobby = new LobbyData(lobbyName, 1, maxPlayers, host.ToString(), "In Lobby");

        lobbies[lobbyId] = newLobby;
        Debug.Log($"Lobby Created: {lobbyName} (ID: {lobbyId})");
    }

    public List<LobbyData> GetLobbies()
    {
        return new List<LobbyData>(lobbies.Values);
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinLobbyServerRpc(int lobbyId, NetworkConnection conn)
    {
        if (lobbies.ContainsKey(lobbyId) && lobbies[lobbyId].Players < lobbies[lobbyId].Capacity)
        {
            lobbies[lobbyId].Players++;
            Debug.Log($"{conn} joined {lobbies[lobbyId].LobbyName}");

            // Send updated lobby list to all clients
            UpdateLobbyListForClients();
        }
        else
        {
            Debug.Log("Lobby is full or doesn't exist!");
        }
    }

    [ObserversRpc]
    private void UpdateLobbyListForClients()
    {
        Debug.Log("Lobby list updated for all clients.");
        // This can be extended to actually update UI on the client side.
    }
}
