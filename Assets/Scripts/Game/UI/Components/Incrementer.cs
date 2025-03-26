using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components
{
    public class Incrementer : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] public Button decrementButton;
        [SerializeField] public Button incrementButton;
        [SerializeField] private TextMeshProUGUI valueText;

        [Header("Incremeneter Settings")]
        [SerializeField] public int minValue = 1;
        [SerializeField] public int maxValue = 99;
        [SerializeField] private int startValue = 1;
        [SerializeField] private int stepSize = 2;

        public System.Action<int> onValueChanged;

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
            decrementButton.interactable = Value > minValue;
            incrementButton.interactable = Value < maxValue;
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
    }
}