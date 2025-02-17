using UnityEngine;
using Unity.Services.Core;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;


namespace Assets.Scripts.Game
{
    public class Init : MonoBehaviour
    {
        [SerializeField] private LoadingBarAnimator loadingBar;
        [SerializeField] private ErrorPopup errorPopup;

        private async void Start()
        {
            loadingBar.StartLoading();
            try
            {
                await UnityServices.InitializeAsync();
                if (UnityServices.State == ServicesInitializationState.Initialized)
                {
                    Debug.Log("Unity Services Initialized");

                    await AuthenticationService.Instance.SignInAnonymouslyAsync(); // temporary until steam integration
                    if (AuthenticationService.Instance.IsSignedIn)
                    {
                        string username = PlayerPrefs.GetString(key: "username");
                        if (string.IsNullOrEmpty(username))
                        {
                            username = "Player" + Random.Range(1000, 9999);
                            PlayerPrefs.SetString(key: "username", username);
                        }

                        Debug.Log($"Signed in as {username} with playerId: {AuthenticationService.Instance.PlayerId}");
                        loadingBar.StopLoading();
                        SceneManager.LoadScene("MainMenu");
                    }
                    else
                    {
                        Debug.LogError("Failed to sign in");
                        loadingBar.StopLoading();
                        errorPopup.ShowError(10002, "Sign-in Error: Failed to sign in.");
                    }
                }
                else
                {
                    Debug.LogError("Unity Services Failed to Initialize");
                    loadingBar.StopLoading();
                    errorPopup.ShowError(20008, "Unity Error: Unity Services Failed to Initialize.");
                }
            }
            catch (AuthenticationException authEx)
            {
                Debug.LogError($"Authentication Error: {authEx.ErrorCode} - {authEx.Message}");
                loadingBar.StopLoading();
                errorPopup.ShowError(authEx.ErrorCode, $"Authentication Error: {authEx.ErrorCode} - {authEx.Message}");
            }
            catch (RequestFailedException reqEx)
            {
                Debug.LogError($"Request Failed: {reqEx.ErrorCode} - {reqEx.Message}");
                loadingBar.StopLoading();
                errorPopup.ShowError(reqEx.ErrorCode, $"Request Failed: {reqEx.ErrorCode} - {reqEx.Message}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Unexpected Error: {ex.Message}");
                loadingBar.StopLoading();
                errorPopup.ShowError(500, $"Unexpected Error: {ex.Message}");
            }
        }
    }
}
