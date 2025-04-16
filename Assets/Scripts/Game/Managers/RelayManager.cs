using System;
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
        private Allocation hostAllocation;
        private JoinAllocation playerAllocation;

        /// <summary>
        /// Creates a relay allocation for hosting.
        /// </summary>
        public async Task<string> CreateRelay(int maxConnections)
        {
            try
            {
                hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var relayServerData = new RelayServerData(hostAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                return joinCode;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Joins an existing relay allocation
        /// </summary>
        public async Task<bool> JoinRelay(string joinCode)
        {
            try
            {
                playerAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var relayServerData = new RelayServerData(playerAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();

                return true;
            }
            catch (Exception) { return false; }
        }
    }
}