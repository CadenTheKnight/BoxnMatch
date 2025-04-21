// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Threading.Tasks;
// using Assets.Scripts.Game.Data;
// using Assets.Scripts.Game.Managers;
// using Assets.Scripts.Game.UI.Components;
// using Assets.Scripts.Game.Events;
// using UnityEngine.SceneManagement;

// namespace Assets.Scripts.Game.UI.Controllers.LoadingCanvas
// {
//     public class LoadingPanelController : MonoBehaviour
//     {
//         [Header("References")]
//         [SerializeField] private LoadingBar loadingBar;
//         [SerializeField] private Image mapThumbnailImage;
//         [SerializeField] private TextMeshProUGUI statusText;
//         [SerializeField] private TextMeshProUGUI mapNameText;
//         [SerializeField] private MapSelectionData mapSelectionData;

//         private int connectedPlayers = 0;

//         private void OnEnable()
//         {
//             mapThumbnailImage.sprite = mapSelectionData.GetMap(int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value)).Thumbnail;
//             mapNameText.text = mapSelectionData.GetMap(int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value)).Name;
//             UpdatePlayerCountDisplay();
//             loadingBar.StartLoading();

//             OnPlayerConnect();
//         }

//         private void OnDisable()
//         {
//             loadingBar.StopLoading();
//         }

//         private async void OnPlayerConnect()
//         {
//             connectedPlayers++;

//             if (connectedPlayers == GameLobbyManager.Instance.Lobby.MaxPlayers)
//             {
//                 loadingBar.StopLoading();
//                 statusText.text = "All players connected!";
//                 await Task.Delay(1000);
//                 statusText.text = "Loading map...";


//                 GameEvents.InvokeGameEnded();
//                 SceneManager.UnloadSceneAsync("Loading");
//             }
//             else UpdatePlayerCountDisplay();
//         }

//         private void UpdatePlayerCountDisplay()
//         {
//             statusText.text = $"Waiting for players...";
//         }
//     }
// }