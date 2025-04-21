using UnityEngine;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel.CharacterPanel
{
    public class CharacterPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject character;
        [SerializeField] private Selector characterSelector;
        [SerializeField] private CharacterSettingsPanelController characterOneSettingsPanelController;
        [SerializeField] private CharacterSettingsPanelController characterTwoSettingsPanelController;
        [SerializeField] private MatchSettingsPanelController matchSettingsPanelController;

        private void Start()
        {
            characterSelector.SetSelection(0, true);
            matchSettingsPanelController.GameModeSelector.OnSelectionChanged += SetGameMode;
        }

        private void OnEnable()
        {
            characterSelector.OnSelectionChanged += SetActiveCharacter;
        }

        private void OnDisable()
        {
            characterSelector.OnSelectionChanged -= SetActiveCharacter;
        }

        private void OnDestroy()
        {
            matchSettingsPanelController.GameModeSelector.OnSelectionChanged -= SetGameMode;
        }

        private void SetActiveCharacter(int selection)
        {
            characterOneSettingsPanelController.gameObject.SetActive(selection == 0);
            characterTwoSettingsPanelController.gameObject.SetActive(selection == 1);
        }

        private void SetGameMode(int selection)
        {
            characterSelector.gameObject.SetActive(selection == 1);
        }
    }
}