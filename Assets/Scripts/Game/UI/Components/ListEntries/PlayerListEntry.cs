using TMPro;
using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options.Selector;

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

        public string PlayerId { get; private set; } = null;

        private void OnEnable()
        {
            playerTeamSelector.onSelectionChanged += ChangeTeam;

            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }

        private void OnDisable()
        {
            playerTeamSelector.onSelectionChanged -= ChangeTeam;

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
            PlayerId = player.Id;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);

            SetTeam(Enum.Parse<Team>(player.Data["Team"].Value));
            SetStatus(Enum.Parse<PlayerStatus>(player.Data["Status"].Value));
            SetButtons(
                player.Id == AuthenticationManager.Instance.LocalPlayer.Id,
                AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId,
                Enum.Parse<PlayerStatus>(player.Data["Status"].Value) == PlayerStatus.NotReady
                );
            SetSteamInfo(ulong.Parse(player.Data["SteamId"].Value));
            SetHostName(player.Id == LobbyManager.Instance.Lobby.HostId);
        }

        public void SetEmpty()
        {
            PlayerId = null;

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
                    nameText.color = UIColors.Green.One;
                    break;
                case PlayerStatus.NotReady:
                    nameText.color = UIColors.Red.One;
                    break;
                case PlayerStatus.InGame:
                    nameText.color = UIColors.Orange.One;
                    inGameStatePanel.SetActive(true);
                    break;
            }
        }

        public void SetButtons(bool isLocalPlayer, bool isHost, bool isNotReady)
        {
            changeTeamButton.gameObject.SetActive((isLocalPlayer || isHost) && isNotReady);
            teamIndicatorImage.gameObject.SetActive((!isLocalPlayer && !isHost) || !isNotReady);
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

        private void SetSteamInfo(ulong Id)
        {
            CSteamID steamId = new(Id);
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

        private async void ChangeTeam(int team)
        {
            Player player = LobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId);

            if ((Team)team != Enum.Parse<Team>(player.Data["Team"].Value))
            {
                changeTeamButton.interactable = false;
                playerTeamSelector.UpdateInteractable(false);
                playerTeamSelectorLoadingBar.StartLoading();
                await GameLobbyManager.Instance.ChangePlayerTeam(player, (Team)team);
            }
            else teamPanel.SetActive(false);
        }

        public async void SetTeam(Team team)
        {
            playerTeamSelector.SetSelection((int)team, true);

            ColorBlock colors = changeTeamButton.colors;
            colors.normalColor = TeamColors.GetColor(team);
            colors.highlightedColor = TeamColors.GetHoverColor(team);
            colors.pressedColor = TeamColors.GetSelectedColor(team);
            colors.selectedColor = TeamColors.GetSelectedColor(team);
            colors.disabledColor = TeamColors.GetDisabledColor(team);
            changeTeamButton.colors = colors;

            teamIndicatorImage.color = TeamColors.GetColor(team);

            playerTeamSelectorLoadingBar.StopLoading();
            playerTeamSelector.UpdateInteractable(true);
            teamPanel.SetActive(false);

            await Task.Delay(1500);

            changeTeamButton.interactable = true;
        }

        private void OnOptionsButtonClicked()
        {
            bool kickablePlayer = PlayerId != AuthenticationManager.Instance.LocalPlayer.Id &&
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
            Application.OpenURL("https://steamcommunity.com/profiles/" + LobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId).Data["SteamId"].Value.ToString());
        }

        private async void OnKickButtonClicked()
        {
            kickButton.interactable = false;
            kickLoadingBar.StartLoading();

            await LobbyManager.Instance.KickPlayer(LobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId));
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            int imageHandle = callback.m_iImage;
            if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
            nameText.text = SteamFriends.GetFriendPersonaName(callback.m_steamID);
        }
    }
}