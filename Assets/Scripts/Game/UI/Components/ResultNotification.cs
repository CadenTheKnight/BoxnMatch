using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Colors;

namespace Assets.Scripts.Game.UI.Components
{
    public enum NotificationType
    {
        Error,
        Warning,
        Success
    }

    /// <summary>
    /// Displays a notification to the user. This notification can be used to display errors, warnings, or success messages.
    /// </summary>
    public class ResultNotification : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Button notification;
        [SerializeField] private TextMeshProUGUI notificationText;

        private Coroutine autoHideCoroutine;
        private readonly float autoHideDelaySeconds = 3f;

        private void OnEnable()
        {
            notification.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            notification.onClick.RemoveListener(OnCloseClicked);
            CancelAutoHide();
        }

        /// <summary>
        /// Shows a notification to the user.
        /// </summary>
        /// <param name="operationResult">The result of the operation.</param>
        /// <param name="type">The type of notification to display.</param>
        public void ShowNotification(OperationResult operationResult, NotificationType type)
        {
            notificationText.text = operationResult.Message;

            ApplyTheme(type);
            gameObject.SetActive(true);
            StartAutoHide();
        }

        /// <summary>
        /// Applies the theme to the notification based on the type.
        /// </summary>
        /// <param name="type">The type of notification.</param>
        private void ApplyTheme(NotificationType type)
        {
            ColorBlock colors = notification.colors;

            switch (type)
            {
                case NotificationType.Error:
                    colors.normalColor = UIColors.redDefaultColor;
                    colors.highlightedColor = UIColors.redHoverColor;
                    colors.pressedColor = UIColors.redDefaultColor;
                    colors.selectedColor = UIColors.redHoverColor;
                    break;

                case NotificationType.Warning:
                    colors.normalColor = UIColors.yellowDefaultColor;
                    colors.highlightedColor = UIColors.yellowHoverColor;
                    colors.pressedColor = UIColors.yellowDefaultColor;
                    colors.selectedColor = UIColors.yellowHoverColor;
                    break;

                case NotificationType.Success:
                default:
                    colors.normalColor = UIColors.greenDefaultColor;
                    colors.highlightedColor = UIColors.greenHoverColor;
                    colors.pressedColor = UIColors.greenDefaultColor;
                    colors.selectedColor = UIColors.greenHoverColor;
                    break;
            }

            colors.disabledColor = UIColors.primaryDisabledColor;
            notification.colors = colors;
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