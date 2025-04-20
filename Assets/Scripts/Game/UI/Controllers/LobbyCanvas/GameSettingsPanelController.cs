using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class GameSettingsPanelController : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private MapChanger mapChanger;
        [SerializeField] private Incrementer roundCountIncrementer;
        [SerializeField] private Incrementer roundTimeIncrementer;
        [SerializeField] private Selector gameModeSelector;

        // [Header("Settings Management")]
        // [SerializeField] private Button editUpdateButton;
        // [SerializeField] private LoadingBar editUpdateLoadingBar;
        // [SerializeField] private TextMeshProUGUI editUpdateText;

        public MapChanger MapChanger => mapChanger;
        public Incrementer RoundCountIncrementer => roundCountIncrementer;
        public Incrementer RoundTimeIncrementer => roundTimeIncrementer;
        public Selector GameModeSelector => gameModeSelector;

        // private void OnEnable()
        // {
        //     mapChanger.OnValueChanged += OnMapIndexChanged;
        //     roundCountIncrementer.OnValueChanged += OnRoundCountChanged;
        //     roundTimeIncrementer.OnValueChanged += OnRoundTimeChanged;
        //     gameModeSelector.OnSelectionChanged += OnGameModeChanged;
        // }

        // private void OnDisable()
        // {
        //     mapChanger.OnValueChanged -= OnMapIndexChanged;
        //     roundCountIncrementer.OnValueChanged -= OnRoundCountChanged;
        //     roundTimeIncrementer.OnValueChanged -= OnRoundTimeChanged;
        //     gameModeSelector.OnSelectionChanged -= OnGameModeChanged;
        // }

        // private bool isEditing = false;

        private void Start()
        {
            // editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

            // LobbyEvents.OnHostMigrated += OnHostMigrated;
            // GameLobbyEvents.OnGameSettingsChanged += OnGameSettingsChanged;
            // GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;

            // UpdateSelections(GameLobbyManager.Instance.Lobby.Data);
            // UpdateOptionsInteractable(isEditing);
            // UpdateEditUpdateButtonState(isEditing);
            // editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;

            mapChanger.SetValue(0);
            roundCountIncrementer.SetValue(5);
            roundTimeIncrementer.SetValue(90);
            gameModeSelector.SetSelection(0);
        }

        // private void OnDestroy()
        // {
        //     editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

        //     LobbyEvents.OnHostMigrated -= OnHostMigrated;
        //     GameLobbyEvents.OnGameSettingsChanged -= OnGameSettingsChanged;
        //     GameLobbyEvents.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;

        //     editUpdateLoadingBar.StopLoading();
        // }

        // private async void OnEditUpdateClicked()
        // {
        //     isEditing = !isEditing;
        //     if (!isEditing)
        //     {
        //         editUpdateButton.interactable = false;
        //         editUpdateText.text = "Updating...";
        //         editUpdateLoadingBar.StartLoading();
        //         UpdateOptionsInteractable(!isEditing);

        //         if (mapChanger.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value)
        //             || roundCountIncrementer.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value)
        //             || roundTimeIncrementer.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value)
        //             || gameModeSelector.Selection != int.Parse(GameLobbyManager.Instance.Lobby.Data["GameMode"].Value))
        //             await GameLobbyManager.Instance.UpdateGameSettings(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, gameModeSelector.Selection);
        //         else OnGameSettingsChanged(true, GameLobbyManager.Instance.Lobby.Data);
        //     }
        //     else
        //     {
        //         UpdateOptionsInteractable(isEditing);
        //         UpdateEditUpdateButtonState(isEditing);
        //     }
        // }

        // private void OnHostMigrated(string playerId)
        // {
        //     editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == playerId
        //         && GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId).Data["ReadyStatus"].Value == ((int)ReadyStatus.NotReady).ToString();
        // }

        // private async void OnGameSettingsChanged(bool success, Dictionary<string, DataObject> gameSettings)
        // {
        //     editUpdateLoadingBar.StopLoading();
        //     UpdateEditUpdateButtonState(isEditing);
        //     UpdateOptionsInteractable(isEditing);

        //     if (success) UpdateSelections(gameSettings);

        //     await Task.Delay(1000);

        //     // editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId
        //     // && GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId).Data["ReadyStatus"].Value == ((int)ReadyStatus.NotReady).ToString();


        // }

        // private void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
        // {
        //     if (success && playerId == AuthenticationService.Instance.PlayerId)
        //     {
        //         editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId && readyStatus == ReadyStatus.NotReady;
        //         if (readyStatus == ReadyStatus.Ready)
        //         {
        //             if (isEditing) UpdateSelections(GameLobbyManager.Instance.Lobby.Data);
        //             isEditing = false;
        //             UpdateEditUpdateButtonState(isEditing);
        //             UpdateOptionsInteractable(isEditing);
        //         }
        //     }
        // }

        // private void UpdateSelections(Dictionary<string, DataObject> gameSettings)
        // {
        //     if (gameSettings.ContainsKey("MapIndex")) mapChanger.SetValue(int.Parse(gameSettings["MapIndex"].Value));
        //     if (gameSettings.ContainsKey("RoundCount")) roundCountIncrementer.SetValue(int.Parse(gameSettings["RoundCount"].Value));
        //     if (gameSettings.ContainsKey("RoundTime")) roundTimeIncrementer.SetValue(int.Parse(gameSettings["RoundTime"].Value));
        //     if (gameSettings.ContainsKey("GameMode")) gameModeSelector.SetSelection(int.Parse(gameSettings["GameMode"].Value));
        // }

        // private void UpdateOptionsInteractable(bool isEditing)
        // {
        //     mapChanger.UpdateInteractable(isEditing);
        //     roundCountIncrementer.UpdateInteractable(isEditing);
        //     roundTimeIncrementer.UpdateInteractable(isEditing);
        //     gameModeSelector.UpdateInteractable(false);
        // }

        // private void UpdateEditUpdateButtonState(bool isEditing)
        // {
        //     editUpdateText.text = isEditing ? "Update" : "Edit Game";

        //     ColorBlock colors = editUpdateButton.colors;
        //     colors.normalColor = isEditing ? UIColors.Green.One : UIColors.Primary.Eight;
        //     colors.highlightedColor = isEditing ? UIColors.Green.Two : UIColors.Primary.Six;
        //     colors.pressedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
        //     colors.selectedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
        //     colors.disabledColor = isEditing ? UIColors.Green.Five : UIColors.Primary.Three;

        //     editUpdateButton.colors = colors;
        // }
    }
}