using UnityEngine;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel
{
    public class MatchSettingsPanelController : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private MapChanger mapChanger;
        [SerializeField] private Incrementer roundCountIncrementer;
        [SerializeField] private Incrementer roundTimeIncrementer;
        [SerializeField] private Selector gameModeSelector;

        public MapChanger MapChanger => mapChanger;
        public Incrementer RoundCountIncrementer => roundCountIncrementer;
        public Incrementer RoundTimeIncrementer => roundTimeIncrementer;
        public Selector GameModeSelector => gameModeSelector;

        private void Start()
        {
            mapChanger.SetValue(0);
            roundCountIncrementer.SetValue(5);
            roundTimeIncrementer.SetValue(90);
            gameModeSelector.SetSelection(0);
        }
    }
}