using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    /// <summary>
    /// Handles the transitional logic for the settings menu.
    /// </summary>
    public class SettingsPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private Selector panelSelector;
        [SerializeField] private Button applyChangesButton;
        [SerializeField] private Button discardChangesButton;
        [SerializeField] private Button resetToDefaultsButton;

        [Header("Settings Panels References")]
        [SerializeField] private VideoSettingsPanelController videoSettingsPanelController;
        [SerializeField] private AudioSettingsPanelController audioSettingsPanelController;
        [SerializeField] private ControlsSettingsPanelController controlsSettingsPanelController;

        protected override void OnEnable()
        {
            base.OnEnable();

            LoadAllSettings();

            panelSelector.SetSelection(0, true);
            panelSelector.onSelectionChanged += SetActivePanel;

            applyChangesButton.onClick.AddListener(ApplyChanges);
            discardChangesButton.onClick.AddListener(DiscardChanges);
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);

            videoSettingsPanelController.OnVideoSettingsChanged += UpdateActionButtonsState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            panelSelector.onSelectionChanged -= SetActivePanel;

            applyChangesButton.onClick.RemoveListener(ApplyChanges);
            discardChangesButton.onClick.RemoveListener(DiscardChanges);
            resetToDefaultsButton.onClick.RemoveListener(ResetToDefaults);

            videoSettingsPanelController.OnVideoSettingsChanged -= UpdateActionButtonsState;
        }

        private void SetActivePanel(int index)
        {
            videoSettingsPanelController.gameObject.SetActive(false);
            audioSettingsPanelController.gameObject.SetActive(false);
            controlsSettingsPanelController.gameObject.SetActive(false);

            if (index == 0) videoSettingsPanelController.gameObject.SetActive(true);
            else if (index == 1) audioSettingsPanelController.gameObject.SetActive(true);
            else if (index == 2) controlsSettingsPanelController.gameObject.SetActive(true);

            UpdateActionButtonsState();
        }

        private void UpdateActionButtonsState()
        {
            bool hasChanges = false;
            bool isDefaults = false;

            if (videoSettingsPanelController.gameObject.activeSelf)
            {
                hasChanges = videoSettingsPanelController.HasChanges();
                isDefaults = videoSettingsPanelController.IsDefaults();
            }

            resetToDefaultsButton.interactable = !isDefaults;
            applyChangesButton.interactable = hasChanges;
            discardChangesButton.interactable = hasChanges;
        }

        private void LoadAllSettings()
        {
            videoSettingsPanelController.LoadSettings();
        }

        private void ResetToDefaults()
        {
            if (videoSettingsPanelController.gameObject.activeSelf && videoSettingsPanelController.HasChanges())
            {
                videoSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.ShowNotification(OperationResult.ErrorResult("VideoSettingsReset", "Video settings reset to default"));
            }

            UpdateActionButtonsState();
        }

        private void DiscardChanges()
        {
            if (videoSettingsPanelController.gameObject.activeSelf && videoSettingsPanelController.HasChanges())
            {
                videoSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("VideoSettingsDiscarded", "Video settings changes discarded"));
            }

            UpdateActionButtonsState();
        }

        private void ApplyChanges()
        {
            if (videoSettingsPanelController.gameObject.activeSelf && videoSettingsPanelController.HasChanges())
            {
                videoSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.SuccessResult("VideoSettingsChanged", "Video settings changes applied"));
            }

            UpdateActionButtonsState();
        }

        public override void HidePanel()
        {
            DiscardChanges();
            base.HidePanel();
        }
    }
}