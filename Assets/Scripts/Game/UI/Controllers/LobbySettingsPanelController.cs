using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers
{
    public class LobbySettingsPanelController : BasePanel
    {
        [SerializeField] private Button saveButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            saveButton.onClick.AddListener(SaveSettings);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            saveButton.onClick.RemoveListener(SaveSettings);
        }

        private void SaveSettings()
        {
            // Save settings

            HidePanel();
        }
    }
}
