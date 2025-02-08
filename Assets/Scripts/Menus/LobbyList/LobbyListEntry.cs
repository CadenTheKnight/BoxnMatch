using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyListEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI hostNameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button joinButton;

    private LobbyData lobbyData;

    public void SetLobbyData(LobbyData data)
    {
        lobbyData = data;
        lobbyNameText.text = data.LobbyName;
        playerCountText.text = $"{data.Players}/{data.Capacity}";
        hostNameText.text = data.HostName;
        statusText.text = data.Status;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(OnJoinLobby);
    }

    private void OnJoinLobby()
    {
        Debug.Log($"Joining {lobbyData.LobbyName} hosted by {lobbyData.HostName}");
        // Call Fish-Net API here to join the lobby
    }
}
