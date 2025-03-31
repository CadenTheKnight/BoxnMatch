using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.NotificationCanvas
{
    /// <summary>
    /// Displays a notification in the top-right corner that dynamically sizes based on content.
    /// </summary>
    public class ResultNotification : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button notificationButton;
        [SerializeField] private RectTransform notificationRect;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Position Settings")]
        [SerializeField] private float topOffset = 0.05f;    // 5% from top
        [SerializeField] private float rightOffset = 0.05f;  // 5% from right

        [Header("Size Settings")]
        [SerializeField] private float maxWidth = 0.25f;     // 25% of screen width
        [SerializeField] private float maxHeight = 0.1f;     // 10% of screen height
        [SerializeField] private float minWidth = 0.05f;     // 5% of screen width
        [SerializeField] private float minHeight = 0.05f;    // 5% of screen height
        [SerializeField] private float padding = 10f;        // Padding inside notification

        [Header("Animation")]
        [SerializeField] private float autoHideDelaySeconds = 3f;

        private Coroutine autoHideCoroutine;

        private void Awake()
        {
            ConfigureRectTransform();
        }

        private void ConfigureRectTransform()
        {
            notificationRect.anchorMin = new Vector2(1, 1);
            notificationRect.anchorMax = new Vector2(1, 1);
            notificationRect.pivot = new Vector2(1, 1);

            float xOffset = Screen.width * rightOffset;
            float yOffset = Screen.height * topOffset;
            notificationRect.anchoredPosition = new Vector2(-xOffset, -yOffset);
        }

        private void OnEnable()
        {
            notificationButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            notificationButton.onClick.RemoveListener(OnCloseClicked);
            CancelAutoHide();
        }

        /// <summary>
        /// Shows a notification to the user.
        /// </summary>
        /// <param name="result">The result of the operation.</param>
        public void ShowNotification(OperationResult result)
        {
            Debug.Log($"{result.Code} - {result.Message}");

            notificationText.text = result.Message;

            notificationText.enableWordWrapping = true;
            notificationRect.sizeDelta = Vector2.zero;

            UpdateNotificationSize();
            ApplyTheme(result.Status);

            gameObject.SetActive(true);
            StartAutoHide();
        }

        private void UpdateNotificationSize()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float maxWidthPixels = screenWidth * maxWidth;
            float maxHeightPixels = screenHeight * maxHeight;
            float minWidthPixels = screenWidth * minWidth;
            float minHeightPixels = screenHeight * minHeight;

            notificationText.enableWordWrapping = false;
            notificationText.rectTransform.sizeDelta = new Vector2(float.MaxValue, float.MaxValue);

            LayoutRebuilder.ForceRebuildLayoutImmediate(notificationText.rectTransform);
            float contentWidth = notificationText.preferredWidth + padding * 2;
            float contentHeight = notificationText.preferredHeight + padding * 2;

            if (contentWidth > maxWidthPixels)
            {
                notificationText.enableWordWrapping = true;
                notificationText.rectTransform.sizeDelta = new Vector2(maxWidthPixels - padding * 2, float.MaxValue);

                LayoutRebuilder.ForceRebuildLayoutImmediate(notificationText.rectTransform);
                contentHeight = notificationText.preferredHeight + padding * 2;
                contentWidth = maxWidthPixels;
            }

            if (contentHeight > maxHeightPixels)
            {
                contentHeight = maxHeightPixels;
                notificationText.overflowMode = TextOverflowModes.Ellipsis;
            }
            else
                notificationText.overflowMode = TextOverflowModes.Overflow;

            contentWidth = Mathf.Max(contentWidth, minWidthPixels);
            contentHeight = Mathf.Max(contentHeight, minHeightPixels);

            notificationRect.sizeDelta = new Vector2(contentWidth, contentHeight);

            notificationText.rectTransform.anchorMin = Vector2.zero;
            notificationText.rectTransform.anchorMax = Vector2.one;
            notificationText.rectTransform.offsetMin = new Vector2(padding, padding);
            notificationText.rectTransform.offsetMax = new Vector2(-padding, -padding);
        }

        /// <summary>
        /// Applies the theme to the notification based on the status.
        /// </summary>
        private void ApplyTheme(ResultStatus status)
        {
            ColorBlock colors = notificationButton.colors;

            switch (status)
            {
                case ResultStatus.Success:
                    colors.normalColor = UIColors.greenDefaultColor;
                    colors.highlightedColor = UIColors.greenHoverColor;
                    break;
                case ResultStatus.Warning:
                    colors.normalColor = UIColors.yellowDefaultColor;
                    colors.highlightedColor = UIColors.yellowHoverColor;
                    break;
                case ResultStatus.Error:
                    colors.normalColor = UIColors.redDefaultColor;
                    colors.highlightedColor = UIColors.redHoverColor;
                    break;
            }

            colors.pressedColor = colors.normalColor;
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = UIColors.secondaryDisabledColor;
            notificationButton.colors = colors;
        }

        private void OnCloseClicked()
        {
            HideNotification();
        }

        private void StartAutoHide()
        {
            CancelAutoHide();
            autoHideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private void CancelAutoHide()
        {
            if (autoHideCoroutine != null)
            {
                StopCoroutine(autoHideCoroutine);
                autoHideCoroutine = null;
            }
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(autoHideDelaySeconds);
            HideNotification();
            autoHideCoroutine = null;
        }

        private void HideNotification()
        {
            CancelAutoHide();
            gameObject.SetActive(false);
        }
    }
}