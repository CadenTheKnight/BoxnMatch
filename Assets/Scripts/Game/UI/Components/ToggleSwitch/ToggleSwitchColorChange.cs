using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components.ToggleSwitch
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

        private bool _isBackgroundImageNotNull;
        private bool _isHandleImageNotNull;

        private void OnEnable()
        {
            transitionEffect += ChangeColors;
        }

        private void OnDisable()
        {
            transitionEffect -= ChangeColors;
        }

        protected override void Awake()
        {
            base.Awake();

            CheckForNull();
            ChangeColors();
        }

        private void CheckForNull()
        {
            _isBackgroundImageNotNull = backgroundImage != null;
            _isHandleImageNotNull = handleImage != null;
        }

        private void ChangeColors()
        {
            if (_isBackgroundImageNotNull && changeBackgroundColor)
                backgroundImage.color = Color.Lerp(offBackgroundColor, onBackgroundColor, sliderValue);

            if (_isHandleImageNotNull && changeHandleColor)
                handleImage.color = Color.Lerp(offHandleColor, onHandleColor, sliderValue);
        }
    }
}