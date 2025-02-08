using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyListManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyItemPrefab;
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button backButton;

    private NetworkManager networkManager;
    private List<LobbyData> availableLobbies = new List<LobbyData>();
    public static LobbyListManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        refreshButton.onClick.AddListener(RefreshLobbyList);
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        LoadLobbyList();
    }

    private void LoadLobbyList()
    {
        availableLobbies.Clear();
        PopulateLobbyList();
    }

    public void AddLobby(LobbyData newLobby)
    {
        availableLobbies.Add(newLobby);
        PopulateLobbyList();
    }

    private void PopulateLobbyList()
    {
        foreach (Transform child in lobbyListContainer) Destroy(child.gameObject);

        foreach (var lobby in availableLobbies)
        {
            GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbyListContainer);
            LobbyListEntry lobbyListEntry = lobbyItem.GetComponent<LobbyListEntry>();
            lobbyListEntry.SetLobbyData(lobby);
        }
    }


    private void RefreshLobbyList() => LoadLobbyList();
}
