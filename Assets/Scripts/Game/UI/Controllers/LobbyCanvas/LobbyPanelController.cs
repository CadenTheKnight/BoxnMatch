using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class LobbyPanelController : MonoBehaviour
    {
        [Header("Header Components")]
        [SerializeField] private Button lobbyCodeButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private bool isCopied = false;

        private void Start()
        {
            lobbyNameText.text = $"{LobbyManager.Instance.Lobby.Name}" + (LobbyManager.Instance.Lobby.IsPrivate ? " (PRIVATE)" : "");
            lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
        }

        private void OnEnable()
        {
            lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyRefreshed += OnLobbyRefreshed;
        }

        private void OnDisable()
        {
            lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);

            LobbyEvents.OnLobbyRefreshed -= OnLobbyRefreshed;
        }

        private void OnLobbyRefreshed()
        {
            lobbyNameText.text = $"{LobbyManager.Instance.Lobby.Name}" + (LobbyManager.Instance.Lobby.IsPrivate ? " (PRIVATE)" : "");
            if (!isCopied) lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
        }

        private async void OnLobbyCodeClicked()
        {
            GUIUtility.systemCopyBuffer = LobbyManager.Instance.Lobby.LobbyCode;

            isCopied = true;
            lobbyCodeText.text = $"Copied!";
            lobbyCodeText.color = UIColors.greenDefaultColor;

            await Task.Delay(1000);

            lobbyCodeText.text = $"Code: {LobbyManager.Instance.Lobby.LobbyCode}";
            lobbyCodeText.color = Color.white;
            isCopied = false;
        }
    }
}