using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI
{
    public class SettingsPanelController : BasePanelController
    {
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Button saveButton;
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
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);
            saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        }

        private void OnFullscreenToggleChanged(bool isFullscreen)
        {
            this.isFullscreen = isFullscreen;
        }

        private void OnSaveButtonClicked()
        {
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();

            Screen.fullScreen = isFullscreen;
            HidePanel();
        }
    }
}
