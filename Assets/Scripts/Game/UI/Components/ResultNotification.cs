using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Framework.Utilities;

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
        [Header("UI References")]
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI notificationText;

        private Coroutine autoHideCoroutine;
        private readonly float autoHideDelaySeconds = 3f;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
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
            ColorBlock colors = closeButton.colors;

            switch (type)
            {
                case NotificationType.Error:
                    colors.normalColor = UIColors.errorDefaultColor;
                    colors.highlightedColor = UIColors.errorHoverColor;
                    colors.pressedColor = UIColors.errorDefaultColor;
                    colors.selectedColor = UIColors.errorHoverColor;
                    break;

                case NotificationType.Warning:
                    colors.normalColor = UIColors.warningDefaultColor;
                    colors.highlightedColor = UIColors.warningHoverColor;
                    colors.pressedColor = UIColors.warningDefaultColor;
                    colors.selectedColor = UIColors.warningHoverColor;
                    break;

                case NotificationType.Success:
                default:
                    colors.normalColor = UIColors.successDefaultColor;
                    colors.highlightedColor = UIColors.successHoverColor;
                    colors.pressedColor = UIColors.successDefaultColor;
                    colors.selectedColor = UIColors.successHoverColor;
                    break;
            }

            colors.disabledColor = UIColors.disabledColor;
            closeButton.colors = colors;
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