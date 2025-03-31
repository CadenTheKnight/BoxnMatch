using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Colors;
using UnityEngine.Events;

namespace Assets.Scripts.Game.UI.Components.Options
{
    public class Selector : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> selectionButtons = new();

        [Header("Selector Settings")]
        [SerializeField] private SelectionMode selectionMode = SelectionMode.SingleSelection;
        [SerializeField] private bool allowDeselection = true;

        [Header("Appearance")]
        [SerializeField] private Color selectedColor = UIColors.primaryHoverColor;
        [SerializeField] private Color unselectedColor = UIColors.primaryDefaultColor;

        public Action<List<int>> onSelectionChanged;
        private readonly List<int> currentSelections = new();
        private readonly List<UnityAction> buttonClickHandlers = new();

        private void Awake()
        {
            for (int i = 0; i < selectionButtons.Count; i++)
                buttonClickHandlers.Add(null);
        }

        private void OnEnable()
        {
            RemoveAllListeners();

            for (int i = 0; i < selectionButtons.Count; i++)
            {
                if (selectionButtons[i] != null)
                {
                    int index = i;
                    buttonClickHandlers[i] = () => OnButtonClicked(index);
                    selectionButtons[i].onClick.AddListener(buttonClickHandlers[i]);
                }
            }

            UpdateButtonAppearance();
        }

        private void OnDisable()
        {
            RemoveAllListeners();
        }

        private void RemoveAllListeners()
        {
            for (int i = 0; i < selectionButtons.Count; i++)
                if (selectionButtons[i] != null && buttonClickHandlers[i] != null)
                    selectionButtons[i].onClick.RemoveListener(buttonClickHandlers[i]);
        }

        public List<int> GetSelectedIndices()
        {
            return new List<int>(currentSelections);
        }

        public void ClearSelections()
        {
            currentSelections.Clear();
            UpdateButtonAppearance();
            onSelectionChanged?.Invoke(GetSelectedIndices());
        }

        public void SetSelection(int index, bool selected)
        {
            if (selected && !currentSelections.Contains(index))
                HandleNewSelection(index);
            else if (!selected && currentSelections.Contains(index))
                HandleDeselection(index);
        }

        private void OnButtonClicked(int buttonIndex)
        {
            if (currentSelections.Contains(buttonIndex) && allowDeselection)
                HandleDeselection(buttonIndex);
            else
                HandleNewSelection(buttonIndex);
        }

        private void HandleNewSelection(int buttonIndex)
        {
            switch (selectionMode)
            {
                case SelectionMode.SingleSelection:
                    currentSelections.Clear();
                    currentSelections.Add(buttonIndex);
                    break;

                case SelectionMode.MultipleSelection:
                    currentSelections.Add(buttonIndex);
                    break;
            }

            UpdateButtonAppearance();
            onSelectionChanged?.Invoke(GetSelectedIndices());
        }

        private void HandleDeselection(int buttonIndex)
        {
            currentSelections.Remove(buttonIndex);
            UpdateButtonAppearance();
            onSelectionChanged?.Invoke(GetSelectedIndices());
        }

        private void UpdateButtonAppearance()
        {
            foreach (Button selectionButton in selectionButtons)
            {
                ColorBlock colors = selectionButton.colors;

                bool isSelected = currentSelections.Contains(selectionButtons.IndexOf(selectionButton));

                colors.normalColor = isSelected ? selectedColor : unselectedColor;
                // colors.highlightedColor = isSelected ? selectedColor : unselectedColor;
                // colors.pressedColor = isSelected ? selectedColor : unselectedColor;
                colors.selectedColor = isSelected ? selectedColor : colors.highlightedColor;
                colors.disabledColor = isSelected ? selectedColor : unselectedColor;

                selectionButton.colors = colors;
            }
        }

        public void DisableInteraction()
        {
            foreach (Button selectionButton in selectionButtons)
                selectionButton.interactable = false;
        }

        public void EnableInteraction()
        {
            foreach (Button selectionButton in selectionButtons)
                selectionButton.interactable = true;
        }
    }
}