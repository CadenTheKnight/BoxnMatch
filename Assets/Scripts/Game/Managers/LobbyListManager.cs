// using Assets.Scripts.Framework.Core;
// using Assets.Scripts.Game.Managers;


// namespace Assets.Scripts.Game.Managers
// {
//     /// <summary>
//     /// Manages the list of lobbies available for players to join.
//     /// </summary>
//     public class LobbyListManager : Singleton<LobbyListManager>
//     {
//         #region Public Methods

//         /// <summary>
//         /// Refreshes the list of active lobbies.
//         /// </summary>
//         private async void RefreshLobbyList()
//         {
//             ClearLobbyList();

//             var lobbies = await GameLobbyManager.Instance.GetActiveLobbies();

//             foreach (var lobby in lobbies)
//             {
//                 var lobbyData = LobbyListLobbyData.FromLobby(lobby);
//                 if (lobbyData != null)
//                 {
//                     var lobbyItem = Instantiate(lobbyListItemPrefab, lobbyListContent);
//                     var lobbyItemController = lobbyItem.GetComponent<LobbyListItemController>();
//                     if (lobbyItemController != null)
//                     {
//                         lobbyItemController.SetData(lobbyData);
//                         lobbyItemController.OnSelected += () => OnLobbySelected(lobbyData);
//                     }
//                 }
//             }
//         }

//         #endregion

//         private void OnLobbySelected(LobbyListLobbyData lobbyData)
//         {
//             selectedLobby = lobbyData;
//             UpdateJoinButtonState();
//         }

//         private async void JoinThroughList(LobbyListLobbyData lobby)
//         {
//             if (lobby != null)
//             {
//                 await GameLobbyManager.Instance.JoinLobbyById(lobby.LobbyId);
//             }
//         }
//     }
// }
