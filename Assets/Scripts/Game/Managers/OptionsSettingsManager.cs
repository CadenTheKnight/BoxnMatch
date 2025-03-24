using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Game.UI.Controllers.SettingsMenu;

namespace Assets.Scripts.Game.Managers
{
    public class OptionsSettingsManager : Singleton<OptionsSettingsManager>
    {
        [Header("Menu References")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Scene Settings")]
        [SerializeField] private string[] disabledMenuScenes = { "Initialization", "Loading" };

        protected override void Awake()
        {
            base.Awake();
            HideAllMenus();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool shouldDisableMenus = ShouldDisableMenusInScene(scene.name);
            UpdateMenuVisibility(shouldDisableMenus);
        }

        private bool ShouldDisableMenusInScene(string sceneName)
        {
            foreach (string disabledScene in disabledMenuScenes)
                if (sceneName == disabledScene)
                    return true;
            return false;
        }

        private void UpdateMenuVisibility(bool shouldDisableMenus)
        {
            if (shouldDisableMenus)
                HideAllMenus();
            else
            {
                ShowOptionsMenu();
                HideSettingsPanel();
            }
        }

        public void ShowOptionsMenu()
        {
            optionsPanel.SetActive(true);
        }

        public void HideOptionsMenu()
        {
            optionsPanel.SetActive(false);
        }

        public void ShowSettingsPanel()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (!ShouldDisableMenusInScene(currentScene))
            {
                settingsPanel.SetActive(true);
                SettingsPanelController settingsController = settingsPanel.GetComponent<SettingsPanelController>();
                settingsController.ShowPanel();
            }
        }

        public void HideSettingsPanel()
        {
            settingsPanel.SetActive(false);
        }

        public void HideAllMenus()
        {
            HideOptionsMenu();
            HideSettingsPanel();
        }
    }
}