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
    /// Manages authentication with Unity Services.
    /// </summary>
    public class AuthenticationManager : Singleton<AuthenticationManager>
    {
        /// <summary>
        /// The local player as a unity player object.
        /// </summary>
        public Player LocalPlayer { get; private set; } = null;

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

                if (!PlayerPrefs.HasKey("Name"))
                {
                    if (LocalPlayer.Data["Name"].Value == null)
                        PlayerPrefs.SetString("Name", "BoxnPlayer" + Random.Range(1000, 9999).ToString());
                    else
                        PlayerPrefs.SetString("Name", LocalPlayer.Data["Name"].Value);
                    PlayerPrefs.Save();
                }

                PlayerData playerData = new();
                playerData.Initialize(PlayerPrefs.GetString("Name"));
                LocalPlayer = new Player(id: AuthenticationService.Instance.PlayerId, data: playerData.Serialize());

                return OperationResult.SuccessResult("Initialize", $"Signed in as {LocalPlayer.Data["Name"].Value}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AuthenticationManager: {ex.Message}");
                return OperationResult.ErrorResult("InitializeError", ex.Message);
            }
        }
    }
}