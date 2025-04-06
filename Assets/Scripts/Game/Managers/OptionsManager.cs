using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Framework.Core;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.OptionsMenu;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.SettingsMenu;

namespace Assets.Scripts.Game.Managers
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        [Header("Menu References")]
        [SerializeField] private OptionsPanelController optionsPanelController;
        [SerializeField] private SettingsPanelController settingsPanelController;

        [Header("Scene Settings")]
        [SerializeField] private string[] disabledMenuScenes = { "Initialization", "Loading" };

        protected override void Awake()
        {
            base.Awake();
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
            optionsPanelController.gameObject.SetActive(!shouldDisableMenus);
            if (shouldDisableMenus) settingsPanelController.gameObject.SetActive(false);
        }
    }
}