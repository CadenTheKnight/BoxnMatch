using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    /// <summary>
    /// Represents a player list entry in the lobby UI.
    /// </summary>
    public class PlayerListEntry : MonoBehaviour
    {

        [Header("UI Components")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private GameObject activeStatePanel;
        [SerializeField] private RawImage profilePictureRawImage;
        [SerializeField] private Button changeTeamButton;
        [SerializeField] private Image teamIndicatorImage;
        [SerializeField] private GameObject inGameStatePanel;
        [SerializeField] private GameObject disconnectedStatePanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button steamProfileButton;
        [SerializeField] private Button kickButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button optionsButton;

        protected Callback<AvatarImageLoaded_t> avatarImageLoadedCallback;

        private Player player;
        private bool isLocalPlayer = false;
        private bool isLocalHost = false;
        private bool optionsOpen = false;

        private void OnEnable()
        {
            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }

        private void OnDisable()
        {
            changeTeamButton.onClick.RemoveListener(OnChangeTeamButtonClicked);
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
            optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        }

        public void SetEmpty()
        {
            player = null;

            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
        }

        public void SetPlayer(Player player)
        {
            this.player = player;

            isLocalPlayer = AuthenticationManager.Instance.LocalPlayer.Id == player.Id;
            isLocalHost = AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);

            switch (player.Data["Status"].Value)
            {
                case string status when status == PlayerStatus.Ready.ToString():
                    nameText.color = UIColors.greenDefaultColor;
                    break;
                case string status when status == PlayerStatus.NotReady.ToString():
                    nameText.color = UIColors.redDefaultColor;
                    break;
                case string status when status == PlayerStatus.InGame.ToString():
                    nameText.color = UIColors.yellowDefaultColor;
                    inGameStatePanel.SetActive(true);
                    break;
                case string status when status == PlayerStatus.Disconnected.ToString():
                    disconnectedStatePanel.SetActive(true);
                    break;
            }

            nameText.text = SteamFriends.GetFriendPersonaName((CSteamID)ulong.Parse(player.Data["Id"].Value)) + (player.Id == LobbyManager.Instance.Lobby.HostId ? " (Host)" : "");

            UpdateTeamColors(int.Parse(player.Data["Team"].Value));

            changeTeamButton.gameObject.SetActive(isLocalPlayer || isLocalHost);
            teamIndicatorImage.gameObject.SetActive(!isLocalPlayer && !isLocalHost);
            optionsButton.gameObject.SetActive(!isLocalPlayer && !optionsOpen);

            avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            int imageHandle = SteamFriends.GetLargeFriendAvatar((CSteamID)ulong.Parse(player.Data["Id"].Value));
            if (imageHandle == -1)
                SteamFriends.RequestUserInformation((CSteamID)ulong.Parse(player.Data["Id"].Value), false);
            else
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (player != null && player.Data.TryGetValue("Id", out PlayerDataObject idObject) &&
                ulong.TryParse(idObject.Value, out ulong playerId) &&
                callback.m_steamID.m_SteamID == playerId)
            {
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(callback.m_iImage);
                string currentName = nameText.text;
                if (currentName.Contains("[unknown]") || currentName == "Unknown Player")
                {
                    string updatedName = SteamFriends.GetFriendPersonaName(callback.m_steamID);
                    if (updatedName != "[unknown]")
                    {
                        bool isHost = LobbyManager.Instance.Lobby.HostId == player.Id;
                        nameText.text = updatedName + (isHost ? " (Host)" : "");
                    }
                }
            }
        }

        private void UpdateTeamColors(int team)
        {
            teamIndicatorImage.color = team == 0 ? UIColors.redDefaultColor : team == 1 ? UIColors.blueDefaultColor
                : team == 2 ? UIColors.greenDefaultColor : UIColors.yellowDefaultColor;

            ColorBlock colors = changeTeamButton.colors;

            colors.normalColor = team == 0 ? UIColors.redDefaultColor : team == 1 ? UIColors.blueDefaultColor
                : team == 2 ? UIColors.greenDefaultColor : UIColors.yellowDefaultColor;

            colors.highlightedColor = team == 0 ? UIColors.redHoverColor : team == 1 ? UIColors.blueHoverColor
                : team == 2 ? UIColors.greenHoverColor : UIColors.yellowHoverColor;

            colors.pressedColor = colors.highlightedColor;
            colors.selectedColor = colors.highlightedColor;

            changeTeamButton.colors = colors;
        }

        private void OnChangeTeamButtonClicked()
        {
            Debug.Log("Change team button clicked for player: " + player.Data["Id"].Value.ToString());
        }

        private void OnOptionsButtonClicked()
        {
            optionsOpen = true;
            optionsButton.gameObject.SetActive(false);

            kickButton.gameObject.SetActive(!isLocalPlayer && isLocalHost);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(!isLocalPlayer && isLocalHost ? 0.5f : 0.8f, 1f);
            steamProfileButton.GetComponent<RectTransform>().anchorMin = new Vector2(!isLocalPlayer && isLocalHost ? 0.5f : 0.8f, 0f);
            steamProfileButton.GetComponent<RectTransform>().anchorMax = new Vector2(!isLocalPlayer && isLocalHost ? 0.7f : 1f, 1f);

            optionsPanel.SetActive(true);
        }


        private void OnBackButtonClicked()
        {
            optionsOpen = false;
            optionsPanel.SetActive(false);

            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, 1f);

            optionsButton.gameObject.SetActive(true);
        }

        private void OnSteamProfileButtonClicked()
        {
            Application.OpenURL("https://steamcommunity.com/profiles/" + player.Data["Id"].Value.ToString());
        }

        private void OnKickButtonClicked()
        {
            GameLobbyManager.Instance.KickPlayer(player.Id);

            SetEmpty();
            player = null;
        }
    }
}