using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.Managers
{
    public class RelayManager : Singleton<RelayManager>
    {
        public string RelayJoinCode { get; private set; }
        public bool IsRelaySessionActive { get; private set; }

        // Add this initialization check
        private bool _isInitialized = false;

        private async Task EnsureInitialized()
        {
            if (_isInitialized) return;

            try
            {
                // Make sure Unity services are initialized

                // Ensure user is authenticated
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.Log("Signing in anonymously...");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                _isInitialized = true;
                Debug.Log("Relay services initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Unity services: {e.Message}");
                throw; // Re-throw to handle it in the calling method
            }
        }

        // For the host to set up a relay server
        public async Task<string> CreateRelaySession(int maxPlayers = 4)
        {
            try
            {
                // Ensure services are initialized first
                await EnsureInitialized();

                Debug.Log($"Creating relay allocation for {maxPlayers} players...");

                // Create allocation - this part works based on your logs
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

                if (allocation == null)
                {
                    Debug.LogError("Relay allocation is null");
                    return string.Empty;
                }

                Debug.Log($"Allocation created with ID: {allocation.AllocationId}");

                // THIS is likely where it's failing:
                string joinCode;
                try
                {
                    // Get the join code with explicit error handling
                    joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                    Debug.Log($"Join code received: {joinCode}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to get join code: {e.Message}");
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(joinCode))
                {
                    Debug.LogError("Join code is empty");
                    return string.Empty;
                }

                // Set up the Netcode transport with more error checking
                if (NetworkManager.Singleton == null)
                {
                    Debug.LogError("NetworkManager.Singleton is null");
                    return string.Empty;
                }

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport == null)
                {
                    Debug.LogError("UnityTransport component not found on NetworkManager");
                    return string.Empty;
                }

                try
                {
                    var relayServerData = new RelayServerData(allocation, "dtls");
                    transport.SetRelayServerData(relayServerData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to set relay server data: {e.Message}");
                    return string.Empty;
                }

                // Successfully set up relay
                RelayJoinCode = joinCode;
                IsRelaySessionActive = true;

                return joinCode;
            }
            catch (Exception e)
            {
                Debug.LogError($"Relay allocation failed: {e.Message}\nStack Trace: {e.StackTrace}");
                return string.Empty;
            }
        }

        // For clients to join using a join code
        public async Task<bool> JoinRelaySession(string joinCode)
        {
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Cannot join relay with empty join code");
                return false;
            }

            try
            {
                // Ensure services are initialized first
                await EnsureInitialized();

                Debug.Log($"Joining Relay with code: {joinCode}");
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                if (joinAllocation == null)
                {
                    Debug.LogError("Join allocation returned null");
                    return false;
                }

                // Set up the Netcode transport with the relay parameters
                if (!NetworkManager.Singleton.TryGetComponent<UnityTransport>(out var transport))
                {
                    Debug.LogError("UnityTransport component not found on NetworkManager");
                    return false;
                }

                var relayServerData = new RelayServerData(joinAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                RelayJoinCode = joinCode;
                IsRelaySessionActive = true;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Relay join failed: {e.Message}\nStack Trace: {e.StackTrace}");
                return false;
            }
        }
    }
}