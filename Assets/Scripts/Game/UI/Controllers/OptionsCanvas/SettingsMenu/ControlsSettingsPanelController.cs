using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    public class ControlsSettingsPanelController : BaseSettingsPanel
    {
        // [Header("Options")]


        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public override void LoadSettings()
        {
            ResetChangeTracking();
        }

        public override void ApplyChanges()
        {
            PlayerPrefs.Save();

            ApplyGameSettings();

            ResetChangeTracking();
        }

        public override void DiscardChanges()
        {
            ApplyGameSettings();

            ResetChangeTracking();
        }

        public override void ResetToDefaults()
        {
            CheckForChanges();
        }

        public override bool HasChanges()
        {
            return _hasChanges;
        }

        private void CheckForChanges()
        {
            bool hasChanges = HasChanges();
            if (hasChanges != _hasChanges)
            {
                _hasChanges = hasChanges;
                NotifySettingsChanged();
            }
        }


        private void ApplyGameSettings()
        {

        }
    }
}