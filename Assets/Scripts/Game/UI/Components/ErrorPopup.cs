using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Find alternative solution for initialization error if this is the only use case

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// A panel that shows an error message and allows the user to retry or quit the game.
    /// </summary>
    public class ErrorPopup : MonoBehaviour
    {
        [SerializeField] private Button quitButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button backgroundButton;
        [SerializeField] private TextMeshProUGUI errorCodeText;
        [SerializeField] private TextMeshProUGUI errorMessageText;

        private System.Action retryAction;

        private void OnEnable()
        {
            quitButton.onClick.AddListener(Quit);
            retryButton.onClick.AddListener(Retry);
            backgroundButton.onClick.AddListener(Retry);
        }

        private void OnDisable()
        {
            quitButton.onClick.RemoveListener(Quit);
            retryButton.onClick.RemoveListener(Retry);
            backgroundButton.onClick.RemoveListener(Retry);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Quit();
        }

        public void ShowError(string errorCode, string errorMessage, System.Action retryAction)
        {
            errorCodeText.text = errorCode;
            errorMessageText.text = errorMessage;
            this.retryAction = retryAction;

            gameObject.SetActive(true);
        }

        private void Quit()
        {
            Application.Quit();
        }

        private void Retry()
        {
            gameObject.SetActive(false);
            retryAction?.Invoke();
        }
    }
}