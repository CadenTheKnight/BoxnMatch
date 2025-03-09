using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.UI.Components.ToggleSwitch
{
    public class ToggleSwitchGroupManager : MonoBehaviour
    {
        [Header("Start Value")]
        [SerializeField] private ToggleSwitch initialToggleSwitch;

        [Header("Toggle Options")]
        [SerializeField] private bool allCanBeToggledOff;

        private List<ToggleSwitch> _toggleSwitches = new();

        private void Awake()
        {
            ToggleSwitch[] toggleSwitches = GetComponentsInChildren<ToggleSwitch>();
            foreach (ToggleSwitch _toggleSwitch in toggleSwitches)
                RegisterToggleButtonToGroup(_toggleSwitch);
        }

        private void RegisterToggleButtonToGroup(ToggleSwitch toggleSwitch)
        {
            if (_toggleSwitches.Contains(toggleSwitch))
                return;

            _toggleSwitches.Add(toggleSwitch);

            toggleSwitch.SetupForManager(this);
        }

        private void Start()
        {
            bool areAllToggledOff = true;
            foreach (var button in _toggleSwitches)
            {
                if (!button.CurrentValue)
                    continue;

                areAllToggledOff = false;
                break;
            }

            if (!areAllToggledOff || allCanBeToggledOff)
                return;

            if (initialToggleSwitch != null)
                initialToggleSwitch.ToggleByGroupManager(valueToSetTo: true);
            else
                _toggleSwitches[0].ToggleByGroupManager(valueToSetTo: true);
        }

        public void ToggleGroup(ToggleSwitch toggleSwitch)
        {
            if (_toggleSwitches.Count <= 1)
                return;

            if (allCanBeToggledOff && toggleSwitch.CurrentValue)
            {
                foreach (var button in _toggleSwitches)
                {
                    if (button == null)
                        continue;

                    button.ToggleByGroupManager(valueToSetTo: false);
                }
            }
            else
            {
                foreach (var button in _toggleSwitches)
                {
                    if (button == null)
                        continue;

                    if (button == toggleSwitch)
                        button.ToggleByGroupManager(valueToSetTo: true);
                    else
                        button.ToggleByGroupManager(valueToSetTo: false);
                }
            }
        }
    }
}