using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Assets.Scripts.Game.UI.Controllers.LobbyCanvas.CharacterCustomizationPanel;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class LobbyPanelController : BasePanel
    {
        [Header("UI Components")]
        // [SerializeField] private Image privateIndicator;
        // [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI startText;
        // [SerializeField] private Button lobbyCodeButton;
        // [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Selector optionsPanelSelector;
        [SerializeField] private TextMeshProUGUI characterOptionsText;
        [SerializeField] private GameSettingsPanelController gameSettingsPanelController;
        [SerializeField] private CharacterCustomizationPanelController characterCustomizationPanelController;

        protected override void OnEnable()
        {
            base.OnEnable();
            startButton.onClick.AddListener(OnStartClicked);
            // lobbyCodeButton.onClick.AddListener(OnLobbyCodeClicked);
            optionsPanelSelector.onSelectionChanged += SetActiveOptionsPanel;
            gameSettingsPanelController.GameModeSelector.onSelectionChanged += SetGameMode;

            // LobbyEvents.OnPlayerLeft += OnPlayerLeft;
            // GameEvents.OnGameStarted += OnGameStarted;
            // GameEvents.OnGameEnded += OnGameEnded;
            // GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;

            optionsPanelSelector.SetSelection(0, true);
            // lobbyNameText.GetComponent<RectTransform>().anchorMin = new Vector2(GameLobbyManager.Instance.Lobby.IsPrivate ? 0.06f : 0f, 0f);
            // lobbyNameText.text = GameLobbyManager.Instance.Lobby.Name;
            // privateIndicator.gameObject.SetActive(GameLobbyManager.Instance.Lobby.IsPrivate);
            // UpdateStartButtonState();
            // lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.Lobby.LobbyCode}";
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            startButton.onClick.RemoveListener(OnStartClicked);
            // lobbyCodeButton.onClick.RemoveListener(OnLobbyCodeClicked);
            optionsPanelSelector.onSelectionChanged -= SetActiveOptionsPanel;
            gameSettingsPanelController.GameModeSelector.onSelectionChanged -= SetGameMode;
        }

        private async void OnStartClicked()
        {
            startButton.interactable = false;
            startText.text = "Starting...";

            await Task.Delay(1000);

            Debug.Log($"Game settings: Map - {gameSettingsPanelController.MapChanger.Value}, Round Count - {gameSettingsPanelController.RoundCountIncrementer.Value}, Round Time - {gameSettingsPanelController.RoundTimeIncrementer.Value}, Game Mode - {gameSettingsPanelController.GameModeSelector.Selection}");
            // Debug.Log($"Character customization: Character - {characterCustomizationPanelController.characterSelector.SelectedIndex}, Color - {characterCustomizationPanelController.colorSelector.SelectedIndex}");

            // start game

            startText.text = "Start";
            startButton.interactable = true;
        }

        // private async void OnLobbyCodeClicked()
        // {
        //     GUIUtility.systemCopyBuffer = GameLobbyManager.Instance.Lobby.LobbyCode;

        //     lobbyCodeText.text = $"Copied!";

        //     await Task.Delay(1000);

        //     lobbyCodeText.text = $"Code: {GameLobbyManager.Instance.Lobby.LobbyCode}";
        // }

        private void SetActiveOptionsPanel(int index)
        {
            if (index == 0)
            {
                gameSettingsPanelController.gameObject.SetActive(true);
                characterCustomizationPanelController.gameObject.SetActive(false);
            }
            else if (index == 1)
            {
                gameSettingsPanelController.gameObject.SetActive(false);
                characterCustomizationPanelController.gameObject.SetActive(true);
            }
        }

        private void SetGameMode(int selection)
        {
            characterOptionsText.text = selection == 0 ? "Character" : "Characters";
        }

        // private void OnPlayerLeft(string playerId)
        // {
        //     if (playerId != AuthenticationService.Instance.PlayerId) UpdateStartButtonState();
        // }

        // private async void OnGameStarted(bool success, string relayJoinCode)
        // {
        //     startLoadingBar.StopLoading();
        //     if (success)
        //     {
        //         startText.text = "In Game";
        //         await GameLobbyManager.Instance.SetPlayerReadyStatus(AuthenticationService.Instance.PlayerId, ReadyStatus.NotReady);

        //         if (AuthenticationService.Instance.PlayerId != GameLobbyManager.Instance.Lobby.HostId)
        //         {
        //             GameObject gameManagerObject = new("GameManager");
        //             gameManagerObject.AddComponent<GameManager>();
        //             await GameManager.Instance.JoinGame(relayJoinCode);

        //         }
        //         else UpdateStartButtonState();
        //     }
        // }

        // private void OnGameEnded()
        // {
        //     UpdateStartButtonState();
        // }

        // private void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
        // {
        //     if (success) UpdateStartButtonState();
        // }

        // private void UpdateStartButtonState()
        // {
        //     int playersReady = 0;
        //     foreach (Player player in GameLobbyManager.Instance.Lobby.Players)
        //         if (player.Data["ReadyStatus"].Value == ((int)ReadyStatus.Ready).ToString()) playersReady++;

        //     int maxPlayers = GameLobbyManager.Instance.Lobby.MaxPlayers;

        //     if (playersReady == maxPlayers)
        //     {
        //         if (AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId)
        //         {

        //             startButton.interactable = true;
        //             startText.text = "Start Game";
        //         }
        //         else startText.text = "Waiting for host...";
        //     }
        //     else
        //     {
        //         startButton.interactable = false;
        //         startText.text = playersReady + " / " + maxPlayers + " Ready";
        //     }
        // }
    }
}