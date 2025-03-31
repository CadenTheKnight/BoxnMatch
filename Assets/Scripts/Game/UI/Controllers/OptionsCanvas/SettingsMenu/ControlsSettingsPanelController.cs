using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu
{
    public class ControlsSettingsPanelController : BaseSettingsPanel
    {
        [Header("Options")]
        [SerializeField] private Button deletePlayerPrefsButton;


        private void OnEnable()
        {
            deletePlayerPrefsButton.onClick.AddListener(OnDeletePlayerPrefsClicked);
        }

        private void OnDisable()
        {
            deletePlayerPrefsButton.onClick.RemoveListener(OnDeletePlayerPrefsClicked);
        }

        private void OnDeletePlayerPrefsClicked()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            // sign out and delete player data
            AuthenticationService.Instance.SignOut();
            SceneManager.LoadScene("Initialization", LoadSceneMode.Single);
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