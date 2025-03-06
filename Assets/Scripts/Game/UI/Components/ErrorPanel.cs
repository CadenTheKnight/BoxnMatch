using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Find alternative solution for initialization error if this is the only use case

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// A panel that shows an error message and allows the user to retry or quit the game.
    /// </summary>
    public class ErrorPanel : MonoBehaviour
    {
        [SerializeField] private Button quitButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private TextMeshProUGUI errorCodeText;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        private System.Action retryAction;

        private void OnEnable()
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void OnDisable()
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
            retryButton.onClick.RemoveListener(OnRetryClicked);
        }

        public void ShowError(string errorCode, string errorMessage, System.Action retryAction)
        {
            errorCodeText.text = errorCode;
            errorMessageText.text = errorMessage;
            this.retryAction = retryAction;

            gameObject.SetActive(true);
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        private void OnRetryClicked()
        {
            gameObject.SetActive(false);
            retryAction?.Invoke();
        }
    }
}