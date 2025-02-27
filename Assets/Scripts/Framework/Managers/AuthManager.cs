using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthManager : Singleton<AuthManager>
    {
        /// <summary>
        /// Generates a random player name and saves it to PlayerPrefs.
        /// </summary>
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
        /// <returns>An OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> InitializeAsync()
        {
            try
            {
                await Task.Delay(1000); // Simulate loading

                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName"))) GenerateAndSaveRandomPlayerName();
                AuthEvents.InvokeAuthenticated();

                return OperationResult.SuccessResult("SignInSuccess", $"Signed in as: {PlayerPrefs.GetString("PlayerName")} | {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException authEx)
            {
                return OperationResult.FailureResult(authEx.ErrorCode.ToString(), authEx.Message);
            }
            catch (RequestFailedException reqEx)
            {
                return OperationResult.FailureResult(reqEx.ErrorCode.ToString(), reqEx.Message);
            }
            catch (System.Exception ex)
            {
                return OperationResult.FailureResult("UnexpectedError", ex.Message);
            }
        }
    }
}