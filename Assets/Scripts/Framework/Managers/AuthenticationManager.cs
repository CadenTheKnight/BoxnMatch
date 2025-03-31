using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Framework.Core;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.Data;
using Unity.Services.Core;

namespace Assets.Scripts.Framework.Managers
{
    /// <summary>
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        /// <summary>
        /// The local player as a unity player object.
        /// </summary>
        public Player LocalPlayer { get; private set; }

        /// <summary>
        /// Generates a new player with a random name if one does not already exist in PlayerPrefs.
        /// </summary>
        private void GeneratePlayer()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
            {
                string randomName = $"BoxnTester{Random.Range(1000, 9999)}";
                PlayerPrefs.SetString("PlayerName", randomName);
                PlayerPrefs.Save();
            }

            PlayerData playerData = new();
            playerData.Initialize();

            LocalPlayer = new Player(id: AuthenticationService.Instance.PlayerId, data: playerData.Serialize(), profile: new PlayerProfile(PlayerPrefs.GetString("PlayerName")));
        }

        /// <summary>
        /// Initializes Unity Services and signs in the player anonymously.
        /// </summary>
        /// <returns>An OperationResult indicating the success or failure of the operation.</returns>
        public async Task<OperationResult> InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                GeneratePlayer();

                return OperationResult.SuccessResult("Initialize", $"Signed in as {LocalPlayer.Profile.Name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AuthenticationManager: {ex.Message}");
                return OperationResult.ErrorResult("InitializeError", ex.Message);
            }
        }
    }
}