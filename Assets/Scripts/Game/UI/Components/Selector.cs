using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Enums;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Colors;

namespace Assets.Scripts.Game.UI.Components
{
    public class Selector : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> selectionButtons = new();

        [Header("Selector Settings")]
        [SerializeField] private SelectionMode selectionMode = SelectionMode.SingleSelection;
        [SerializeField] private int requiredSelections = 1;
        [SerializeField] private bool allowDeselection = true;

        [Header("Appearance")]
        [SerializeField] private Color selectedColor = UIColors.primaryHoverColor;
        [SerializeField] private Color unselectedColor = UIColors.primaryDefaultColor;

        public System.Action<List<int>> onSelectionChanged;
        private List<int> currentSelections = new();

        private void OnEnable()
        {
            for (int i = 0; i < selectionButtons.Count; i++)
            {
                int index = i;
                if (selectionButtons[i] != null)
                    selectionButtons[i].onClick.AddListener(() => OnButtonClicked(index));
            }

            UpdateButtonAppearance();
        }

        private void OnDisable()
        {
            for (int i = 0; i < selectionButtons.Count; i++)
            {
                int index = i;
                if (selectionButtons[i] != null)
                    selectionButtons[i].onClick.RemoveListener(() => OnButtonClicked(index));
            }
        }

        public void SetSelectionMode(SelectionMode mode, int count = 1)
        {
            selectionMode = mode;
            requiredSelections = count;
            UpdateSelections();
        }

        public List<int> GetSelectedIndices()
        {
            return new List<int>(currentSelections);
        }

        public void ClearSelections()
        {
            currentSelections.Clear();
            UpdateButtonAppearance();
            onSelectionChanged.Invoke(GetSelectedIndices());
        }

        public void SetSelection(int index, bool selected = true)
        {
            if (index < 0 || index >= selectionButtons.Count)
                return;

            if (selected && !currentSelections.Contains(index))
                HandleNewSelection(index);
            else if (!selected && currentSelections.Contains(index))
            {
                currentSelections.Remove(index);
                UpdateButtonAppearance();
                onSelectionChanged.Invoke(GetSelectedIndices());
            }
        }

        private void OnButtonClicked(int buttonIndex)
        {
            if (currentSelections.Contains(buttonIndex))
            {
                if (allowDeselection)
                {
                    if (selectionMode == SelectionMode.ExactCount && currentSelections.Count <= requiredSelections)
                        return;

                    currentSelections.Remove(buttonIndex);
                    UpdateButtonAppearance();
                    onSelectionChanged.Invoke(GetSelectedIndices());
                }
            }
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

                case SelectionMode.ExactCount:
                    if (currentSelections.Count >= requiredSelections)
                        currentSelections.RemoveAt(0);

                    currentSelections.Add(buttonIndex);
                    break;
            }

            UpdateButtonAppearance();
            onSelectionChanged.Invoke(GetSelectedIndices());
        }

        private void UpdateSelections()
        {
            while (selectionMode == SelectionMode.ExactCount && currentSelections.Count > requiredSelections)
                currentSelections.RemoveAt(0);

            if (selectionMode == SelectionMode.SingleSelection && currentSelections.Count > 1)
            {
                int lastSelection = currentSelections.Last();
                currentSelections.Clear();
                currentSelections.Add(lastSelection);
            }

            UpdateButtonAppearance();
            onSelectionChanged.Invoke(GetSelectedIndices());
        }

        private void UpdateButtonAppearance()
        {
            foreach (Button selectionButton in selectionButtons)
                if (selectionButton != null)
                {
                    ColorBlock colors = selectionButton.colors;

                    bool isSelected = currentSelections.Contains(selectionButtons.IndexOf(selectionButton));

                    colors.normalColor = isSelected ? selectedColor : unselectedColor;
                    colors.selectedColor = isSelected ? selectedColor : colors.highlightedColor;

                    selectionButton.colors = colors;
                }
        }

        public void AddSelectionButton(Button button)
        {
            int buttonIndex = selectionButtons.Count;
            selectionButtons.Add(button);
            button.onClick.AddListener(() => OnButtonClicked(buttonIndex));
            UpdateButtonAppearance();
        }

        public void RemoveSelectionButton(Button button)
        {
            int index = selectionButtons.IndexOf(button);
            if (index >= 0)
            {
                button.onClick.RemoveListener(() => OnButtonClicked(index));
                selectionButtons.RemoveAt(index);

                currentSelections.RemoveAll(i => i == index || i >= selectionButtons.Count);

                List<int> newSelections = new();
                foreach (int selection in currentSelections)
                {
                    if (selection < index)
                        newSelections.Add(selection);
                    else if (selection > index)
                        newSelections.Add(selection - 1);
                }

                currentSelections = newSelections;
                UpdateButtonAppearance();
                onSelectionChanged.Invoke(GetSelectedIndices());
            }
        }
    }
}