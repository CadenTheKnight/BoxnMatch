
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Events;

namespace Assets.Scripts.Game
{
    public class LobbyController : MonoBehaviour
    {
        // [SerializeField] private TextMeshProUGUI LobbyNameText;
        [SerializeField] private TextMeshProUGUI LobbyCodeText;
        [SerializeField] private Button readyButton;
        [SerializeField] private Image mapImage;
        [SerializeField] private Button mapLeftButton;
        [SerializeField] private Button mapRightButton;
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private MapSelectionData mapSelectionData;

        private int currentMapIndex = 0;

        private void OnEnable()
        {
            if (GameLobbyManager.Instance.IsHost)
            {
                mapLeftButton.onClick.AddListener(OnMapLeftClicked);
                mapRightButton.onClick.AddListener(OnMapRightClicked);
            }

            readyButton.onClick.AddListener(OnReadyClicked);

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            if (GameLobbyManager.Instance.IsHost)
            {
                mapLeftButton.onClick.RemoveAllListeners();
                mapRightButton.onClick.RemoveAllListeners();
            }

            readyButton.onClick.RemoveAllListeners();

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
        }

        void Start()
        {
            // LobbyNameText.text = GameLobbyManager.Instance.GetLobbyName();
            LobbyCodeText.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";

            if (!GameLobbyManager.Instance.IsHost)
            {
                mapLeftButton.gameObject.SetActive(false);
                mapRightButton.gameObject.SetActive(false);
            }
        }

        private async void OnMapLeftClicked()
        {
            if (currentMapIndex > 0)
            {
                currentMapIndex--;
            }
            else
            {
                currentMapIndex = mapSelectionData.Maps.Count - 1;
            }


            bool succeeded = await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);
            if (succeeded)
            {
                UpdateMap();
            }
        }

        private async void OnMapRightClicked()
        {
            if (currentMapIndex < mapSelectionData.Maps.Count - 1)
            {
                currentMapIndex++;
            }
            else
            {
                currentMapIndex = 0;
            }

            bool succeeded = await GameLobbyManager.Instance.SetSelectedMap(currentMapIndex);
            if (succeeded)
            {
                UpdateMap();
            }
        }

        private async void OnReadyClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.SetPlayerReady();
            if (succeeded)
            {
                readyButton.interactable = false;
            }
        }

        private void UpdateMap()
        {
            mapImage.color = mapSelectionData.Maps[currentMapIndex].MapThumbnail;
            mapNameText.text = mapSelectionData.Maps[currentMapIndex].MapName;
        }

        private void OnLobbyUpdated()
        {
            currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            UpdateMap();
        }
    }
}