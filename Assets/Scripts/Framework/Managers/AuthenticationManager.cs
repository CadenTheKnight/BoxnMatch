using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

// Still need to add steam integration

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
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
        /// <returns>OperationResult indicating success or failure.</returns>
        public async Task<OperationResult> InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName"))) GenerateAndSaveRandomPlayerName();

                AuthenticationEvents.InvokeOnAuthenticated(PlayerPrefs.GetString("PlayerName"));
                return OperationResult.SuccessResult("Authenticated", $"Signed in as: {PlayerPrefs.GetString("PlayerName")}");
            }
            catch (System.Exception authEx)
            {
                return OperationResult.ErrorResult("AuthenticationError", authEx.Message);
            }
        }
    }
}