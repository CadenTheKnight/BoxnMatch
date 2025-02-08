using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyCreationMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private GameObject lobbyCreationPanel;
    [SerializeField] private TMP_InputField lobbyNameInput;

    public void Start()
    {
        backButton.onClick.AddListener(() => lobbyCreationPanel.SetActive(false));
        createLobbyButton.onClick.AddListener(OnCreateLobby);
    }

    public void OpenLobbyCreationMenu()
    {
        lobbyCreationPanel.SetActive(true);
    }

    public void CloseLobbyCreationMenu()
    {
        lobbyCreationPanel.SetActive(false);
    }

    private void OnCreateLobby()
    {
        string lobbyName = lobbyNameInput.text;

        if (string.IsNullOrWhiteSpace(lobbyName))
        {
            Debug.Log("Invalid lobby name");
            return;
        }

        // Hardcoded values for now
        int playerCount = 1;
        int capacity = 4;
        string hostName = PlayerPrefs.GetString("PlayerName");
        string status = "In Lobby";

        LobbyData newLobby = new LobbyData(lobbyName, playerCount, capacity, hostName, status);
        if (LobbyListManager.Instance != null)
        {
            LobbyListManager.Instance.AddLobby(newLobby);
        }
        else
        {
            Debug.LogError("LobbyListManager.Instance is null!");
        }

        CloseLobbyCreationMenu();
        SceneManager.LoadScene("Lobby");
    }
}
