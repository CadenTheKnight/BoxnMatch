using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Assets.Scripts.Game.UI.Controllers.LobbyMenu;
using TMPro;
using Assets.Scripts.Framework.Managers;
using System;

namespace Assets.Scripts.Game.Managers
{
    public class GameManager : MonoBehaviour  // Changed from NetworkBehaviour to MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        private float returnToLobbyTime = 5f;
        private float currentTime;
        private LobbyPanelController lobbyPanel;

        private bool isCountdownStarted = false;

        private void Start()
        {
            lobbyPanel = FindObjectOfType<LobbyPanelController>(true);
            if (lobbyPanel == null)
            {
                Debug.LogError("LobbyPanelController not found!");
            }

            countdownText.text = "Establishing connection...";

            // Start a routine to check network status
            StartCoroutine(CheckNetworkAndStartCountdown());
        }

        private IEnumerator CheckNetworkAndStartCountdown()
        {
            // Wait a short delay for network to initialize
            yield return new WaitForSeconds(0.5f);

            // Check if we're initialized yet
            if (isCountdownStarted) yield break;

            bool isHost = false;
            bool isClient = false;

            // Check if NetworkManager exists and get roles
            if (NetworkManager.Singleton != null)
            {
                isHost = NetworkManager.Singleton.IsHost;
                isClient = NetworkManager.Singleton.IsClient;

                Debug.Log($"Network check - IsHost: {isHost}, IsClient: {isClient}");
            }
            else
            {
                Debug.LogWarning("NetworkManager.Singleton is null! Assuming host role as fallback.");
                isHost = true; // Fallback assumption
            }

            // Start appropriate countdown
            isCountdownStarted = true;

            if (isHost)
            {
                Debug.Log("Starting host countdown");
                StartCoroutine(AutoReturnToLobbyAfterDelay());
            }
            else if (isClient)
            {
                Debug.Log("Starting client countdown");
                StartCoroutine(ShowCountdown());
            }
            else
            {
                // If we can't determine role, default to showing countdown
                Debug.LogWarning("Could not determine network role - defaulting to client countdown");
                StartCoroutine(ShowCountdown());
            }
        }

        private IEnumerator AutoReturnToLobbyAfterDelay()
        {
            currentTime = returnToLobbyTime;

            while (currentTime > 0)
            {
                countdownText.text = $"Returning to lobby in {Mathf.CeilToInt(currentTime)}...";
                yield return null;
                currentTime -= Time.deltaTime;
            }

            Debug.Log("Host countdown complete - initiating return to lobby");
            countdownText.text = "Returning to lobby...";

            // Since we're not using NetworkBehaviour anymore, manually notify clients
            if (NetworkManager.Singleton != null)
            {
                // Direct shutdown for simplicity
                NetworkManager.Singleton.Shutdown();
            }

            // Wait a moment for network shutdown
            yield return new WaitForSeconds(0.5f);

            // Return to lobby
            ReturnToLobby();
        }

        private IEnumerator ShowCountdown()
        {
            currentTime = returnToLobbyTime;

            while (currentTime > 0)
            {
                countdownText.text = $"Returning to lobby in {Mathf.CeilToInt(currentTime)}...";
                yield return null;
                currentTime -= Time.deltaTime;
            }

            countdownText.text = "Waiting for host...";

            // Wait up to 5 more seconds for host to return us
            float waitTime = 0;
            while (waitTime < 5)
            {
                yield return null;
                waitTime += Time.deltaTime;
            }

            // If we've waited too long, try to return ourselves
            Debug.LogWarning("Host didn't return us to lobby after waiting - attempting self-return");
            ReturnToLobby();
        }

        private void ReturnToLobby()
        {
            try
            {
                Debug.Log("Executing ReturnToLobby()");

                // Ensure NetworkManager is shut down if it exists
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.Shutdown();
                    Debug.Log("NetworkManager shut down");
                }

                // Return to lobby via the panel controller
                if (lobbyPanel != null)
                {
                    Debug.Log("Calling lobbyPanel.ReturnToLobby()");
                    lobbyPanel.ReturnToLobby();
                }
                else
                {
                    Debug.LogError("LobbyPanel is null, can't return to lobby!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error returning to lobby: {ex.Message}");
            }
        }
    }
}