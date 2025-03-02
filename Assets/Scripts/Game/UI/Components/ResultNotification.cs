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
        Success
    }

    /// <summary>
    /// Displays a notification to the user. This notification can be used to display errors, warnings, or success messages.
    /// </summary>
    public class ResultNotification : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button closeButton;
        [SerializeField] private float autoHideDelaySeconds = 3f;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Error Colors")]
        [SerializeField] private Color errorDefaultColor = new(1f, 0.28f, 0.34f); // FF4757
        [SerializeField] private Color errorHoverColor = new(1f, 0.42f, 0.51f);   // FF6B81

        [Header("Success Colors")]
        [SerializeField] private Color successDefaultColor = new(0.18f, 0.84f, 0.45f); // 2ED573
        [SerializeField] private Color successHoverColor = new(0.48f, 0.93f, 0.62f);   // 7BED9F

        [Header("Shared Colors")]
        [SerializeField] private Color disabledColor = new(0.34f, 0.38f, 0.44f); // 57606F

        private Coroutine autoHideCoroutine;

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
            notificationText.text = $"{operationResult.Code} - {operationResult.Message}";
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
                    colors.normalColor = errorDefaultColor;
                    colors.highlightedColor = errorHoverColor;
                    colors.pressedColor = errorDefaultColor;
                    colors.selectedColor = errorHoverColor;
                    break;

                case NotificationType.Success:
                default:
                    colors.normalColor = successDefaultColor;
                    colors.highlightedColor = successHoverColor;
                    colors.pressedColor = successDefaultColor;
                    colors.selectedColor = successHoverColor;
                    break;
            }

            colors.disabledColor = disabledColor;
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