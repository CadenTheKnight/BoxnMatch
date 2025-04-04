using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    public class AudioSettingsPanelController : BaseSettingsPanel
    {
        // [Header("Options")]
        // [SerializeField] private Slider masterVolumeSlider;
        // [SerializeField] private Slider musicVolumeSlider;
        // [SerializeField] private Slider sfxVolumeSlider;
        // [SerializeField] private Toggle muteToggle;

        // [SerializeField] private TextMeshProUGUI masterVolumeText;
        // [SerializeField] private TextMeshProUGUI musicVolumeText;
        // [SerializeField] private TextMeshProUGUI sfxVolumeText;

        // private float _originalMasterVolume;
        // private float _originalMusicVolume;
        // private float _originalSfxVolume;
        // private bool _originalMute;

        private void OnEnable()
        {
            // masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            // musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            // sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            // muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
        }

        private void OnDisable()
        {
            // masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            // musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            // sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            // muteToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);
        }

        public override void LoadSettings()
        {
            // _originalMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            // _originalMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            // _originalSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
            // _originalMute = PlayerPrefs.GetInt("AudioMuted", 0) == 1;

            // masterVolumeSlider.SetValueWithoutNotify(_originalMasterVolume);
            // musicVolumeSlider.SetValueWithoutNotify(_originalMusicVolume);
            // sfxVolumeSlider.SetValueWithoutNotify(_originalSfxVolume);
            // muteToggle.SetIsOnWithoutNotify(_originalMute);

            UpdateVolumeTexts();

            ResetChangeTracking();
        }

        public override void ApplyChanges()
        {
            // PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            // PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
            // PlayerPrefs.SetFloat("SfxVolume", sfxVolumeSlider.value);
            // PlayerPrefs.SetInt("AudioMuted", muteToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();

            ApplyAudioSettings();

            // _originalMasterVolume = masterVolumeSlider.value;
            // _originalMusicVolume = musicVolumeSlider.value;
            // _originalSfxVolume = sfxVolumeSlider.value;
            // _originalMute = muteToggle.isOn;

            ResetChangeTracking();
        }

        public override void DiscardChanges()
        {
            // masterVolumeSlider.SetValueWithoutNotify(_originalMasterVolume);
            // musicVolumeSlider.SetValueWithoutNotify(_originalMusicVolume);
            // sfxVolumeSlider.SetValueWithoutNotify(_originalSfxVolume);
            // muteToggle.SetIsOnWithoutNotify(_originalMute);

            UpdateVolumeTexts();

            ApplyAudioSettings();

            ResetChangeTracking();
        }

        public override void ResetToDefaults()
        {

        }

        public bool IsDefaults()
        {
            return true;
        }

        public bool HasChanges()
        {
            return false;
        }


        private void CheckForChanges()
        {
            if (HasChanges()) NotifySettingsChanged();
            if (!IsDefaults()) NotifySettingsChanged();
        }

        private void OnMasterVolumeChanged(float value)
        {
            // masterVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            CheckForChanges();
        }

        private void OnMusicVolumeChanged(float value)
        {
            // musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            CheckForChanges();
        }

        private void OnSfxVolumeChanged(float value)
        {
            // sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            CheckForChanges();
        }

        private void OnMuteToggleChanged(bool value)
        {
            CheckForChanges();
        }

        private void UpdateVolumeTexts()
        {
            // masterVolumeText.text = $"{Mathf.RoundToInt(masterVolumeSlider.value * 100)}%";
            // musicVolumeText.text = $"{Mathf.RoundToInt(musicVolumeSlider.value * 100)}%";
            // sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
        }

        private void ApplyAudioSettings()
        {
            // AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
            // AudioManager.Instance.SetSfxVolume(sfxVolumeSlider.value);
        }
    }
}