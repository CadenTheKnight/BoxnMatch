using System;
using System.Linq;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Assets.Scripts.Framework.Core;

namespace Assets.Scripts.Game.Managers
{
    public class RelayManager : Singleton<RelayManager>
    {
        private bool isHost = false;
        public string relayJoinCode;
        private string relayServerAddress;
        private int relayServerPort;
        private byte[] key;
        private byte[] connectionData;
        private byte[] hostConnectionData;
        private Guid allocationId;
        private byte[] allocationIdBytes;

        public bool IsHost
        {
            get { return isHost; }
            private set { isHost = value; }
        }

        public string GetAllocationId()
        {
            return allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return connectionData.ToString();
        }

        public async Task<string> CreateRelay(int maxConnection)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            relayServerAddress = dtlsEndpoint.Host;
            relayServerPort = dtlsEndpoint.Port;

            allocationId = allocation.AllocationId;
            allocationIdBytes = allocation.AllocationIdBytes;
            connectionData = allocation.ConnectionData;
            key = allocation.Key;

            IsHost = true;

            return relayJoinCode;
        }

        public async Task<bool> JoinRelay(string joinCode)
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerEndpoint dtlsEndpoint = joinAllocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            relayServerAddress = dtlsEndpoint.Host;
            relayServerPort = dtlsEndpoint.Port;

            allocationId = joinAllocation.AllocationId;
            allocationIdBytes = joinAllocation.AllocationIdBytes;
            connectionData = joinAllocation.ConnectionData;
            hostConnectionData = joinAllocation.HostConnectionData;
            key = joinAllocation.Key;

            return true;
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) GetHostConnectionInfo()
        {
            return (allocationIdBytes, key, connectionData, relayServerAddress, relayServerPort);
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) GetClientConnectionInfo()
        {
            return (allocationIdBytes, key, connectionData, hostConnectionData, relayServerAddress, relayServerPort);
        }
    }
}