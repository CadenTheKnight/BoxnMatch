using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.SettingsMenu
{
    /// <summary>
    /// Handles the transitional logic for the settings menu.
    /// </summary>
    public class SettingsPanelController : BasePanel
    {
        [Header("Sub Panels")]
        [SerializeField] private GameSettingsPanelController gameSettingsPanelController;
        [SerializeField] private VideoSettingsPanelController videoSettingsPanelController;
        [SerializeField] private AudioSettingsPanelController audioSettingsPanelController;
        [SerializeField] private ControlsSettingsPanelController controlsSettingsPanelController;

        [Header("Tab Buttons")]
        [SerializeField] private Button gameTabButton;
        [SerializeField] private Button videoTabButton;
        [SerializeField] private Button audioTabButton;
        [SerializeField] private Button controlsTabButton;

        [Header("Action Buttons")]
        [SerializeField] private Button applyChangesButton;
        [SerializeField] private Button discardChangesButton;
        [SerializeField] private Button resetToDefaultsButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            LoadAllSettings();
            ShowGamePanel();

            UpdateActionButtonsState();

            gameTabButton.onClick.AddListener(ShowGamePanel);
            videoTabButton.onClick.AddListener(ShowVideoPanel);
            audioTabButton.onClick.AddListener(ShowAudioPanel);
            controlsTabButton.onClick.AddListener(ShowControlsPanel);

            applyChangesButton.onClick.AddListener(ApplyChanges);
            discardChangesButton.onClick.AddListener(DiscardChanges);
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);

            gameSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
            videoSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
            audioSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
            controlsSettingsPanelController.OnSettingsChanged += UpdateActionButtonsState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            gameTabButton.onClick.RemoveListener(ShowGamePanel);
            videoTabButton.onClick.RemoveListener(ShowVideoPanel);
            audioTabButton.onClick.RemoveListener(ShowAudioPanel);
            controlsTabButton.onClick.RemoveListener(ShowControlsPanel);

            applyChangesButton.onClick.RemoveListener(ApplyChanges);
            discardChangesButton.onClick.RemoveListener(DiscardChanges);
            resetToDefaultsButton.onClick.RemoveListener(ResetToDefaults);

            gameSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
            videoSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
            audioSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
            controlsSettingsPanelController.OnSettingsChanged -= UpdateActionButtonsState;
        }

        /// <summary>
        /// Shows the game settings panel.
        /// </summary>
        public void ShowGamePanel()
        {
            SetActivePanel(gameSettingsPanelController.gameObject);
            SetSelectedTab(gameTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the video settings panel.
        /// </summary>
        public void ShowVideoPanel()
        {
            SetActivePanel(videoSettingsPanelController.gameObject);
            SetSelectedTab(videoTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the audio settings panel.
        /// </summary>
        public void ShowAudioPanel()
        {
            SetActivePanel(audioSettingsPanelController.gameObject);
            SetSelectedTab(audioTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the controls settings panel.
        /// </summary>
        public void ShowControlsPanel()
        {
            SetActivePanel(controlsSettingsPanelController.gameObject);
            SetSelectedTab(controlsTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Hides all panels and shows the active panel.
        /// </summary>
        /// <param name="activePanel">The selected panel.</param>
        private void SetActivePanel(GameObject activePanel)
        {
            gameSettingsPanelController.gameObject.SetActive(false);
            videoSettingsPanelController.gameObject.SetActive(false);
            audioSettingsPanelController.gameObject.SetActive(false);
            controlsSettingsPanelController.gameObject.SetActive(false);

            activePanel.SetActive(true);
        }

        /// <summary>
        /// Sets the selected tab color and unselects the other tabs.
        /// </summary>
        /// <param name="selectedTab">The selected tab.</param>
        private void SetSelectedTab(Button selectedTab)
        {
            SetTabColor(gameTabButton, false);
            SetTabColor(videoTabButton, false);
            SetTabColor(audioTabButton, false);
            SetTabColor(controlsTabButton, false);

            SetTabColor(selectedTab, true);
        }

        /// <summary>
        /// Sets the tab color.
        /// </summary>
        /// <param name="tab">The current tab.</param>
        /// <param name="selected">Whether the tab is selected.</param>
        private void SetTabColor(Button tab, bool selected)
        {
            var colors = tab.colors;

            if (selected)
                colors.normalColor = new Color32(0x2F, 0x35, 0x42, 0xFF);
            else
                colors.normalColor = new Color32(0x74, 0x7D, 0x8C, 0xFF);

            tab.colors = colors;
        }

        /// <summary>
        /// Enable or disable Apply/Discard buttons based on whether changes have been made.
        /// </summary>
        private void UpdateActionButtonsState()
        {
            bool hasChanges = false;

            if (gameSettingsPanelController.gameObject.activeSelf)
                hasChanges = gameSettingsPanelController.HasChanges();
            else if (videoSettingsPanelController.gameObject.activeSelf)
                hasChanges = videoSettingsPanelController.HasChanges();
            else if (audioSettingsPanelController.gameObject.activeSelf)
                hasChanges = audioSettingsPanelController.HasChanges();
            else if (controlsSettingsPanelController.gameObject.activeSelf)
                hasChanges = controlsSettingsPanelController.HasChanges();

            applyChangesButton.interactable = hasChanges;
            discardChangesButton.interactable = hasChanges;
        }

        /// <summary>
        /// Loads all settings from the player preferences.
        /// </summary>
        private void LoadAllSettings()
        {
            gameSettingsPanelController.LoadSettings();
            videoSettingsPanelController.LoadSettings();
            audioSettingsPanelController.LoadSettings();
            controlsSettingsPanelController.LoadSettings();
        }

        /// <summary>
        /// Applies changes for the current panel.
        /// </summary>
        private void ApplyChanges()
        {
            if (gameSettingsPanelController.gameObject.activeSelf)
            {
                gameSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("GameSettingsChanged", "Game settings changes applied"));
            }
            else if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("VideoSettingsChanged", "Video settings changes applied"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("AudioSettingsChanged", "Audio settings changes applied"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.ApplyChanges();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("ControlsSettingsChanged", "Controls settings changes applied"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Resets the current panel settings to their defaults.
        /// </summary>
        private void ResetToDefaults()
        {
            if (gameSettingsPanelController.gameObject.activeSelf)
            {
                gameSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("GameSettingsReset", "Game settings reset to default"));
            }
            else if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("VideoSettingsReset", "Video settings reset to default"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("AudioSettingsReset", "Audio settings reset to default"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.ResetToDefaults();
                NotificationManager.Instance.HandleResult(OperationResult.SuccessResult("ControlsSettingsReset", "Controls settings reset to default"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Discards changes for the current panel.
        /// </summary>
        private void DiscardChanges()
        {
            if (gameSettingsPanelController.gameObject.activeSelf)
            {
                gameSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("GameSettingsDiscarded", "Game settings changes discarded"));
            }
            else if (videoSettingsPanelController.gameObject.activeSelf)
            {
                videoSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("VideoSettingsDiscarded", "Video settings changes discarded"));
            }
            else if (audioSettingsPanelController.gameObject.activeSelf)
            {
                audioSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("AudioSettingsDiscarded", "Audio settings changes discarded"));
            }
            else if (controlsSettingsPanelController.gameObject.activeSelf)
            {
                controlsSettingsPanelController.DiscardChanges();
                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("ControlsSettingsDiscarded", "Controls settings changes discarded"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Closes the settings menu.
        /// </summary>
        public override void HidePanel()
        {
            bool hasUnsavedChanges =
                gameSettingsPanelController.HasChanges() ||
                videoSettingsPanelController.HasChanges() ||
                audioSettingsPanelController.HasChanges() ||
                controlsSettingsPanelController.HasChanges();

            if (hasUnsavedChanges)
            {
                gameSettingsPanelController.DiscardChanges();
                videoSettingsPanelController.DiscardChanges();
                audioSettingsPanelController.DiscardChanges();
                controlsSettingsPanelController.DiscardChanges();

                NotificationManager.Instance.HandleResult(OperationResult.WarningResult("SettingsDiscarded", "Settings changes discarded"));
            }

            base.HidePanel();
        }
    }
}