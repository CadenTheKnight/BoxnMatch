using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components.Options
{
    public class Incrementer : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button decrementButton;
        [SerializeField] private Button incrementButton;
        [SerializeField] private TextMeshProUGUI valueText;

        [Header("Incremeneter Settings")]
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 99;
        [SerializeField] private int startValue = 1;
        [SerializeField] private int stepSize = 2;
        [SerializeField] private bool allowLooping = false;

        private bool interactable = true;

        public Action<int> OnValueChanged;

        public int Value { get; private set; }

        private void Awake()
        {
            Value = startValue;
            UpdateUI();
        }

        private void OnEnable()
        {
            decrementButton.onClick.AddListener(OnDecrementClicked);
            incrementButton.onClick.AddListener(OnIncrementClicked);
        }

        private void OnDisable()
        {
            decrementButton.onClick.RemoveListener(OnDecrementClicked);
            incrementButton.onClick.RemoveListener(OnIncrementClicked);
        }

        private void OnDecrementClicked()
        {
            if (Value > minValue) Value -= stepSize;
            else if (allowLooping) Value = maxValue;

            UpdateUI();
            OnValueChanged?.Invoke(Value);
        }

        private void OnIncrementClicked()
        {
            if (Value < maxValue) Value += stepSize;
            else if (allowLooping) Value = minValue;

            UpdateUI();
            OnValueChanged?.Invoke(Value);
        }

        private void UpdateButtons()
        {
            decrementButton.interactable = Value > minValue && interactable;
            incrementButton.interactable = Value < maxValue && interactable;
        }

        private void UpdateUI()
        {
            valueText.text = Value.ToString();
            UpdateButtons();
        }

        public void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, minValue, maxValue);

            if (Value != newValue)
            {
                Value = newValue;
                UpdateUI();
                OnValueChanged?.Invoke(Value);
            }
        }

        public void UpdateInteractable(bool isInteractable)
        {
            interactable = isInteractable;
            UpdateButtons();
        }
    }
}