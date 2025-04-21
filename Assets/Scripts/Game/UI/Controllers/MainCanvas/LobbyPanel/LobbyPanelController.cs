using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel.CharacterPanel;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel
{
    public class LobbyPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI startText;
        [SerializeField] private Selector optionsPanelSelector;
        [SerializeField] private TextMeshProUGUI characterOptionsText;
        [SerializeField] private MatchSettingsPanelController matchSettingsPanelController;
        [SerializeField] private CharacterPanelController characterPanelController;

        private void Start()
        {
            optionsPanelSelector.SetSelection(0, true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            startButton.onClick.AddListener(OnStartClicked);
            optionsPanelSelector.OnSelectionChanged += SetActiveOptionsPanel;
            matchSettingsPanelController.GameModeSelector.OnSelectionChanged += SetGameMode;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            startButton.onClick.RemoveListener(OnStartClicked);
            optionsPanelSelector.OnSelectionChanged -= SetActiveOptionsPanel;
            matchSettingsPanelController.GameModeSelector.OnSelectionChanged -= SetGameMode;
        }

        private void OnStartClicked()
        {
            GameManager.Instance.StartGame(matchSettingsPanelController.MapChanger.Value, matchSettingsPanelController.RoundCountIncrementer.Value, matchSettingsPanelController.RoundTimeIncrementer.Value, (GameMode)matchSettingsPanelController.GameModeSelector.Selection);
        }

        private void SetActiveOptionsPanel(int index)
        {
            matchSettingsPanelController.gameObject.SetActive(index == 0);
            characterPanelController.gameObject.SetActive(index == 1);
        }

        private void SetGameMode(int selection)
        {
            characterOptionsText.text = selection == 0 ? "Character" : "Characters";
        }
    }
}