using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;

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
        [SerializeField] private GameObject connectingStatePanel;
        [SerializeField] private GameObject inGameStatePanel;
        [SerializeField] private GameObject disconnectedStatePanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button steamProfileButton;
        [SerializeField] private Button kickButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button optionsButton;
        [SerializeField] private GameObject teamPanel;
        [SerializeField] private Selector teamSelector;
        [SerializeField] private Button redTeamButton;
        [SerializeField] private Button blueTeamButton;
        [SerializeField] private Button greenTeamButton;
        [SerializeField] private Button yellowTeamButton;

        protected Callback<AvatarImageLoaded_t> avatarImageLoadedCallback;

        public Player Player { get; private set; } = null;
        private bool isLocalPlayer = false;
        private bool isLocalHost = false;

        private void OnEnable()
        {
            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            redTeamButton.onClick.AddListener(() => ChangeTeam(Team.Red));
            blueTeamButton.onClick.AddListener(() => ChangeTeam(Team.Blue));
            greenTeamButton.onClick.AddListener(() => ChangeTeam(Team.Green));
            yellowTeamButton.onClick.AddListener(() => ChangeTeam(Team.Yellow));
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }

        private void OnDisable()
        {
            changeTeamButton.onClick.RemoveListener(OnChangeTeamButtonClicked);
            redTeamButton.onClick.RemoveListener(() => ChangeTeam(Team.Red));
            blueTeamButton.onClick.RemoveListener(() => ChangeTeam(Team.Blue));
            greenTeamButton.onClick.RemoveListener(() => ChangeTeam(Team.Green));
            yellowTeamButton.onClick.RemoveListener(() => ChangeTeam(Team.Yellow));
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
            optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        }

        public void SetPlayer(Player player)
        {
            Player = player;

            SetTeam((Team)int.Parse(Player.Data["Team"].Value));
            SetStatus((PlayerStatus)int.Parse(player.Data["Status"].Value));
            SetButtons();
            SetSteamInfo();
        }

        public void SetEmpty()
        {
            Player = null;

            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
        }

        public void SetTeam(Team team)
        {
            teamIndicatorImage.color = TeamColors.GetColor(team);

            ColorBlock colors = changeTeamButton.colors;
            colors.normalColor = TeamColors.GetColor(team);
            colors.highlightedColor = TeamColors.GetHoverColor(team);
            colors.pressedColor = TeamColors.GetHoverColor(team);
            colors.selectedColor = TeamColors.GetHoverColor(team);
            changeTeamButton.colors = colors;

            teamSelector.SetSelection((int)team, changeTeamButton.colors.normalColor, UIColors.primaryDisabledColor);
        }

        public void SetStatus(PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Ready:
                    nameText.color = UIColors.greenDefaultColor;
                    break;
                case PlayerStatus.NotReady:
                    nameText.color = UIColors.redDefaultColor;
                    break;
                case PlayerStatus.InGame:
                    nameText.color = UIColors.yellowDefaultColor;
                    inGameStatePanel.SetActive(true);
                    break;
            }
        }

        public void SetButtons()
        {
            isLocalPlayer = AuthenticationManager.Instance.LocalPlayer.Id == Player.Id;
            isLocalHost = AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId;

            changeTeamButton.gameObject.SetActive(isLocalPlayer || isLocalHost);
            teamIndicatorImage.gameObject.SetActive(!isLocalPlayer && !isLocalHost);
            optionsButton.gameObject.SetActive(!isLocalPlayer);
        }

        public void SetConnecting()
        {
            connectingStatePanel.SetActive(true);
        }

        public void SetConnected()
        {
            connectingStatePanel.SetActive(false);
        }

        public void SetDisconnected()
        {
            disconnectedStatePanel.SetActive(true);
        }

        private void SetSteamInfo()
        {
            CSteamID steamId = new(ulong.Parse(Player.Data["Id"].Value));
            nameText.text = "Loading..." + (Player.Id == LobbyManager.Instance.Lobby.HostId ? " (Host)" : "");
            if (steamId == SteamUser.GetSteamID())
            {
                nameText.text = SteamFriends.GetPersonaName() + (Player.Id == LobbyManager.Instance.Lobby.HostId ? " (Host)" : "");
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(SteamFriends.GetLargeFriendAvatar(steamId));
            }
            else
            {
                bool nameRequested = SteamFriends.RequestUserInformation(steamId, false);
                if (nameRequested) avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
                else
                {
                    nameText.text = SteamFriends.GetFriendPersonaName(steamId) + (Player.Id == LobbyManager.Instance.Lobby.HostId ? " (Host)" : "");
                    int imageHandle = SteamFriends.GetLargeFriendAvatar(steamId);
                    if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
                }
            }
        }

        private void OnChangeTeamButtonClicked()
        {
            teamPanel.SetActive(true);
        }

        private async void ChangeTeam(Team team)
        {
            PlayerData playerData = new() { Id = (CSteamID)ulong.Parse(Player.Data["Id"].Value), Team = team, Status = (PlayerStatus)int.Parse(Player.Data["Status"].Value) };
            await LobbyManager.Instance.UpdatePlayerData(Player.Id, playerData.Serialize());

            teamPanel.SetActive(false);
        }

        private void OnOptionsButtonClicked()
        {
            optionsButton.gameObject.SetActive(false);
            kickButton.gameObject.SetActive(!isLocalPlayer && isLocalHost);
            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.35f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(!isLocalPlayer && isLocalHost ? 0.7f : 1f, 1f);

            optionsPanel.SetActive(true);
        }

        private void OnBackButtonClicked()
        {
            optionsPanel.SetActive(false);

            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.15f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, 1f);
            optionsButton.gameObject.SetActive(true);

        }

        private void OnSteamProfileButtonClicked()
        {
            Application.OpenURL("https://steamcommunity.com/profiles/" + Player.Data["Id"].Value.ToString());
        }

        private async void OnKickButtonClicked()
        {
            await LobbyManager.Instance.KickPlayer(Player);
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (Player.Data.TryGetValue("Id", out PlayerDataObject idObject) &&
                ulong.TryParse(idObject.Value, out ulong playerId) &&
                callback.m_steamID.m_SteamID == playerId)
            {
                int imageHandle = callback.m_iImage;
                if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);

                string updatedName = SteamFriends.GetFriendPersonaName(callback.m_steamID);
                if (updatedName != "[unknown]" && nameText.text.StartsWith("Loading"))
                {
                    bool isHost = LobbyManager.Instance.Lobby.HostId == Player.Id;
                    nameText.text = updatedName + (isHost ? " (Host)" : "");
                }
            }
        }
    }
}