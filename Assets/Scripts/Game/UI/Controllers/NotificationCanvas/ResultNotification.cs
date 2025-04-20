using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Game.UI.Colors;
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
                    colors.normalColor = UIColors.Green.One;
                    colors.highlightedColor = UIColors.Green.Two;
                    colors.pressedColor = UIColors.Green.Three;
                    colors.selectedColor = UIColors.Green.Three;
                    break;
                case ResultStatus.Warning:
                    colors.normalColor = UIColors.Orange.One;
                    colors.highlightedColor = UIColors.Orange.Two;
                    colors.pressedColor = UIColors.Orange.Three;
                    colors.selectedColor = UIColors.Orange.Three;
                    break;
                case ResultStatus.Error:
                    colors.normalColor = UIColors.Red.One;
                    colors.highlightedColor = UIColors.Red.Two;
                    colors.pressedColor = UIColors.Red.Three;
                    colors.selectedColor = UIColors.Red.Three;
                    break;
            }

            colors.disabledColor = UIColors.Primary.Three;
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