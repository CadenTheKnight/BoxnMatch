using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Framework.Constants;

// Still need to add steam integration

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        [Header("Testing")]
        [SerializeField] private int simulateFailureAttempt = 1;
        [SerializeField] private bool simulateNetworkFailure = false;

        /// <summary>
        /// The number of attempts to authenticate. Used for testing.
        /// </summary>
        private int _attempt = 0;

        /// <summary>
        /// The player's ID.
        /// </summary>
        public string PlayerId => AuthenticationService.Instance.PlayerId;

        /// <summary>
        /// Generates a random player name and saves it to PlayerPrefs.
        /// </summary>
        /// <remarks>
        /// This is a temporary solution until Steam integration is added.
        /// </remarks>
        private void GenerateAndSaveRandomPlayerName()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
            {
                string randomName = $"BoxnTester{Random.Range(1000, 9999)}";
                PlayerPrefs.SetString("PlayerName", randomName);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Initializes Unity Services and signs in the player anonymously.
        /// </summary>
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> InitializeAsync(int maxRetries = 3)
        {
            try
            {
                _attempt++;
                var initializeTask = UnityServices.InitializeAsync();
                if (await Task.WhenAny(initializeTask, Task.Delay(15000)) != initializeTask)
                    return OperationResult.ErrorResult(ErrorCodes.Auth.INITIALIZATION_TIMEOUT,
                        "Unity Services initialization timed out", "Authentication");

                if (simulateNetworkFailure)
                    if (_attempt <= simulateFailureAttempt)
                        return OperationResult.ErrorResult(ErrorCodes.Auth.NETWORK_ERROR,
                            "No internet connection", "Authentication");

                if (Application.internetReachability == NetworkReachability.NotReachable)
                    return OperationResult.ErrorResult(ErrorCodes.Auth.NETWORK_ERROR,
                        "No internet connection", "Authentication");

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerId))
                    return OperationResult.ErrorResult(ErrorCodes.Auth.AUTHENTICATION_ERROR,
                        "Player ID is missing", "Authentication");

                if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
                    GenerateAndSaveRandomPlayerName();

                return await AuthenticationEvents.InvokeOnAuthenticated(PlayerPrefs.GetString("PlayerName"));
            }
            catch (RequestFailedException authEx)
            {
                string errorCode;
                string errorMessage;

                if (authEx.ErrorCode == 401)
                {
                    errorCode = ErrorCodes.Auth.INVALID_CREDENTIALS;
                    errorMessage = "Authentication failed: Invalid credentials";
                }
                else if (authEx.ErrorCode >= 500)
                {
                    errorCode = ErrorCodes.Auth.SERVER_ERROR;
                    errorMessage = "Authentication failed: Server error";
                }
                else if (authEx.ErrorCode == 429)
                {
                    errorCode = ErrorCodes.Auth.RATE_LIMIT_EXCEEDED;
                    errorMessage = "Authentication failed: Too many requests";
                }
                else
                {
                    errorCode = ErrorCodes.Auth.AUTHENTICATION_ERROR;
                    errorMessage = $"Authentication error ({authEx.ErrorCode}): {authEx.Message}";
                }

                return OperationResult.ErrorResult(errorCode, errorMessage, "Authentication");
            }
            catch (System.Exception ex)
            {
                return OperationResult.ErrorResult(ErrorCodes.UNKNOWN_ERROR, ex.Message, "Authentication");
            }
        }
    }
}