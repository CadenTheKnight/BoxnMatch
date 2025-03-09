using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.SettingsMenu
{
    public class GameSettingsPanelController : BaseSettingsPanel
    {
        [Header("UI References")]
        [SerializeField] private Button deletePlayerDataButton;

        private void OnEnable()
        {
            deletePlayerDataButton.onClick.AddListener(OnDeletePlayerDataClicked);
        }

        private void OnDisable()
        {
            deletePlayerDataButton.onClick.RemoveListener(OnDeletePlayerDataClicked);
        }

        private void OnDeletePlayerDataClicked()
        {
            try
            {
                AuthenticationService.Instance.SignOut();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during sign out: {ex.Message}");
            }

            Debug.Log($"Deleting all player data for {PlayerPrefs.GetString("PlayerName")}");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            SceneManager.LoadScene("Initialization");
        }

        public override void LoadSettings()
        {
            ResetChangeTracking();
        }

        public override void DiscardChanges()
        {
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

        public override void ApplyChanges()
        {
            PlayerPrefs.Save();

            ResetChangeTracking();
        }
    }
}