using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Game.UI.Components.ToggleSwitch
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [Header("Toggle Components")]
        [SerializeField] private RectTransform handleRect;
        [SerializeField] private RectTransform backgroundRect;

        [Header("Toggle State")]
        [SerializeField] private bool startOn = false;
        protected float sliderValue;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDurationSeconds = 0.25f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Events")]
        public Action<bool> onToggle;
        protected Action transitionEffect;

        public bool CurrentValue { get; private set; }
        private ToggleSwitchGroupManager _toggleSwitchGroupManager;
        private Coroutine _animationCoroutine;
        private Vector2 _offPosition;
        private Vector2 _onPosition;

        private void OnRectTransformDimensionsChange()
        {
            if (Application.isPlaying)
            {
                InitializePositions();
                UpdateHandlePosition(CurrentValue ? 1f : 0f);
            }
        }

        protected virtual void Awake()
        {
            InitializePositions();
            CurrentValue = startOn;

            UpdateHandlePosition(startOn ? 1f : 0f);
            sliderValue = startOn ? 1f : 0f;
        }

        private void InitializePositions()
        {
            _offPosition = new Vector2(0, 0);
            _onPosition = new Vector2(backgroundRect.rect.width - handleRect.rect.width, 0);
        }

        public void SetupForManager(ToggleSwitchGroupManager manager)
        {
            _toggleSwitchGroupManager = manager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        public void Toggle()
        {
            if (_toggleSwitchGroupManager != null)
                _toggleSwitchGroupManager.ToggleGroup(this);
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

            onToggle?.Invoke(CurrentValue);

            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            _animationCoroutine = StartCoroutine(AnimateToggle());
        }

        private IEnumerator AnimateToggle()
        {
            float startValue = sliderValue;
            float endValue = CurrentValue ? 1f : 0f;
            float startTime = Time.time;

            while (Time.time < startTime + animationDurationSeconds)
            {
                float elapsed = Time.time - startTime;
                float t = elapsed / animationDurationSeconds;
                float easedT = slideEase.Evaluate(t);

                sliderValue = Mathf.Lerp(startValue, endValue, easedT);
                UpdateHandlePosition(sliderValue);

                transitionEffect?.Invoke();

                yield return null;
            }

            sliderValue = endValue;
            UpdateHandlePosition(sliderValue);
            transitionEffect?.Invoke();

            _animationCoroutine = null;
        }

        private void UpdateHandlePosition(float value)
        {
            if (handleRect != null)
                handleRect.anchoredPosition = Vector2.Lerp(_offPosition, _onPosition, value);
        }
    }
}