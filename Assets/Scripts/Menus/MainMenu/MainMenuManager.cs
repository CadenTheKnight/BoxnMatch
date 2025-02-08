using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button submitButton; // temp until steam integration
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject inputPanel; // temp until steam integration
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private Button clearPlayerRefButton; // temp until steam integration
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI playerNameDisplay;
    [SerializeField] private LobbyCreationMenu lobbyCreationMenu;

    private void Start()
    {
        quitButton.onClick.AddListener(OnQuit);
        settingsButton.onClick.AddListener(OnSettings);
        submitButton.onClick.AddListener(OnSubmitName);
        joinLobbyButton.onClick.AddListener(OnJoinLobby);
        createLobbyButton.onClick.AddListener(OnCreateLobby);
        clearPlayerRefButton.onClick.AddListener(OnClearPlayerRef);

        inputPanel.SetActive(!PlayerPrefs.HasKey("PlayerName"));
        playerPanel.SetActive(PlayerPrefs.HasKey("PlayerName"));
        playerNameDisplay.text = PlayerPrefs.GetString("PlayerName") ?? "";
    }

    private void OnJoinLobby()
    {
        SceneManager.LoadScene("LobbyList");
    }

    private void OnCreateLobby()
    {
        lobbyCreationMenu.OpenLobbyCreationMenu();
    }

    private void OnSettings()
    {
        settingsMenu.OpenSettingsMenu();
    }

    private void OnQuit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    private void OnClearPlayerRef()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        playerNameDisplay.text = "";
        inputPanel.SetActive(true);
        playerPanel.SetActive(false);
    }

    private void OnSubmitName()
    {
        if (string.IsNullOrWhiteSpace(playerNameInput.text)) return;
        PlayerPrefs.SetString("PlayerName", playerNameInput.text);
        PlayerPrefs.Save();
        playerNameDisplay.text = playerNameInput.text;
        inputPanel.SetActive(false);
        playerPanel.SetActive(true);
    }
}
