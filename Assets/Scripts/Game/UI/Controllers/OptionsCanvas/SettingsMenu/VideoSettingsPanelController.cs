using System;
using UnityEngine;
using Assets.Scripts.Game.UI.Components.Options.ToggleSwitch;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    public class VideoSettingsPanelController : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private ToggleSwitch fullscreenToggle;

        public Action OnVideoSettingsChanged;

        private bool _originalFullscreen;

        private void OnEnable()
        {
            fullscreenToggle.onToggle += OnFullscreenChanged;
        }

        private void OnDisable()
        {
            fullscreenToggle.onToggle -= OnFullscreenChanged;
        }

        public void LoadSettings()
        {
            _originalFullscreen = PlayerPrefs.GetInt("video_fullscreen") == 1;
            fullscreenToggle.SetState(_originalFullscreen, false);
            OnVideoSettingsChanged?.Invoke();
        }

        public void DiscardChanges()
        {
            fullscreenToggle.SetState(_originalFullscreen, false);
            OnVideoSettingsChanged?.Invoke();
        }

        public void ResetToDefaults()
        {
            fullscreenToggle.SetState(false, false);
            OnVideoSettingsChanged?.Invoke();
        }

        public bool IsDefaults()
        {
            return fullscreenToggle.CurrentValue == false;
        }

        public bool HasChanges()
        {
            return fullscreenToggle.CurrentValue != _originalFullscreen;
        }

        private void OnFullscreenChanged(bool isFullscreen)
        {
            OnVideoSettingsChanged?.Invoke();
        }

        public void ApplyChanges()
        {
            Screen.fullScreen = fullscreenToggle.CurrentValue;

            PlayerPrefs.SetInt("video_fullscreen", fullscreenToggle.CurrentValue ? 1 : 0);
            PlayerPrefs.Save();

            _originalFullscreen = fullscreenToggle.CurrentValue;

            OnVideoSettingsChanged?.Invoke();
        }
    }
}