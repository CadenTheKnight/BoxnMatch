// using TMPro;
// using System;
// using UnityEngine;
// using UnityEngine.UI;
// using Assets.Scripts.Game.Data;
// using Assets.Scripts.Game.Types;
// using Unity.Services.Lobbies.Models;
// using Assets.Scripts.Game.UI.Colors;

// namespace Assets.Scripts.Game.UI.Components.ListEntries
// {
//     /// <summary>
//     /// Represents a lobby list entry in the join UI.
//     /// /// </summary>
//     public class LobbyListEntry : MonoBehaviour
//     {
//         [Header("UI Components")]
//         [SerializeField] private Button lobbyButton;
//         [SerializeField] private TextMeshProUGUI nameText;
//         [SerializeField] private TextMeshProUGUI playerCountText;
//         [SerializeField] private TextMeshProUGUI gameModeText;
//         [SerializeField] private Image mapImage;
//         [SerializeField] private MapSelectionData mapSelectionData;

//         private Lobby lobby;
//         private float lastClickTime;
//         private bool doubleClickCooldown = false;

//         public Action<string, LobbyListEntry> lobbySingleClicked;
//         public Action lobbyDoubleClicked;

//         private void OnEnable()
//         {
//             lobbyButton.onClick.AddListener(HandleClick);
//         }

//         private void OnDestroy()
//         {
//             lobbyButton.onClick.RemoveListener(HandleClick);
//         }

//         private void HandleClick()
//         {
//             if (doubleClickCooldown || lobby.AvailableSlots == 0) return;

//             if (Time.time - lastClickTime <= 0.5f)
//             {
//                 lobbyDoubleClicked?.Invoke();
//                 doubleClickCooldown = true;
//                 Invoke(nameof(ResetDoubleClickCooldown), 1f);
//             }
//             else
//             {
//                 lobbySingleClicked?.Invoke(lobby.Id, this);
//                 SetSelected(true);
//             }

//             lastClickTime = Time.time;
//         }

//         private void ResetDoubleClickCooldown()
//         {
//             doubleClickCooldown = false;
//             lastClickTime = 0f;
//         }

//         public void SetLobby(Lobby lobby)
//         {
//             this.lobby = lobby;
//             nameText.text = lobby.Name;
//             playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
//             gameModeText.text = ((GameMode)int.Parse(lobby.Data["GameMode"].Value)).ToString();
//             mapImage.sprite = mapSelectionData.GetMap(int.Parse(lobby.Data["MapIndex"].Value)).Thumbnail;

//             SetSelected(false);
//         }

//         public void SetSelected(bool isSelected)
//         {
//             ColorBlock colors = lobbyButton.colors;
//             colors.normalColor = isSelected ? UIColors.Primary.Six : UIColors.Primary.Nine;
//             lobbyButton.colors = colors;
//         }
//     }
// }