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
    /// Displays a notification in the top-right corner.
    /// </summary>
    public class ResultNotification : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button notificationButton;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Animation")]
        [SerializeField] private float autoHideDelaySeconds = 3f;

        private Coroutine autoHideCoroutine;

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
            ApplyTheme(result.Status);

            gameObject.SetActive(true);
            StartAutoHide();
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