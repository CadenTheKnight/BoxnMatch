using System;

[Serializable]
public class LobbyData
{
    public string LobbyName;
    public int Players;
    public int Capacity;
    public string HostName;
    public string Status;

    public LobbyData(string lobbyName, int players, int capacity, string hostName, string status)
    {
        LobbyName = lobbyName;
        Players = players;
        Capacity = capacity;
        HostName = hostName;
        Status = status;
    }
}
