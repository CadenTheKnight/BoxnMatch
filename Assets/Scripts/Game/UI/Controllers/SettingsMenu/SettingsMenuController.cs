using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.SettingsMenu
{
    /// <summary>
    /// Handles the transitional logic for the settings menu.
    /// </summary>
    public class SettingsMenuController : BasePanel
    {
        [Header("Settings Panels Controllers")]
        [SerializeField] private GameSettingsPanelController gameSettingsController;
        [SerializeField] private VideoSettingsPanelController videoSettingsController;
        [SerializeField] private AudioSettingsPanelController audioSettingsController;
        [SerializeField] private ControlsSettingsPanelController controlsSettingsController;


        [Header("Tab Buttons")]
        [SerializeField] private Button gameTabButton;
        [SerializeField] private Button videoTabButton;
        [SerializeField] private Button audioTabButton;
        [SerializeField] private Button controlsTabButton;

        [Header("Action Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button discardChangesButton;
        [SerializeField] private Button resetToDefaultsButton;

        [Header("Tab Visual Indicators")]
        [SerializeField] private Color selectedTabColor = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color unselectedTabColor = new(0.5f, 0.5f, 0.5f);

        [Header("Result Handling")]
        [SerializeField] private ResultHandler resultHandler;


        private void Start()
        {
            gameTabButton.onClick.AddListener(ShowGamePanel);
            videoTabButton.onClick.AddListener(ShowVideoPanel);
            audioTabButton.onClick.AddListener(ShowAudioPanel);
            controlsTabButton.onClick.AddListener(ShowControlsPanel);

            applyButton.onClick.AddListener(ApplyChanges);
            discardChangesButton.onClick.AddListener(DiscardChanges);
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);

            LoadAllSettings();
            ShowGamePanel();

            UpdateActionButtonsState();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            gameSettingsController.OnSettingsChanged += UpdateActionButtonsState;
            videoSettingsController.OnSettingsChanged += UpdateActionButtonsState;
            audioSettingsController.OnSettingsChanged += UpdateActionButtonsState;
            controlsSettingsController.OnSettingsChanged += UpdateActionButtonsState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            gameSettingsController.OnSettingsChanged -= UpdateActionButtonsState;
            videoSettingsController.OnSettingsChanged -= UpdateActionButtonsState;
            audioSettingsController.OnSettingsChanged -= UpdateActionButtonsState;
            controlsSettingsController.OnSettingsChanged -= UpdateActionButtonsState;
        }

        /// <summary>
        /// Shows the game settings panel.
        /// </summary>
        public void ShowGamePanel()
        {
            SetActivePanel(gameSettingsController.gameObject);
            SetSelectedTab(gameTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the video settings panel.
        /// </summary>
        public void ShowVideoPanel()
        {
            SetActivePanel(videoSettingsController.gameObject);
            SetSelectedTab(videoTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the audio settings panel.
        /// </summary>
        public void ShowAudioPanel()
        {
            SetActivePanel(audioSettingsController.gameObject);
            SetSelectedTab(audioTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Shows the controls settings panel.
        /// </summary>
        public void ShowControlsPanel()
        {
            SetActivePanel(controlsSettingsController.gameObject);
            SetSelectedTab(controlsTabButton);
            UpdateActionButtonsState();
        }

        /// <summary>
        /// Hides all panels and shows the active panel.
        /// </summary>
        /// <param name="activePanel">The selected panel.</param>
        private void SetActivePanel(GameObject activePanel)
        {
            gameSettingsController.gameObject.SetActive(false);
            videoSettingsController.gameObject.SetActive(false);
            audioSettingsController.gameObject.SetActive(false);
            controlsSettingsController.gameObject.SetActive(false);

            activePanel.SetActive(true);
        }

        /// <summary>
        /// Sets the selected tab color and unselects the other tabs.
        /// </summary>
        /// <param name="selectedTab">The selected tab.</param>
        private void SetSelectedTab(Button selectedTab)
        {
            SetTabColor(gameTabButton, unselectedTabColor);
            SetTabColor(videoTabButton, unselectedTabColor);
            SetTabColor(audioTabButton, unselectedTabColor);
            SetTabColor(controlsTabButton, unselectedTabColor);

            SetTabColor(selectedTab, selectedTabColor);
        }

        /// <summary>
        /// Sets the tab color.
        /// </summary>
        /// <param name="tab">The current tab.</param>
        /// <param name="color">The color being applied.</param>
        private void SetTabColor(Button tab, Color color)
        {
            tab.targetGraphic.color = color;
        }

        /// <summary>
        /// Enable or disable Apply/Discard buttons based on whether changes have been made.
        /// </summary>
        private void UpdateActionButtonsState()
        {
            bool hasChanges = false;

            if (gameSettingsController.gameObject.activeSelf)
                hasChanges = gameSettingsController.HasChanges();
            else if (videoSettingsController.gameObject.activeSelf)
                hasChanges = videoSettingsController.HasChanges();
            else if (audioSettingsController.gameObject.activeSelf)
                hasChanges = audioSettingsController.HasChanges();
            else if (controlsSettingsController.gameObject.activeSelf)
                hasChanges = controlsSettingsController.HasChanges();

            applyButton.interactable = hasChanges;
            discardChangesButton.interactable = hasChanges;
        }

        /// <summary>
        /// Loads all settings from the player preferences.
        /// </summary>
        private void LoadAllSettings()
        {
            gameSettingsController.LoadSettings();
            videoSettingsController.LoadSettings();
            audioSettingsController.LoadSettings();
            controlsSettingsController.LoadSettings();
        }

        /// <summary>
        /// Applies changes for the current panel.
        /// </summary>
        private void ApplyChanges()
        {
            if (gameSettingsController.gameObject.activeSelf)
            {
                gameSettingsController.SaveSettings();
                resultHandler.HandleResult(OperationResult.SuccessResult("GameSettingsChanged", "Game settings changes applied"));
            }
            else if (videoSettingsController.gameObject.activeSelf)
            {
                videoSettingsController.SaveSettings();
                resultHandler.HandleResult(OperationResult.SuccessResult("VideoSettingsChanged", "Video settings changes applied"));
            }
            else if (audioSettingsController.gameObject.activeSelf)
            {
                audioSettingsController.SaveSettings();
                resultHandler.HandleResult(OperationResult.SuccessResult("AudioSettingsChanged", "Audio settings changes applied"));
            }
            else if (controlsSettingsController.gameObject.activeSelf)
            {
                controlsSettingsController.SaveSettings();
                resultHandler.HandleResult(OperationResult.SuccessResult("ControlsSettingsChanged", "Controls settings changes applied"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Resets the current panel settings to their defaults.
        /// </summary>
        private void ResetToDefaults()
        {
            if (gameSettingsController.gameObject.activeSelf)
            {
                gameSettingsController.ResetToDefaults();
                resultHandler.HandleResult(OperationResult.SuccessResult("GameSettingsReset", "Game settings reset to default"));
            }
            else if (videoSettingsController.gameObject.activeSelf)
            {
                videoSettingsController.ResetToDefaults();
                resultHandler.HandleResult(OperationResult.SuccessResult("VideoSettingsReset", "Video settings reset to default"));
            }
            else if (audioSettingsController.gameObject.activeSelf)
            {
                audioSettingsController.ResetToDefaults();
                resultHandler.HandleResult(OperationResult.SuccessResult("AudioSettingsReset", "Audio settings reset to default"));
            }
            else if (controlsSettingsController.gameObject.activeSelf)
            {
                controlsSettingsController.ResetToDefaults();
                resultHandler.HandleResult(OperationResult.SuccessResult("ControlsSettingsReset", "Controls settings reset to default"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Discards changes for the current panel.
        /// </summary>
        private void DiscardChanges()
        {
            if (gameSettingsController.gameObject.activeSelf)
            {
                gameSettingsController.DiscardChanges();
                resultHandler.HandleResult(OperationResult.WarningResult("GameSettingsDiscarded", "Game settings changes discarded"));
            }
            else if (videoSettingsController.gameObject.activeSelf)
            {
                videoSettingsController.DiscardChanges();
                resultHandler.HandleResult(OperationResult.WarningResult("VideoSettingsDiscarded", "Video settings changes discarded"));
            }
            else if (audioSettingsController.gameObject.activeSelf)
            {
                audioSettingsController.DiscardChanges();
                resultHandler.HandleResult(OperationResult.WarningResult("AudioSettingsDiscarded", "Audio settings changes discarded"));
            }
            else if (controlsSettingsController.gameObject.activeSelf)
            {
                controlsSettingsController.DiscardChanges();
                resultHandler.HandleResult(OperationResult.WarningResult("ControlsSettingsDiscarded", "Controls settings changes discarded"));
            }

            UpdateActionButtonsState();
        }

        /// <summary>
        /// Closes the settings menu.
        /// </summary>
        public override void HidePanel()
        {
            bool hasUnsavedChanges =
                gameSettingsController.HasChanges() ||
                videoSettingsController.HasChanges() ||
                audioSettingsController.HasChanges() ||
                controlsSettingsController.HasChanges();

            if (hasUnsavedChanges)
            {
                // TODO: show confirm dialog before closing

                gameSettingsController.DiscardChanges();
                videoSettingsController.DiscardChanges();
                audioSettingsController.DiscardChanges();
                controlsSettingsController.DiscardChanges();

                resultHandler.HandleResult(OperationResult.WarningResult("SettingsDiscarded", "Settings changes discarded"));
            }

            base.HidePanel();
        }
    }
}