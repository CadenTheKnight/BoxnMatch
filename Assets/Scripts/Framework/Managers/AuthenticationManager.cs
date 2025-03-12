using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Utilities;

// Todo
// 1. Steam integration
// 2. ?

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        /// <summary>
        /// Event that is invoked when the player is authenticated.
        /// </summary>
        public delegate void AuthenticatedHandler();
        public static event AuthenticatedHandler OnAuthenticated;

        /// <summary>
        /// The player's ID.
        /// </summary>
        public string PlayerId => AuthenticationService.Instance.PlayerId;

        /// <summary>
        /// Generates a random player name and saves it to PlayerPrefs.
        /// </summary>
        /// <remarks>
        /// This method is temporary until steam integration is added.
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
        /// <returns>An OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName"))) GenerateAndSaveRandomPlayerName();

                OnAuthenticated?.Invoke();
                return OperationResult.SuccessResult("Authenticated", $"Signed in as: {PlayerPrefs.GetString("PlayerName")}");
            }
            catch (System.Exception authEx)
            {
                return OperationResult.FailureResult("AuthenticationError", authEx.Message);
            }
        }
    }
}