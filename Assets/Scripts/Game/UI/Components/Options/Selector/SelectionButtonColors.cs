using UnityEngine;
using Assets.Scripts.Game.UI.Colors;

namespace Assets.Scripts.Game.UI.Components.Options.Selector
{
    /// <summary>
    /// Represents a button with more colors for the selected state.
    /// </summary>
    public class SelectionButtonColors : MonoBehaviour
    {
        [Header("Normal State")]
        [SerializeField] private Color normalColor = UIColors.Primary.Three;
        [SerializeField] private Color normalHoverColor = UIColors.Primary.Five;
        [SerializeField] private Color normalPressedColor = UIColors.Primary.Six;
        [SerializeField] private Color normalSelectedColor = UIColors.Primary.Five;
        [SerializeField] private Color normalDisabledColor = UIColors.Primary.Two;

        [Header("Selected State")]
        [SerializeField] private Color selectedColor = UIColors.Green.One;
        [SerializeField] private Color selectedHoverColor = UIColors.Green.Two;
        [SerializeField] private Color selectedPressedColor = UIColors.Green.Three;
        [SerializeField] private Color selectedSelectedColor = UIColors.Green.Four;
        [SerializeField] private Color selectedDisabledColor = UIColors.Primary.Six;

        public Color NormalColor => normalColor;
        public Color NormalHoverColor => normalHoverColor;
        public Color NormalPressedColor => normalPressedColor;
        public Color NormalSelectedColor => normalSelectedColor;
        public Color NormalDisabledColor => normalDisabledColor;

        public Color SelectedColor => selectedColor;
        public Color SelectedHoverColor => selectedHoverColor;
        public Color SelectedPressedColor => selectedPressedColor;
        public Color SelectedSelectedColor => selectedSelectedColor;
        public Color SelectedDisabledColor => selectedDisabledColor;
    }
}