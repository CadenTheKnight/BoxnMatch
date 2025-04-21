// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Types;
// using Assets.Scripts.Game.Events;
// using Assets.Scripts.Game.Managers;
// using Unity.Services.Authentication;
// using Assets.Scripts.Game.UI.Colors;
// using Assets.Scripts.Framework.Types;
// using Assets.Scripts.Framework.Events;
// using Assets.Scripts.Game.UI.Components;
// using Assets.Scripts.Framework.Utilities;

// namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
// {
//     public class PlayerOptionsPanelController : MonoBehaviour
//     {
//         [Header("UI Components")]
//         [SerializeField] private Button leaveButton;
//         [SerializeField] private Button readyUnreadyButton;
//         [SerializeField] private LoadingBar leaveLoadingBar;
//         [SerializeField] private TextMeshProUGUI readyUnreadyText;
//         [SerializeField] private LoadingBar readyUnreadyLoadingBar;

//         private void OnEnable()
//         {
//             leaveButton.onClick.AddListener(OnLeaveClicked);
//             readyUnreadyButton.onClick.AddListener(OnReadyUnreadyClicked);

//             LobbyEvents.OnLobbyLeft += OnLobbyLeft;
//             GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;
//         }

//         private void OnDisable()
//         {
//             leaveButton.onClick.RemoveListener(OnLeaveClicked);
//             readyUnreadyButton.onClick.RemoveListener(OnReadyUnreadyClicked);

//             LobbyEvents.OnLobbyLeft -= OnLobbyLeft;
//             GameLobbyEvents.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;

//             leaveLoadingBar.StopLoading();
//             readyUnreadyLoadingBar.StopLoading();
//         }

//         private async void OnLeaveClicked()
//         {
//             leaveButton.interactable = false;
//             leaveLoadingBar.StartLoading();

//             await GameLobbyManager.Instance.LeaveLobby(GameLobbyManager.Instance.Lobby.Id);
//         }

//         private async void OnReadyUnreadyClicked()
//         {
//             readyUnreadyButton.interactable = false;
//             readyUnreadyLoadingBar.StartLoading();

//             await GameLobbyManager.Instance.SetPlayerReadyStatus(AuthenticationService.Instance.PlayerId, GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId).Data["ReadyStatus"].Value == ((int)ReadyStatus.Ready).ToString() ? ReadyStatus.NotReady : ReadyStatus.Ready);
//         }

//         private async void OnLobbyLeft(OperationResult result)
//         {
//             if (result.Status == ResultStatus.Error)
//             {
//                 leaveLoadingBar.StopLoading();

//                 await Task.Delay(1000);
//                 leaveButton.interactable = true;
//             }
//         }

//         private async void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
//         {
//             if (success)
//             {
//                 if (playerId == AuthenticationService.Instance.PlayerId)
//                 {
//                     UpdateReadyButtonState(readyStatus);
//                     readyUnreadyLoadingBar.StopLoading();

//                     await Task.Delay(1000);

//                     readyUnreadyButton.interactable = true;
//                 }
//             }
//         }

//         private void UpdateReadyButtonState(ReadyStatus readyStatus)
//         {
//             readyUnreadyText.text = readyStatus == ReadyStatus.Ready ? "Ready" : "Not Ready";

//             ColorBlock colors = readyUnreadyButton.colors;
//             if (readyStatus == ReadyStatus.Ready)
//             {
//                 colors.normalColor = UIColors.Green.One;
//                 colors.highlightedColor = UIColors.Green.Two;
//                 colors.pressedColor = UIColors.Green.Three;
//                 colors.selectedColor = UIColors.Green.Three;
//                 colors.disabledColor = UIColors.Green.Five;
//             }
//             else
//             {
//                 colors.normalColor = UIColors.Red.One;
//                 colors.highlightedColor = UIColors.Red.Two;
//                 colors.pressedColor = UIColors.Red.Three;
//                 colors.selectedColor = UIColors.Red.Three;
//                 colors.disabledColor = UIColors.Red.Five;
//             }
//             readyUnreadyButton.colors = colors;
//         }
//     }
// }