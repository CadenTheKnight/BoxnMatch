using UnityEngine;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options.ToggleSwitch;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    public class VideoSettingsPanelController : BaseSettingsPanel
    {
        [Header("Options")]
        [SerializeField] private ToggleSwitch fullscreenToggle;

        private bool _originalFullscreen;

        private void OnEnable()
        {
            fullscreenToggle.onToggle += OnFullscreenChanged;
        }

        private void OnDisable()
        {
            fullscreenToggle.onToggle -= OnFullscreenChanged;
        }

        public override void LoadSettings()
        {
            _originalFullscreen = PlayerPrefs.GetInt("video_fullscreen") == 1;
            fullscreenToggle.SetState(_originalFullscreen, false);
            if (!IsDefaults()) NotifySettingsChanged();
        }

        public override void DiscardChanges()
        {
            fullscreenToggle.SetState(_originalFullscreen, true);

            ResetChangeTracking();
        }

        public override void ResetToDefaults()
        {
            fullscreenToggle.SetState(false, true);
        }

        public bool IsDefaults()
        {
            return fullscreenToggle.CurrentValue == false;
        }

        public bool HasChanges()
        {
            return fullscreenToggle.CurrentValue != _originalFullscreen;
        }

        private void CheckForChanges()
        {
            if (HasChanges()) NotifySettingsChanged();
            if (!IsDefaults()) NotifySettingsChanged();
        }

        private void OnFullscreenChanged(bool isFullscreen)
        {
            CheckForChanges();
        }

        public override void ApplyChanges()
        {
            Screen.fullScreen = fullscreenToggle.CurrentValue;

            PlayerPrefs.SetInt("video_fullscreen", fullscreenToggle.CurrentValue ? 1 : 0);
            PlayerPrefs.Save();

            _originalFullscreen = fullscreenToggle.CurrentValue;

            ResetChangeTracking();
        }
    }
}