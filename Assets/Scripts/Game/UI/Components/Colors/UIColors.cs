using UnityEngine;

namespace Assets.Scripts.Game.UI.Components.Colors
{
    /// <summary>
    /// Scriptable Object that contains editable UI color palettes.
    /// </summary>
    public static class UIColors
    {
        [Header("Primary Colors")]
        [SerializeField] public static Color primaryDefaultColor = new(0.87f, 0.89f, 0.92f); // DDE0E4
        [SerializeField] public static Color primaryHoverColor = new(0.184f, 0.208f, 0.259f); // 2F3542
        [SerializeField] public static Color primaryPressedColor = new(0.455f, 0.490f, 0.549f); // 747D8C
        [SerializeField] public static Color primaryDisabledColor = new(0.34f, 0.38f, 0.44f); // 57606F

        [Header("Green Colors")]
        [SerializeField] public static Color greenDefaultColor = new(0.18f, 0.84f, 0.45f); // 2ED573
        [SerializeField] public static Color greenHoverColor = new(0.48f, 0.93f, 0.62f);   // 7BED9F

        [Header("Yellow Colors")]
        [SerializeField] public static Color yellowDefaultColor = new(1f, 0.65f, 0.01f); // FFA502
        [SerializeField] public static Color yellowHoverColor = new(1f, 0.75f, 0.1f);   // FFCF19

        [Header("Red Colors")]
        [SerializeField] public static Color redDefaultColor = new(0.91f, 0.3f, 0.24f); // E74C3C
        [SerializeField] public static Color redHoverColor = new(0.95f, 0.45f, 0.35f);   // F16F5A
    }
}