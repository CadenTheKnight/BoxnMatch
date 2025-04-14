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
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Assets.Scripts.Framework.Types;
using System.Collections.Generic;
using Assets.Scripts.Game.Events;

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
            playerTeamSelector.onSelectionChanged += ChangeTeam;

            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            steamProfileButton.onClick.AddListener(OnSteamProfileButtonClicked);
            kickButton.onClick.AddListener(OnKickButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);

            // LobbyEvents.OnPlayerDataUpdated += OnPlayerDataUpdated;

        }

        private void OnDisable()
        {
            playerTeamSelector.onSelectionChanged -= ChangeTeam;

            changeTeamButton.onClick.RemoveListener(OnChangeTeamButtonClicked);
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            steamProfileButton.onClick.RemoveListener(OnSteamProfileButtonClicked);
            kickButton.onClick.RemoveListener(OnKickButtonClicked);
            optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);

            // LobbyEvents.OnPlayerDataUpdated -= OnPlayerDataUpdated;

            playerTeamSelectorLoadingBar.StopLoading();
            kickLoadingBar.StopLoading();
        }

        public void SetPlayer(string playerId)
        {
            PlayerId = playerId;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);

            Player player = GameLobbyManager.Instance.Lobby.Players.Find(player => player.Id == PlayerId);

            SetTeam((Team)int.Parse(player.Data["Team"].Value));
            SetStatus((PlayerStatus)int.Parse(player.Data["Status"].Value));
            SetSteamInfo(ulong.Parse(player.Data["SteamId"].Value));

            hostIndicatorImage.gameObject.SetActive(player.Id == GameLobbyManager.Instance.Lobby.HostId);
            optionsButton.gameObject.SetActive(player.Id != AuthenticationService.Instance.PlayerId);
        }

        public void SetEmpty()
        {
            PlayerId = null;

            kickLoadingBar.StopLoading();
            playerTeamSelectorLoadingBar.StopLoading();

            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
        }



        public void SetButtons(bool ready)
        {
            changeTeamButton.gameObject.SetActive(PlayerId == AuthenticationService.Instance.PlayerId && !ready);
            teamIndicatorImage.gameObject.SetActive(PlayerId != AuthenticationService.Instance.PlayerId || ready);
        }

        // public void SetConnecting()
        // {
        //     connectingStatePanel.SetActive(true);
        // }

        // public void SetConnected()
        // {
        //     connectingStatePanel.SetActive(false);
        // }

        // public void SetDisconnected()
        // {
        //     disconnectedStatePanel.SetActive(true);
        // }

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

        private void OnChangeTeamButtonClicked()
        {
            teamPanel.SetActive(true);
        }

        private async void ChangeTeam(int team)
        {
            try
            {
                playerTeamSelector.UpdateInteractable(false);
                changeTeamButton.interactable = false;
                playerTeamSelectorLoadingBar.StartLoading();

                Task<OperationResult> toggleTask = GameLobbyManager.Instance.ChangePlayerTeam(GameLobbyManager.Instance.GetPlayerById(AuthenticationService.Instance.PlayerId), (Team)team);
                await Task.WhenAny(toggleTask, Task.Delay(5000));

                if (toggleTask.IsCompletedSuccessfully && toggleTask.Result.Status == ResultStatus.Success)
                {
                    if (toggleTask.Result.Data is Dictionary<string, PlayerDataObject> dataDict && dataDict.ContainsKey("Team"))
                    {
                        SetTeam((Team)int.Parse(dataDict["Team"].Value));

                        GameLobbyEvents.InvokePlayerTeamChanged(AuthenticationService.Instance.PlayerId);
                    }
                }
            }
            finally
            {
                playerTeamSelectorLoadingBar.StopLoading();
                playerTeamSelector.UpdateInteractable(true);
                teamPanel.SetActive(false);

                await Task.Delay(1000);
                changeTeamButton.interactable = true;
            }
        }

        public void SetStatus(PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Ready:
                    nameText.color = UIColors.Green.One;
                    SetButtons(true);
                    teamPanel.SetActive(false);
                    break;
                case PlayerStatus.NotReady:
                    nameText.color = UIColors.Red.One;
                    SetButtons(false);
                    break;
                case PlayerStatus.InGame:
                    nameText.color = UIColors.Orange.One;
                    inGameStatePanel.SetActive(true);
                    break;
            }
        }

        public void SetTeam(Team team)
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

        private void OnOptionsButtonClicked()
        {
            bool isHost = PlayerId == GameLobbyManager.Instance.Lobby.HostId;
            bool kickablePlayer = PlayerId != AuthenticationService.Instance.PlayerId && AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;

            optionsButton.gameObject.SetActive(false);
            hostIndicatorImage.gameObject.SetActive(isHost);
            kickButton.gameObject.SetActive(kickablePlayer);

            nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0.35f, 0f);
            nameText.GetComponent<RectTransform>().anchorMax = new Vector2(kickablePlayer ? 0.7f : isHost ? 0.85f : 1f, 1f);
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

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            int imageHandle = callback.m_iImage;
            if (imageHandle > 0) profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);
            nameText.text = SteamFriends.GetFriendPersonaName(callback.m_steamID);
        }
    }
}