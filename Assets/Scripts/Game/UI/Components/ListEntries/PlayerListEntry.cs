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
        [SerializeField] private GameObject playerStatePanel;
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
            playerStatePanel.SetActive(false);
        }

        public void SetPlayer(Player player)
        {
            this.player = player;

            playerStatePanel.SetActive(true);
            emptyStatePanel.SetActive(false);

            activeStatePanel.SetActive(true);
            inGameStatePanel.SetActive(false);
            disconnectedStatePanel.SetActive(false);

            avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            int imageHandle = SteamFriends.GetLargeFriendAvatar((CSteamID)ulong.Parse(player.Data["Id"].Value));
            if (imageHandle == -1)
                SteamFriends.RequestUserInformation((CSteamID)ulong.Parse(player.Data["Id"].Value), false);
            else
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(imageHandle);

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
            if (player != null && player.Data.TryGetValue("Id", out PlayerDataObject idObject) &&
                ulong.TryParse(idObject.Value, out ulong playerId) &&
                callback.m_steamID.m_SteamID == playerId)
            {
                profilePictureRawImage.texture = GetSteamInfo.SteamImageToUnityImage(callback.m_iImage);
                string currentName = playerNameText.text;
                if (currentName.Contains("[unknown]") || currentName == "Unknown Player")
                {
                    string updatedName = SteamFriends.GetFriendPersonaName(callback.m_steamID);
                    if (updatedName != "[unknown]")
                    {
                        bool isHost = LobbyManager.Instance.Lobby.HostId == player.Id;
                        playerNameText.text = updatedName + (isHost ? " (Host)" : "");
                    }
                }
            }
        }
    }
}