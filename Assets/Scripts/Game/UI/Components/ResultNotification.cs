using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Components
{
    public class ResultNotification : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI NotificationText;
        [SerializeField] private float autoHideDelaySeconds = 3f;

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

        public void ShowNotification(OperationResult operationResult)
        {
            NotificationText.text = $"{operationResult.Code} - {operationResult.Message}";

            gameObject.SetActive(true);
            StartAutoHide();
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