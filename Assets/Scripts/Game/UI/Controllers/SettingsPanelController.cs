using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Game.UI.Components;
using Unity.Services.Authentication;
using Assets.Scripts.Framework.Managers;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class SettingsPanelController : BasePanel
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Button deletePlayerDataButton;
        private bool isFullscreen;

        public override void ShowPanel()
        {
            isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.isOn = isFullscreen;
            base.ShowPanel();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            saveButton.onClick.AddListener(OnSaveClicked);
            fullscreenToggle.onValueChanged.AddListener(OnToggleValueChanged);
            deletePlayerDataButton.onClick.AddListener(OnDeletePlayerDataClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            saveButton.onClick.RemoveListener(OnSaveClicked);
            fullscreenToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            deletePlayerDataButton.onClick.RemoveListener(OnDeletePlayerDataClicked);
        }

        private void OnToggleValueChanged(bool isFullscreen)
        {
            this.isFullscreen = isFullscreen;
        }

        private void OnSaveClicked()
        {
            SaveSettings();
            base.HidePanel();
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

            SceneManager.LoadScene("Initialize");
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();

            Screen.fullScreen = isFullscreen;
        }
    }
}

