using System;
using Steamworks;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services through steam.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        /// <summary>
        /// The local player as a unity player object.
        /// </summary>
        public Player LocalPlayer { get; private set; } = null;

        Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
        string m_SessionTicket;

        void SignInWithSteam()
        {
            m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);
            SteamUser.GetAuthTicketForWebApi("unityauthenticationservice");
        }

        async void OnAuthCallback(GetTicketForWebApiResponse_t callback)
        {
            m_SessionTicket = BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);
            m_AuthTicketForWebApiResponseCallback.Dispose();
            m_AuthTicketForWebApiResponseCallback = null;

            await AuthenticationService.Instance.SignInWithSteamAsync(m_SessionTicket, "unityauthenticationservice");
        }

        /// <summary>
        /// Initializes Unity Services and signs in the player through steam.
        /// </summary>
        /// <returns>An OperationResult indicating the success or failure of the operation.</returns>
        public async Task<OperationResult> InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();

                while (!SteamManager.Initialized) await Task.Delay(100);

                SignInWithSteam();

                while (!AuthenticationService.Instance.IsSignedIn) await Task.Delay(100);

                PlayerData playerData = new();
                playerData.Initialize(SteamUser.GetSteamID());
                LocalPlayer = new Player(id: AuthenticationService.Instance.PlayerId, data: playerData.Serialize());

                return OperationResult.SuccessResult("Initialize", $"Signed in as {SteamFriends.GetPersonaName()}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"AuthenticationManager: {ex.Message}");
                return OperationResult.ErrorResult("InitializeError", ex.Message);
            }
        }
    }
}