using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;

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
            panelSelector.onSelectionChanged += (index) =>
            {
                switch (index)
                {
                    case 0:
                        SetActivePanel(videoSettingsPanelController.gameObject);
                        break;
                    case 1:
                        SetActivePanel(audioSettingsPanelController.gameObject);
                        break;
                    case 2:
                        SetActivePanel(controlsSettingsPanelController.gameObject);
                        break;
                }
                UpdateActionButtonsState();
            };

            applyChangesButton.onClick.AddListener(ApplyChanges);
            discardChangesButton.onClick.AddListener(DiscardChanges);
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);

            videoSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
            audioSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
            controlsSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            panelSelector.onSelectionChanged -= (index) =>
            {
                switch (index)
                {
                    case 0:
                        SetActivePanel(videoSettingsPanelController.gameObject);
                        break;
                    case 1:
                        SetActivePanel(audioSettingsPanelController.gameObject);
                        break;
                    case 2:
                        SetActivePanel(controlsSettingsPanelController.gameObject);
                        break;
                }
                UpdateActionButtonsState();
            };

            applyChangesButton.onClick.RemoveListener(ApplyChanges);
            discardChangesButton.onClick.RemoveListener(DiscardChanges);
            resetToDefaultsButton.onClick.RemoveListener(ResetToDefaults);

            videoSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
            audioSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
            controlsSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
        }

        private void SetActivePanel(GameObject activePanel)
        {
            videoSettingsPanelController.gameObject.SetActive(false);
            audioSettingsPanelController.gameObject.SetActive(false);
            controlsSettingsPanelController.gameObject.SetActive(false);

            activePanel.SetActive(true);
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
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                hasChanges = audioSettingsPanelController.HasChanges();
                isDefaults = audioSettingsPanelController.IsDefaults();
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                hasChanges = controlsSettingsPanelController.HasChanges();
                isDefaults = controlsSettingsPanelController.IsDefaults();
            }

            resetToDefaultsButton.interactable = !isDefaults;
            applyChangesButton.interactable = hasChanges;
            discardChangesButton.interactable = hasChanges;
        }

        private void LoadAllSettings()
        {
            videoSettingsPanelController.LoadSettings();
            audioSettingsPanelController.LoadSettings();
            controlsSettingsPanelController.LoadSettings();
        }

        private void ResetToDefaults()
        {
            if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.ShowNotification(OperationResult.ErrorResult("VideoSettingsReset", "Video settings reset to default"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.ShowNotification(OperationResult.ErrorResult("AudioSettingsReset", "Audio settings reset to default"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.ShowNotification(OperationResult.ErrorResult("ControlsSettingsReset", "Controls settings reset to default"));
            }

            UpdateActionButtonsState();
        }

        private void DiscardChanges()
        {
            if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("VideoSettingsDiscarded", "Video settings changes discarded"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("AudioSettingsDiscarded", "Audio settings changes discarded"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.WarningResult("ControlsSettingsDiscarded", "Controls settings changes discarded"));
            }

            UpdateActionButtonsState();
        }

        private void ApplyChanges()
        {
            if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.SuccessResult("VideoSettingsChanged", "Video settings changes applied"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.SuccessResult("AudioSettingsChanged", "Audio settings changes applied"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.ShowNotification(OperationResult.SuccessResult("ControlsSettingsChanged", "Controls settings changes applied"));
            }

            UpdateActionButtonsState();
        }

        public override void HidePanel()
        {
            base.HidePanel();
        }
    }
}