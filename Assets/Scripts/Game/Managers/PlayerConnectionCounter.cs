using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Tracks connected players for accurate connection counts
    /// </summary>
    public class PlayerConnectionCounter : NetworkBehaviour
    {
        public static PlayerConnectionCounter Instance { get; private set; }

        // Networked player count
        private NetworkVariable<int> connectedPlayersCount = new NetworkVariable<int>(0);

        // Local player tracking
        private HashSet<ulong> connectedPlayerIds = new HashSet<ulong>();

        // Public property to get count
        public int ConnectedPlayersCount => connectedPlayersCount.Value;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Server listens for connections
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

                // Count existing connections
                UpdatePlayerCount();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            base.OnNetworkDespawn();
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer) return;

            connectedPlayerIds.Add(clientId);
            UpdatePlayerCount();
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!IsServer) return;

            connectedPlayerIds.Remove(clientId);
            UpdatePlayerCount();
        }

        private void UpdatePlayerCount()
        {
            if (!IsServer) return;

            connectedPlayersCount.Value = connectedPlayerIds.Count;
            Debug.Log($"Updated player count: {connectedPlayersCount.Value}");
        }
    }
}