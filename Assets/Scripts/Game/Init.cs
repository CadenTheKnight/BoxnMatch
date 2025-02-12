using UnityEngine;
using Unity.Services.Core;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

namespace Assets.Scripts.Game
{
    public class Init : MonoBehaviour
    {
        async void Start()
        {
            await UnityServices.InitializeAsync();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                Debug.Log("Unity Services Initialized");

                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // temporary until steam integration
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);

                    string username = PlayerPrefs.GetString(key: "username");
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "Player" + Random.Range(1000, 9999);
                        PlayerPrefs.SetString(key: "username", username);
                    }

                    SceneManager.LoadSceneAsync("MainMenu");
                }
                else
                {
                    Debug.Log("Failed to sign in");
                }
            }
            else
            {
                Debug.Log("Unity Services Failed to Initialize");
            }
        }
    }
}
