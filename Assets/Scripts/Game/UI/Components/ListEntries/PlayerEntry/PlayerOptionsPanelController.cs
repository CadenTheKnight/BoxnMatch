using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Game.Managers;

namespace Assets.Scripts.Game.UI.Components.ListEntries.PlayerEntry
{
    /// <summary>
    /// Handles the options for a player in the player list.
    /// </summary>
    public class PlayerOptionsPanelController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button steamProfileButton;
        [SerializeField] private Button kickButton;

        private CSteamID steamId = CSteamID.Nil;
        private string playerId = string.Empty;
        private bool kickablePlayer = false;

        public void SetPlayerOptions(CSteamID steamId, string playerId)
        {
            this.steamId = steamId;
            this.playerId = playerId;
            kickablePlayer = AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId &&
                AuthenticationManager.Instance.LocalPlayer.Id != playerId;

            nameText.text = SteamFriends.GetFriendPersonaName(steamId);
            SetPanelState(kickablePlayer);

            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
        }

        private void OnDestroy()
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
        }

        private void SetPanelState(bool kickablePlayer)
        {
            kickButton.gameObject.SetActive(kickablePlayer);

            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(kickablePlayer ? 0.5f : 0.8f, 1f);
            steamProfileButton.GetComponent<RectTransform>().anchorMin = new Vector2(kickablePlayer ? 0.5f : 0.8f, 0f);
            steamProfileButton.GetComponent<RectTransform>().anchorMax = new Vector2(kickablePlayer ? 0.7f : 1f, 1f);
        }

        private void OnBackButtonClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnSteamProfileButtonClicked()
        {
            Application.OpenURL("https://steamcommunity.com/profiles/" + steamId.ToString());
        }

        private void OnKickButtonClicked()
        {
            GameLobbyManager.Instance.KickPlayer(playerId);
        }
    }
}
