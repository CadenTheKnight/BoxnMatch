using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options.ToggleSwitch;

namespace Assets.Scripts.Game.UI.Controllers.SettingsCanvas
{
    public class SettingsPanelController : BasePanel
    {
        [Header("UI Components")]
        [SerializeField] private ToggleSwitch fullscreenToggle;
        [SerializeField] private ToggleSwitch screenShakeToggle;
        [SerializeField] private Slider volumeSlider;
        // [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Button applyChangesButton;
        [SerializeField] private Button discardChangesButton;
        [SerializeField] private Button resetToDefaultsButton;

        // public static event Action<OperationResult> OnSettingsUpdated;

        private bool _originalFullscreen;
        private bool _originalScreenShake;
        private float _originalVolume;
        // private Resolution _originalResolution;


        protected override void OnEnable()
        {
            base.OnEnable();
            LoadSettings();

            fullscreenToggle.onToggle += OnFullscreenChanged;
            screenShakeToggle.onToggle += OnScreenShakeChanged;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            // resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

            applyChangesButton.onClick.AddListener(ApplyChanges);
            discardChangesButton.onClick.AddListener(DiscardChanges);
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            fullscreenToggle.onToggle -= OnFullscreenChanged;
            screenShakeToggle.onToggle -= OnScreenShakeChanged;
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            // resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

            applyChangesButton.onClick.RemoveListener(ApplyChanges);
            discardChangesButton.onClick.RemoveListener(DiscardChanges);
            resetToDefaultsButton.onClick.RemoveListener(ResetToDefaults);
        }

        public void LoadSettings()
        {
            _originalFullscreen = PlayerPrefs.GetInt("video_fullscreen") == 1;
            _originalScreenShake = PlayerPrefs.GetInt("video_screenShake") == 1;
            _originalVolume = PlayerPrefs.GetFloat("audio_volume", 1.0f);
            // _originalResolution = Screen.currentResolution;

            fullscreenToggle.SetState(_originalFullscreen, false);
            screenShakeToggle.SetState(_originalScreenShake, false);
            volumeSlider.value = _originalVolume;
            // resolutionDropdown.value = Array.IndexOf(Screen.resolutions, _originalResolution);
        }

        public bool IsDefaults()
        {
            return fullscreenToggle.CurrentValue == false &&
                   screenShakeToggle.CurrentValue == false &&
                   volumeSlider.value == 0.5f;
        }

        public bool HasChanges()
        {
            return fullscreenToggle.CurrentValue != _originalFullscreen ||
                   screenShakeToggle.CurrentValue != _originalScreenShake ||
                   volumeSlider.value != _originalVolume;
        }

        private void OnFullscreenChanged(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            UpdateActionButtonsState();
        }

        private void OnScreenShakeChanged(bool isEnabled)
        {
            UpdateActionButtonsState();
        }

        private void OnVolumeChanged(float volume)
        {
            AudioListener.volume = volume;
            UpdateActionButtonsState();
        }

        // private void OnResolutionChanged(int resolutionIndex)
        // {
        //     Resolution selectedResolution = Screen.resolutions[resolutionIndex];
        //     Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        //     UpdateActionButtonsState();
        // }

        private void UpdateActionButtonsState()
        {
            resetToDefaultsButton.interactable = !IsDefaults();
            applyChangesButton.interactable = HasChanges();
            discardChangesButton.interactable = HasChanges();
        }

        private void ResetToDefaults()
        {
            fullscreenToggle.SetState(false, false);
            screenShakeToggle.SetState(false, false);
            volumeSlider.value = 0.5f;
            // resolutionDropdown.value = 0;

            // OnSettingsUpdated?.Invoke(OperationResult.WarningResult("SettingsReset", "Settings reset to defaults"));
            UpdateActionButtonsState();
        }

        private void DiscardChanges()
        {
            fullscreenToggle.SetState(_originalFullscreen, false);
            screenShakeToggle.SetState(_originalScreenShake, false);
            volumeSlider.value = _originalVolume;
            // resolutionDropdown.value = Array.IndexOf(Screen.resolutions, _originalResolution);

            // OnSettingsUpdated?.Invoke(OperationResult.WarningResult("SettingsDiscarded", "Settings changes discarded"));
            UpdateActionButtonsState();
        }

        private void ApplyChanges()
        {
            PlayerPrefs.SetInt("video_fullscreen", fullscreenToggle.CurrentValue ? 1 : 0);
            PlayerPrefs.SetInt("video_screenShake", screenShakeToggle.CurrentValue ? 1 : 0);
            PlayerPrefs.SetFloat("audio_volume", volumeSlider.value);
            // PlayerPrefs.SetInt("video_resolution", resolutionDropdown.value);
            PlayerPrefs.Save();

            _originalFullscreen = fullscreenToggle.CurrentValue;
            _originalScreenShake = screenShakeToggle.CurrentValue;
            _originalVolume = volumeSlider.value;
            // _originalResolution = Screen.resolutions[resolutionDropdown.value];

            // OnSettingsUpdated?.Invoke(OperationResult.SuccessResult("SettingsApplied", "Settings changes applied successfully"));
            UpdateActionButtonsState();
        }

        public override void HidePanel()
        {
            DiscardChanges();
            base.HidePanel();
        }
    }
}