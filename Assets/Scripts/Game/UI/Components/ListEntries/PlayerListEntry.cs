using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Unity.Services.Authentication;
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
        [SerializeField] private Image hostIndicatorImage;
        [SerializeField] private Button optionsButton;
        [SerializeField] private GameObject teamPanel;
        [SerializeField] private Selector playerTeamSelector;
        [SerializeField] private LoadingBar playerTeamSelectorLoadingBar;

        protected Callback<AvatarImageLoaded_t> avatarImageLoadedCallback;

        public string PlayerId { get; private set; } = null;

        private void OnEnable()
        {
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            playerTeamSelector.onSelectionChanged += ChangeTeam;
        }

        private void OnDisable()
        {
            optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
            changeTeamButton.onClick.RemoveListener(OnChangeTeamButtonClicked);
            playerTeamSelector.onSelectionChanged -= ChangeTeam;

            playerTeamSelectorLoadingBar.StopLoading();
            kickLoadingBar.StopLoading();
        }

        private void OnOptionsButtonClicked()
        {
            optionsButton.gameObject.SetActive(false);
            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.35f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(
                PlayerId != AuthenticationService.Instance.PlayerId && AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId ? 0.8f
                : PlayerId == GameLobbyManager.Instance.Lobby.HostId ? 0.85f : 1f, 1f);
            hostIndicatorImage.GetComponent<RectTransform>().anchorMin = new Vector2(0.85f, 0f);
            hostIndicatorImage.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);

            optionsPanel.SetActive(true);
        }

        private void OnBackButtonClicked()
        {
            optionsPanel.SetActive(false);

            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.15f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(PlayerId == GameLobbyManager.Instance.Lobby.HostId ? 0.7f : 0.85f, 1f);
            hostIndicatorImage.GetComponent<RectTransform>().anchorMin = new Vector2(0.7f, 0f);
            hostIndicatorImage.GetComponent<RectTransform>().anchorMax = new Vector2(0.85f, 1f);

            optionsButton.gameObject.SetActive(true);
        }

        private void OnSteamProfileButtonClicked()
        {
            Application.OpenURL("https://steamcommunity.com/profiles/" + GameLobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId).Data["SteamId"].Value.ToString());
        }

        private async void OnKickButtonClicked()
        {
            kickButton.interactable = false;
            kickLoadingBar.StartLoading();

            await LobbyManager.KickPlayer(GameLobbyManager.Instance.Lobby.Id, PlayerId);
        }

        private void OnChangeTeamButtonClicked()
        {
            teamPanel.SetActive(true);
        }

        private async void ChangeTeam(int team)
        {

            playerTeamSelector.UpdateInteractable(false);
            changeTeamButton.interactable = false;
            playerTeamSelectorLoadingBar.StartLoading();

            if (team != int.Parse(GameLobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId).Data["Team"].Value))
                await GameLobbyManager.Instance.ChangePlayerTeam(PlayerId, (Team)team);
            else SetTeam(true, (Team)team);
        }

        public async void SetPlayer(string playerId)
        {
            PlayerId = playerId;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);

            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(PlayerId == GameLobbyManager.Instance.Lobby.HostId ? 0.7f : .85f, 1f);

            Player player = GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == PlayerId);
            if (player == null)
            {
                SetTeam(true, Team.Red);
                SetReadyStatus(true, ReadyStatus.NotReady);
                nameText.color = UIColors.Primary.Four;
                nameText.text = "Loading...";
                hostIndicatorImage.gameObject.SetActive(false);
                optionsButton.gameObject.SetActive(false);

                while (player == null)
                {
                    player = GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == PlayerId);
                    if (player == null) await Task.Delay(100);
                }
            }

            SetTeam(true, (Team)int.Parse(player.Data["Team"].Value));
            SetReadyStatus(true, (ReadyStatus)int.Parse(player.Data["ReadyStatus"].Value));
            SetSteamInfo(ulong.Parse(player.Data["SteamId"].Value));

            hostIndicatorImage.gameObject.SetActive(player.Id == GameLobbyManager.Instance.Lobby.HostId);
            optionsButton.gameObject.SetActive(player.Id != AuthenticationService.Instance.PlayerId);
            kickButton.gameObject.SetActive(player.Id != AuthenticationService.Instance.PlayerId && AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId);
        }

        public void SetEmpty()
        {
            PlayerId = null;

            kickLoadingBar.StopLoading();
            playerTeamSelectorLoadingBar.StopLoading();

            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
        }

        public async void SetTeam(bool success, Team team)
        {
            teamPanel.SetActive(false);
            playerTeamSelectorLoadingBar.StopLoading();
            playerTeamSelector.UpdateInteractable(true);

            if (success)
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
            }

            await Task.Delay(1000);

            changeTeamButton.interactable = PlayerId == AuthenticationService.Instance.PlayerId;
        }

        public void SetReadyStatus(bool success, ReadyStatus status)
        {
            if (!success) return;

            switch (status)
            {
                case ReadyStatus.Ready:
                    nameText.color = UIColors.Green.One;
                    SetButtons(true);
                    teamPanel.SetActive(false);
                    break;
                case ReadyStatus.NotReady:
                    nameText.color = UIColors.Red.One;
                    SetButtons(false);
                    break;
                case ReadyStatus.InGame:
                    nameText.color = UIColors.Orange.One;
                    inGameStatePanel.SetActive(true);
                    break;
            }
        }

        public void SetConnectionStatus(bool success, ConnectionStatus status)
        {
            if (!success) return;

            switch (status)
            {
                case ConnectionStatus.Connecting:
                    connectingStatePanel.SetActive(true);
                    disconnectedStatePanel.SetActive(false);
                    break;
                case ConnectionStatus.Connected:
                    connectingStatePanel.SetActive(false);
                    disconnectedStatePanel.SetActive(false);
                    break;
                case ConnectionStatus.Disconnected:
                    disconnectedStatePanel.SetActive(true);
                    connectingStatePanel.SetActive(false);
                    break;
            }
        }

        public void SetButtons(bool ready)
        {
            changeTeamButton.gameObject.SetActive(PlayerId == AuthenticationService.Instance.PlayerId && !ready);
            teamIndicatorImage.gameObject.SetActive(PlayerId != AuthenticationService.Instance.PlayerId || ready);
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

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            int imageHandle = callback.m_iImage;
            if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
            nameText.text = SteamFriends.GetFriendPersonaName(callback.m_steamID);
        }
    }
}