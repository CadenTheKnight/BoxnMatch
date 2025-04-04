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


        private void ApplyGameSettings()
        {

        }
    }
}