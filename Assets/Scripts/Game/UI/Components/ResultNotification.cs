using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Colors;

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// Displays a notification to the user.
    /// </summary>
    public class ResultNotification : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button notificationButton;
        [SerializeField] private RectTransform notificationRect;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Size Settings")]
        [SerializeField] private float maxWidth = 0.4f;
        [SerializeField] private float maxHeight = 0.2f;

        private Coroutine autoHideCoroutine;
        private readonly float autoHideDelaySeconds = 3f;

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
        /// <param name="operationResult">The result of the operation.</param>
        /// <param name="type">The type of notification to display.</param>
        public void ShowNotification(OperationResult operationResult, ResultStatus type)
        {
            notificationText.text = operationResult.Message;

            float maxX = Screen.width * maxWidth;
            float maxY = Screen.height * maxHeight;
            Vector2 size = notificationText.rectTransform.sizeDelta;
            size.x = Mathf.Min(notificationText.preferredWidth, maxX);
            size.y = Mathf.Min(notificationText.preferredHeight, maxY);
            notificationText.rectTransform.sizeDelta = size;

            ApplyTheme(type);
            gameObject.SetActive(true);
            StartAutoHide();
        }

        /// <summary>
        /// Applies the theme to the notification based on the type.
        /// </summary>
        /// <param name="type">The type of notification.</param>
        private void ApplyTheme(ResultStatus type)
        {
            ColorBlock colors = notificationButton.colors;

            switch (type)
            {
                case ResultStatus.Success:
                    colors.normalColor = UIColors.greenDefaultColor;
                    colors.highlightedColor = UIColors.greenHoverColor;
                    colors.pressedColor = UIColors.greenDefaultColor;
                    colors.selectedColor = UIColors.greenHoverColor;
                    break;
                case ResultStatus.Warning:
                    colors.normalColor = UIColors.yellowDefaultColor;
                    colors.highlightedColor = UIColors.yellowHoverColor;
                    colors.pressedColor = UIColors.yellowDefaultColor;
                    colors.selectedColor = UIColors.yellowHoverColor;
                    break;
                case ResultStatus.Error:
                    colors.normalColor = UIColors.redDefaultColor;
                    colors.highlightedColor = UIColors.redHoverColor;
                    colors.pressedColor = UIColors.redDefaultColor;
                    colors.selectedColor = UIColors.redHoverColor;
                    break;
            }

            colors.disabledColor = UIColors.secondaryDisabledColor;
            notificationButton.colors = colors;
        }

        private void OnCloseClicked()
        {
            CancelAutoHide();
            gameObject.SetActive(false);
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
            gameObject.SetActive(false);
            autoHideCoroutine = null;
        }
    }
}