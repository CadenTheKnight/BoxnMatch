using TMPro;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Components.ListEntries
{
    public class PlayerListEntry : MonoBehaviour
    {

        [Header("UI Components")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private GameObject activeStatePanel;
        [SerializeField] private GameObject inGameStatePanel;
        [SerializeField] private GameObject disconnectedStatePanel;
        [SerializeField] private RawImage profilePictureRawImage;
        [SerializeField] private TextMeshProUGUI playerNameText;

        protected Callback<AvatarImageLoaded_t> avatarImageLoadedCallback;

        private Player player;

        public void SetEmpty()
        {
            emptyStatePanel.SetActive(true);
            activeStatePanel.SetActive(false);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);
        }

        public void SetPlayer(Player player)
        {
            this.player = player;

            emptyStatePanel.SetActive(false);
            activeStatePanel.SetActive(true);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);

            avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(SteamFriends.GetLargeFriendAvatar((CSteamID)ulong.Parse(player.Data["Id"].Value)));

            if (player.Data["Status"].Value == PlayerStatus.Ready.ToString() || player.Data["Status"].Value == PlayerStatus.NotReady.ToString())
            {
                bool isHost = LobbyManager.Instance.Lobby.HostId == player.Id;
                playerNameText.text = SteamFriends.GetFriendPersonaName((CSteamID)ulong.Parse(player.Data["Id"].Value)) + (isHost ? " (Host)" : "");
                playerNameText.color = isHost ? UIColors.greenDefaultColor : player.Data["Status"].Value == PlayerStatus.Ready.ToString() ? UIColors.greenDefaultColor : UIColors.redDefaultColor;
            }
            else if (player.Data["Status"].Value == PlayerStatus.InGame.ToString())
            {
                playerNameText.color = UIColors.yellowDefaultColor;
                inGameStatePanel.SetActive(true);
            }
            else if (player.Data["Status"].Value == PlayerStatus.Disconnected.ToString())
            {
                playerNameText.color = UIColors.redDefaultColor;
                disconnectedStatePanel.SetActive(true);
            }
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == ulong.Parse(player.Data["Id"].Value))
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(callback.m_iImage);
        }
    }
}