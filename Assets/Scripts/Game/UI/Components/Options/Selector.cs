using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Colors;

namespace Assets.Scripts.Game.UI.Components.Options
{
    public class Selector : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> selectionButtons = new();

        [Header("Selector Settings")]
        [SerializeField] private bool allowDeselection = true;

        [Header("Color Settings")]
        [SerializeField] private Color selectedColor = UIColors.greenDefaultColor;
        [SerializeField] private Color selectedHoverColor = UIColors.greenHoverColor;
        [SerializeField] private Color unselectedColor = UIColors.grayDefaultColor;
        [SerializeField] private Color unselectedHoverColor = UIColors.primaryPressedColor;
        [SerializeField] private Color selectedDisabledColor = UIColors.primaryPressedColor;
        [SerializeField] private Color unselectedDisabledColor = UIColors.secondaryDisabledColor;

        public Color SelectedColor
        {
            get => selectedColor;
            set { selectedColor = value; UpdateButtons(); }
        }

        public Color SelectedHoverColor
        {
            get => selectedHoverColor;
            set { selectedHoverColor = value; UpdateButtons(); }
        }

        public Color UnselectedColor
        {
            get => unselectedColor;
            set { unselectedColor = value; UpdateButtons(); }
        }

        public Color UnselectedHoverColor
        {
            get => unselectedHoverColor;
            set { unselectedHoverColor = value; UpdateButtons(); }
        }

        public Color SelectedDisabledColor
        {
            get => selectedDisabledColor;
            set { selectedDisabledColor = value; UpdateButtons(); }
        }

        public Color UnselectedDisabledColor
        {
            get => unselectedDisabledColor;
            set { unselectedDisabledColor = value; UpdateButtons(); }
        }

        public Action<int> onSelectionChanged;

        public int Selection { get; private set; } = -1;

        private void Awake()
        {
            foreach (Button selectionButton in selectionButtons)
                selectionButton.onClick.AddListener(() => OnButtonClicked(selectionButtons.IndexOf(selectionButton)));
        }

        private void OnDestroy()
        {
            foreach (Button selectionButton in selectionButtons)
                selectionButton.onClick.RemoveListener(() => OnButtonClicked(selectionButtons.IndexOf(selectionButton)));
        }

        private void Select(int index)
        {
            if (index < 0 || index >= selectionButtons.Count) return;
            Selection = index;
        }

        public void Clear()
        {
            if (Selection == -1 || !allowDeselection) return;
            Selection = -1;
        }

        public void SetSelection(int index, bool selected)
        {
            if (selected && index != Selection) Select(index);
            else if (!selected && index == Selection) Clear();
            UpdateButtons();
            onSelectionChanged?.Invoke(Selection);
        }

        private void OnButtonClicked(int index)
        {
            if (Selection != index) Select(index);
            else if (allowDeselection) Clear();
            UpdateButtons();
            onSelectionChanged?.Invoke(Selection);
        }

        private void UpdateButtons()
        {
            foreach (Button selectionButton in selectionButtons)
            {
                bool selected = selectionButtons.IndexOf(selectionButton) == Selection;

                ColorBlock colors = selectionButton.colors;

                colors.normalColor = selected ? selectedColor : unselectedColor;
                colors.selectedColor = selected ? selectedColor : unselectedColor;
                colors.highlightedColor = selected ? selectedHoverColor : unselectedHoverColor;
                colors.pressedColor = selected ? selectedHoverColor : unselectedHoverColor;
                colors.disabledColor = selected ? selectedDisabledColor : unselectedDisabledColor;

                selectionButton.colors = colors;
            }
        }

        public void UpdateInteractable(bool isInteractable)
        {
            foreach (Button selectionButton in selectionButtons)
                selectionButton.interactable = isInteractable;
        }
    }
}