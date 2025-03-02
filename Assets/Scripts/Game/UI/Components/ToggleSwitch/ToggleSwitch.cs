using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Assets.Scripts.Game.UI.Components.ToggleSwitch
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [Header("Slider")]
        [SerializeField, Range(0, 1f)] private float sliderValue;
        public bool CurrentValue { get; private set; }
        private Slider _slider;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDurationSeconds = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(timeStart: 0, valueStart: 0, timeEnd: 1, valueEnd: 1);
        private Coroutine _animationSliderCoroutine;

        [Header("Events")]
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;
        private ToggleSwitchGroupManager _toggleSwitchGroupManager;

        protected void OnValidate()
        {
            SetUpToggleComponents();

            _slider.value = sliderValue;
        }

        private void SetUpToggleComponents()
        {
            if (_slider == null)
                return;

            SetUpSliderComponent();
        }

        private void SetUpSliderComponent()
        {
            _slider = GetComponent<Slider>();

            if (_slider == null)
                return;

            _slider.interactable = false;
            var sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        public void SetupForManager(ToggleSwitchGroupManager manager)
        {
            _toggleSwitchGroupManager = manager;
        }

        private void Awake()
        {
            SetUpToggleComponents();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        private void Toggle()
        {
            if (_toggleSwitchGroupManager != null)
                _toggleSwitchGroupManager.ToggleGroup(toggleSwitch: this);
            else
                SetStateAndStartAnimation(!CurrentValue);
        }

        public void ToggleByGroupManager(bool valueToSetTo)
        {
            SetStateAndStartAnimation(valueToSetTo);
        }

        private void SetStateAndStartAnimation(bool state)
        {
            if (CurrentValue == state)
                return;

            CurrentValue = state;

            if (CurrentValue)
                onToggleOn.Invoke();
            else
                onToggleOff.Invoke();

            if (_animationSliderCoroutine != null)
                StopCoroutine(_animationSliderCoroutine);

            _animationSliderCoroutine = StartCoroutine(routine: AnimateSlider());
        }

        private IEnumerator AnimateSlider()
        {
            float startValue = _slider.value;
            float endValue = CurrentValue ? 1 : 0;

            float time = 0;
            if (animationDurationSeconds > 0)
            {
                while (time < animationDurationSeconds)
                {
                    time += Time.deltaTime;

                    float lerpFactor = slideEase.Evaluate(time / animationDurationSeconds);
                    _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    yield return null;
                }
            }

            _slider.value = endValue;
        }
    }
}
