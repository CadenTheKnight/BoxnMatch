using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI
{
    public class LobbySettingsPanelController : BasePanelController
    {
        [SerializeField] private Button saveButton;

        public override void ShowPanel()
        {
            base.ShowPanel();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        }

        private void OnSaveButtonClicked()
        {
            HidePanel();
        }
    }
}
