using UnityEngine;

namespace Assets.Scripts.Game.UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform progressRect;

        private float progress = 0f;

        public void SetProgress(float value)
        {
            progress = Mathf.Clamp01(value);
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            progressRect.anchorMax = new Vector2(progress, 1f);
        }
    }
}