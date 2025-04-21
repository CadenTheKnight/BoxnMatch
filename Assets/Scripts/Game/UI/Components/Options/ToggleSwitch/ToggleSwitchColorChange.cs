using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components.Options.ToggleSwitch
{
    public class ToggleSwitchColorChange : ToggleSwitch
    {
        [Header("Toggle Switch Components")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image handleImage;

        [Header("Options")]
        [SerializeField] private bool changeBackgroundColor;
        [SerializeField] private bool changeHandleColor;

        [Header("Colors")]
        [SerializeField] private Color onBackgroundColor;
        [SerializeField] private Color offBackgroundColor;
        [SerializeField] private Color onHandleColor;
        [SerializeField] private Color offHandleColor;

        private void OnEnable()
        {
            changeColors += ChangeColors;
            transitionEffect += TransitionColors;

        }

        private void OnDisable()
        {
            changeColors -= ChangeColors;
            transitionEffect -= TransitionColors;
        }

        private void ChangeColors(float sliderValue)
        {
            handleImage.color = sliderValue == 1 ? onHandleColor : offHandleColor;
            backgroundImage.color = sliderValue == 1 ? onBackgroundColor : offBackgroundColor;
        }

        private void TransitionColors()
        {
            handleImage.color = Color.Lerp(offHandleColor, onHandleColor, sliderValue);
            backgroundImage.color = Color.Lerp(offBackgroundColor, onBackgroundColor, sliderValue);
        }

        public override void UpdateInteractable(bool isInteractable)
        {
            base.UpdateInteractable(isInteractable);
            handleImage.color = isInteractable ? sliderValue == 1 ? onHandleColor : offHandleColor : offHandleColor;
            backgroundImage.color = isInteractable ? sliderValue == 1 ? onBackgroundColor : offBackgroundColor : offBackgroundColor;
        }
    }
}