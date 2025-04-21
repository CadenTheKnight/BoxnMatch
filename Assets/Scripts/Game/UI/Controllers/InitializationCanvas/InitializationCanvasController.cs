// using System;
// using Steamworks;
// using UnityEngine;
// using Unity.Services.Core;
// using System.Threading.Tasks;
// using Unity.Services.Authentication;
// using Assets.Scripts.Framework.Events;
// using Assets.Scripts.Framework.Managers;
// using Assets.Scripts.Game.UI.Components;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.UI.Controllers.InitializationCanvas
// {
//     /// <summary>
//     /// Handles the logic for the initialization scene.
//     /// </summary>
//     public class InitializationCanvasController : MonoBehaviour
//     {
//         [Header("UI Components")]
//         [SerializeField] private LoadingStatus loadingStatusPanel;

//         Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
//         string m_SessionTicket;

//         private void SignInWithSteam()
//         {
//             m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);
//             SteamUser.GetAuthTicketForWebApi("unityauthenticationservice");
//         }

//         private async void OnAuthCallback(GetTicketForWebApiResponse_t callback)
//         {
//             m_SessionTicket = BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);
//             m_AuthTicketForWebApiResponseCallback.Dispose();
//             m_AuthTicketForWebApiResponseCallback = null;

//             await AuthenticationService.Instance.SignInWithSteamAsync(m_SessionTicket, "unityauthenticationservice");
//         }

//         public async void Start()
//         {
//             if (await InitializeUnityServices())
//                 if (await InitializeSteam())
//                     if (await Authenticate())
//                         await AttemptRejoin();
//         }

//         /// <summary>
//         /// Initializes Unity Servicess.
//         /// </summary>
//         /// <returns>True if unity services initialization was successful, false otherwise.</returns>
//         private async Task<bool> InitializeUnityServices()
//         {
//             loadingStatusPanel.StartLoading();
//             loadingStatusPanel.UpdateStatus("Initializing Unity Services...");

//             try
//             {
//                 await UnityServices.InitializeAsync();
//                 loadingStatusPanel.UpdateStatus("Unity Services initialized");
//                 return true;
//             }
//             catch (Exception ex)
//             {
//                 loadingStatusPanel.UpdateStatus("Initialization failed");
//                 loadingStatusPanel.StopLoading();
//                 AuthEvents.InvokeInitializationError(OperationResult.ErrorResult("InitializeUnityServicesError", ex.Message, retry: async () => await InitializeUnityServices()));
//                 return false;
//             }
//         }

//         /// <summary>
//         /// Initializes Steam.
//         /// </summary>
//         /// <returns>True if steam initialization was successful, false otherwise.</returns>
//         private async Task<bool> InitializeSteam()
//         {
//             loadingStatusPanel.StartLoading();
//             loadingStatusPanel.UpdateStatus("Initializing Steam...");

//             float startTime = Time.realtimeSinceStartup;
//             while (!SteamManager.Initialized)
//             {
//                 if (Time.realtimeSinceStartup - startTime > 5f)
//                 {
//                     loadingStatusPanel.UpdateStatus("Steam initialization failed");
//                     loadingStatusPanel.StopLoading();
//                     AuthEvents.InvokeInitializationError(OperationResult.ErrorResult("InitializeSteamError", "Steam initialization failed", retry: async () => await InitializeSteam()));
//                     return false;
//                 }
//                 await Task.Delay(1000);
//             }

//             loadingStatusPanel.UpdateStatus("Steam initialized");
//             return true;
//         }

//         /// <summary>
//         /// Authenticates the user.
//         /// </summary>
//         /// <returns>True if authentication was successful, false otherwise.</returns>
//         private async Task<bool> Authenticate()
//         {
//             loadingStatusPanel.StartLoading();
//             loadingStatusPanel.UpdateStatus("Authenticating...");

//             try
//             {
//                 SignInWithSteam();

//                 float startTime = Time.realtimeSinceStartup;
//                 while (!AuthenticationService.Instance.IsSignedIn)
//                 {
//                     if (Time.realtimeSinceStartup - startTime > 10f)
//                     {
//                         loadingStatusPanel.UpdateStatus("Authentication failed");
//                         loadingStatusPanel.StopLoading();
//                         AuthEvents.InvokeInitializationError(OperationResult.ErrorResult("AuthenticationTimeout", "Authentication timed out", retry: async () => await Authenticate()));
//                         return false;
//                     }
//                     await Task.Delay(100);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 loadingStatusPanel.UpdateStatus("Authentication failed");
//                 loadingStatusPanel.StopLoading();
//                 AuthEvents.InvokeInitializationError(OperationResult.ErrorResult("AuthenticationError", ex.Message, retry: async () => await Authenticate()));
//                 return false;
//             }

//             loadingStatusPanel.UpdateStatus("Authenticated");
//             return true;
//         }

//         /// <summary>
//         /// Attempts to rejoin the first joined lobby if any are found.
//         /// </summary>
//         private async Task AttemptRejoin()
//         {
//             loadingStatusPanel.UpdateStatus("Checking for joined lobbies...");

//             await LobbyManager.RejoinLobby();
//         }
//     }
// }