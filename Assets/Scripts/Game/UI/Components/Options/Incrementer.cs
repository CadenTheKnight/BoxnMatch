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

        private bool interactable = true;

        public Action<int> onValueChanged;

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
            if (Value > minValue)
            {
                Value -= stepSize;
                UpdateUI();
                onValueChanged?.Invoke(Value);
            }
        }

        private void OnIncrementClicked()
        {
            if (Value < maxValue)
            {
                Value += stepSize;
                UpdateUI();
                onValueChanged?.Invoke(Value);
            }
        }

        private void UpdateUI()
        {
            valueText.text = Value.ToString();
            if (interactable)
            {
                decrementButton.interactable = Value > minValue;
                incrementButton.interactable = Value < maxValue;
            }
        }

        public void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, minValue, maxValue);

            if (Value != newValue)
            {
                Value = newValue;
                UpdateUI();
                onValueChanged?.Invoke(Value);
            }
        }

        public void DisableInteraction()
        {
            interactable = false;
            decrementButton.interactable = false;
            incrementButton.interactable = false;
        }

        public void EnableInteraction()
        {
            interactable = true;
            decrementButton.interactable = true;
            incrementButton.interactable = true;
        }
    }
}