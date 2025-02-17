using UnityEngine;

public class LoadingBarAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform movingBar;
    [SerializeField] private RectTransform backgroundBar;
    [SerializeField] public float timeToCrossBar = 1f;

    private float speed;
    private float startPosition;
    private float endPosition;
    private bool isLoading = false;

    private void Start()
    {
        speed = (backgroundBar.rect.width + movingBar.rect.width) / timeToCrossBar;
        startPosition = -movingBar.rect.width;
        endPosition = movingBar.rect.width + backgroundBar.rect.width;
        movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
    }

    private void Update()
    {
        if (isLoading)
        {
            float newX = movingBar.anchoredPosition.x + speed * Time.deltaTime;

            if (newX > endPosition)
            {
                newX = startPosition;
            }

            movingBar.anchoredPosition = new Vector2(newX, movingBar.anchoredPosition.y);
        }
    }

    public void StartLoading()
    {
        isLoading = true;
    }

    public void StopLoading()
    {
        isLoading = false;
        movingBar.anchoredPosition = new Vector2(startPosition, movingBar.anchoredPosition.y);
    }
}

