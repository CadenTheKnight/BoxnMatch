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

        [Header("Appearance")]
        [SerializeField] private Color selectedColor = UIColors.primaryHoverColor;
        [SerializeField] private Color unselectedColor = UIColors.primaryDefaultColor;

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

        public void ClearSelection()
        {
            if (Selection == -1 || !allowDeselection) return;
            Selection = -1;
            UpdateButtonAppearance();
            onSelectionChanged?.Invoke(Selection);
        }

        public void SetSelection(int index, bool selected)
        {
            if (selected && index != Selection) HandleNewSelection(index);
            else if (!selected && index == Selection) ClearSelection();
        }

        private void OnButtonClicked(int index)
        {
            if (index == Selection) ClearSelection();
            else HandleNewSelection(index);
        }

        private void HandleNewSelection(int index)
        {
            if (index < 0 || index >= selectionButtons.Count) return;
            Selection = index;
            UpdateButtonAppearance();
            onSelectionChanged?.Invoke(Selection);
        }
        private void UpdateButtonAppearance()
        {
            foreach (Button selectionButton in selectionButtons)
            {
                ColorBlock colors = selectionButton.colors;

                bool isSelected = selectionButton == selectionButtons[Selection];

                colors.normalColor = isSelected ? selectedColor : unselectedColor;
                colors.selectedColor = isSelected ? selectedColor : colors.highlightedColor;
                colors.disabledColor = isSelected ? selectedColor : unselectedColor;

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