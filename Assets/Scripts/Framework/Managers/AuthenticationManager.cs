using System;
using Steamworks;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Utilities;

// may want to add timeouts for steam and unity services

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
        /// Initializes Unity Services.
        /// </summary>
        /// <returns>An OperationResult indicating the success or failure of the operation.</returns>
        public async Task<OperationResult> InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();

                return OperationResult.SuccessResult("InitializeUnityServices", "Unity Services initialized successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.ErrorResult("InitializeUnityServicesError", ex.Message);
            }
        }

        /// <summary>
        /// Initializes Steam API.
        /// </summary>
        /// <returns>An OperationResult indicating the success or failure of the operation.</returns>
        public async Task<OperationResult> InitializeSteam()
        {
            try
            {
                while (!SteamManager.Initialized) await Task.Delay(100);

                return OperationResult.SuccessResult("InitializeSteam", "Steam initialized successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.ErrorResult("InitializeSteamError", ex.Message);
            }
        }

        /// <summary>
        /// Authenticates the player with Unity Services using Steam.
        /// </summary>
        /// <returns>An OperationResult indicating the success or failure of the operation.</returns>
        public async Task<OperationResult> Authenticate()
        {
            try
            {
                SignInWithSteam();
                while (!AuthenticationService.Instance.IsSignedIn) await Task.Delay(100);

                PlayerData playerData = new() { Id = SteamUser.GetSteamID() };
                LocalPlayer = new Player(id: AuthenticationService.Instance.PlayerId, data: playerData.Serialize());

                return OperationResult.SuccessResult("Authenticated", $"Signed in as {SteamFriends.GetPersonaName()}");
            }
            catch (Exception ex)
            {
                return OperationResult.ErrorResult("AuthenticationError", ex.Message);
            }
        }
    }
}