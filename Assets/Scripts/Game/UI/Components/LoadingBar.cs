using UnityEngine;

namespace Assets.Scripts.Game.UI.Components
{
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

        private void RecalculatePositions()
        {
            speed = (backgroundBar.rect.width + movingBar.rect.width) / 1f;
            startPosition = -movingBar.rect.width;
            endPosition = backgroundBar.rect.width + movingBar.rect.width;
        }

        private void Update()
        {
            if (!isLoading) return;

            float newX = movingBar.anchoredPosition.x + speed * Time.deltaTime;
            if (newX > endPosition)
            {
                newX = startPosition;
            }
            movingBar.anchoredPosition = new Vector2(newX, movingBar.anchoredPosition.y);
        }

        public void StartLoading()
        {
            RecalculatePositions();
            movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
            isLoading = true;
        }

        public void StopLoading()
        {
            isLoading = false;
            movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
            {
                RecalculatePositions();
            }
        }
    }
}