using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Game.UI.Components.Options.Selector
{
    public class Selector : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> selectionButtons = new();

        [Header("Selector Settings")]
        [SerializeField] private bool allowDeselection = true;

        public Action<int> onSelectionChanged;

        public int Selection { get; private set; } = -1;

        private void Awake()
        {
            for (int i = 0; i < selectionButtons.Count; i++)
            {
                int buttonIndex = i;
                selectionButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));
            }

            UpdateButtons();
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

        public void SetSelection(int index, bool selected = true)
        {
            if (selected && index != Selection) Select(index);
            else if (!selected && index == Selection) Clear();
            UpdateButtons();
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

                SelectionButtonColors colorConfig = selectionButton.GetComponent<SelectionButtonColors>();

                colors.normalColor = selected ? colorConfig.SelectedColor : colorConfig.NormalColor;
                colors.highlightedColor = selected ? colorConfig.SelectedHoverColor : colorConfig.NormalHoverColor;
                colors.pressedColor = selected ? colorConfig.SelectedPressedColor : colorConfig.NormalPressedColor;
                colors.selectedColor = selected ? colorConfig.SelectedSelectedColor : colorConfig.NormalSelectedColor;
                colors.disabledColor = selected ? colorConfig.SelectedDisabledColor : colorConfig.NormalDisabledColor;

                selectionButton.colors = colors;
            }
        }

        public void UpdateInteractable(bool isInteractable, int index = -1)
        {
            if (index >= 0 && index < selectionButtons.Count) selectionButtons[index].interactable = isInteractable;
            else foreach (Button selectionButton in selectionButtons) selectionButton.interactable = isInteractable;
        }
    }
}