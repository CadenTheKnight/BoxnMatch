using UnityEngine;

// Todo
// 1. Check functionality on different screen sizes
// 2. ?

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// Represents a loading bar that moves from left to right.
    /// </summary>
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private RectTransform movingBar;
        [SerializeField] private RectTransform backgroundBar;

        private float speed;
        private float startPosition;
        private float endPosition;
        private bool isLoading = false;

        private void Awake()
        {
            RecalculatePositions();
        }

        /// <summary>
        /// Recalculates the speed and position of the moving bar and the position of the background bar.
        /// </summary>
        private void RecalculatePositions()
        {
            speed = (backgroundBar.rect.width + movingBar.rect.width) / 1f;
            startPosition = -movingBar.rect.width;
            endPosition = backgroundBar.rect.width + movingBar.rect.width;
        }

        /// <summary>
        /// Moves the loading bar from left to right when loading.
        /// </summary>
        private void Update()
        {
            if (!isLoading) return;

            float newX = movingBar.anchoredPosition.x + speed * Time.deltaTime;
            if (newX > endPosition)
                newX = startPosition;

            movingBar.anchoredPosition = new Vector2(newX, movingBar.anchoredPosition.y);
        }

        /// <summary>
        /// Starts the loading animation.
        /// </summary>
        public void StartLoading()
        {
            RecalculatePositions();
            movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
            isLoading = true;
        }

        /// <summary>
        /// Stops the loading animation.
        /// </summary>
        public void StopLoading()
        {
            isLoading = false;
            movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
                RecalculatePositions();
        }
    }
}