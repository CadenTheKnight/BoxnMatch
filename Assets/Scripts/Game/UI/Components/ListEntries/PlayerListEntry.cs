using TMPro;
using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
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
        [SerializeField] private LoadingBar kickLoadingBar;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button optionsButton;
        [SerializeField] private GameObject teamPanel;
        [SerializeField] private Selector playerTeamSelector;
        [SerializeField] private LoadingBar playerTeamSelectorLoadingBar;

        protected Callback<AvatarImageLoaded_t> avatarImageLoadedCallback;

        public Player Player { get; private set; } = null;

        private void OnEnable()
        {
            playerTeamSelector.onSelectionChanged += (index) =>
            {
                Team team = (Team)index;
                ChangeTeam(team);
            };

            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }

        private void OnDisable()
        {
            playerTeamSelector.onSelectionChanged -= (index) =>
            {
                Team team = (Team)index;
                ChangeTeam(team);
            };

            changeTeamButton.onClick.RemoveListener(OnChangeTeamButtonClicked);
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
            optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);

            playerTeamSelectorLoadingBar.StopLoading();
            kickLoadingBar.StopLoading();
        }

        public void SetPlayer(Player player)
        {
            Player = player;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);

            SetTeam(Enum.Parse<Team>(Player.Data["Team"].Value));
            SetStatus(Enum.Parse<PlayerStatus>(Player.Data["Status"].Value));
            SetButtons(player.Id == AuthenticationManager.Instance.LocalPlayer.Id,
                AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId);
            SetSteamInfo();
            SetHostName(player.Id == LobbyManager.Instance.Lobby.HostId);
        }

        public void SetEmpty()
        {
            Player = null;

            kickLoadingBar.StopLoading();
            playerTeamSelectorLoadingBar.StopLoading();

            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
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

        public void SetButtons(bool isLocalPlayer, bool isHost)
        {
            changeTeamButton.gameObject.SetActive((isLocalPlayer || isHost) && Enum.Parse<PlayerStatus>(Player.Data["Status"].Value) == PlayerStatus.NotReady);
            teamIndicatorImage.gameObject.SetActive((!isLocalPlayer && !isHost) || Enum.Parse<PlayerStatus>(Player.Data["Status"].Value) != PlayerStatus.NotReady);
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
            nameText.text = "Loading...";
            if (steamId == SteamUser.GetSteamID())
            {
                nameText.text = SteamFriends.GetPersonaName();
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(SteamFriends.GetLargeFriendAvatar(steamId));
            }
            else
            {
                bool nameRequested = SteamFriends.RequestUserInformation(steamId, false);
                if (nameRequested) avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
                else
                {
                    nameText.text = SteamFriends.GetFriendPersonaName(steamId);
                    int imageHandle = SteamFriends.GetLargeFriendAvatar(steamId);
                    if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
                }
            }
        }

        public void SetHostName(bool isHost)
        {
            nameText.text += isHost ? " (Host)" : "";
        }

        private void OnChangeTeamButtonClicked()
        {
            teamPanel.SetActive(true);
        }

        private async void ChangeTeam(Team team)
        {
            playerTeamSelector.UpdateInteractable(false);
            playerTeamSelectorLoadingBar.StartLoading();

            await GameLobbyManager.Instance.ChangePlayerTeam(Player, team);
        }

        public void SetTeam(Team team)
        {
            playerTeamSelector.SetSelection((int)team, true);

            ColorBlock colors = changeTeamButton.colors;
            colors.normalColor = TeamColors.GetColor(team);
            colors.highlightedColor = TeamColors.GetHoverColor(team);
            colors.pressedColor = TeamColors.GetHoverColor(team);
            colors.selectedColor = TeamColors.GetHoverColor(team);
            changeTeamButton.colors = colors;

            teamIndicatorImage.color = TeamColors.GetColor(team);

            playerTeamSelectorLoadingBar.StopLoading();
            playerTeamSelector.UpdateInteractable(true);
            teamPanel.SetActive(false);
        }

        private void OnOptionsButtonClicked()
        {
            bool kickablePlayer = Player.Id != AuthenticationManager.Instance.LocalPlayer.Id &&
                AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId;

            optionsButton.gameObject.SetActive(false);
            kickButton.gameObject.SetActive(kickablePlayer);
            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.35f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(kickablePlayer ? 0.7f : 1f, 1f);

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
            kickButton.interactable = false;
            kickLoadingBar.StartLoading();

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
                    nameText.text = updatedName;
            }
        }
    }
}