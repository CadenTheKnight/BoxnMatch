using UnityEngine;

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// Contains the colors used by the UI components.
    /// </summary>
    public static class UIColors
    {
        [Header("Error Colors")]
        [SerializeField] public static Color errorDefaultColor = new(1f, 0.28f, 0.34f); // FF4757
        [SerializeField] public static Color errorHoverColor = new(1f, 0.42f, 0.51f);   // FF6B81

        [Header("Warning Colors")]
        [SerializeField] public static Color warningDefaultColor = new(1f, 0.5f, 0.31f); // FF7F50
        [SerializeField] public static Color warningHoverColor = new(1f, 0.65f, 0.01f);   // FFA502

        [Header("Success Colors")]
        [SerializeField] public static Color successDefaultColor = new(0.18f, 0.84f, 0.45f); // 2ED573
        [SerializeField] public static Color successHoverColor = new(0.48f, 0.93f, 0.62f);   // 7BED9F

        [Header("Disabled Color")]
        [SerializeField] public static Color disabledColor = new(0.34f, 0.38f, 0.44f); // 57606F
    }
}
