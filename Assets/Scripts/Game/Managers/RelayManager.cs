using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Assets.Scripts.Framework.Core;
using Unity.Networking.Transport.Relay;

namespace Assets.Scripts.Game.Managers
{
    public class RelayManager : Singleton<RelayManager>
    {
        [SerializeField] private bool showDebugMessages = true;

        private Allocation hostAllocation;
        private JoinAllocation clientAllocation;

        /// <summary>
        /// Creates a relay allocation for hosting a game.
        /// </summary>
        public async Task<string> CreateRelay(int maxConnections)
        {
            try
            {
                if (showDebugMessages) Debug.Log($"Creating relay allocation for {maxConnections} players");

                hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

                if (showDebugMessages) Debug.Log($"Relay allocation created. Join code: {joinCode}");

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var relayServerData = new RelayServerData(hostAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                if (showDebugMessages) Debug.Log("Host transport configured with relay server data");

                return joinCode;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create relay allocation: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Joins an existing relay allocation.
        /// </summary>
        public async Task<bool> JoinRelay(string joinCode)
        {
            try
            {
                if (showDebugMessages) Debug.Log($"Joining relay allocation with code: {joinCode}");

                clientAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                if (showDebugMessages) Debug.Log($"Joined relay allocation successfully");

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var relayServerData = new RelayServerData(clientAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                if (showDebugMessages) Debug.Log("Client transport configured with relay server data");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join relay allocation: {e.Message}");
                return false;
            }
        }
    }
}