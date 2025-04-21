using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.NotificationCanvas
{
    /// <summary>
    /// A panel that shows an error message and allows the user to retry or quit the game.
    /// </summary>
    public class ErrorPopup : MonoBehaviour
    {
        [SerializeField] private Button backgroundButton;
        [SerializeField] private TextMeshProUGUI errorCodeText;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button retryButton;

        private Action retryAction;

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
            if (Input.GetKeyDown(KeyCode.Escape)) Retry();
        }

        public void ShowError(OperationResult result, Action retryAction)
        {
            errorCodeText.text = result.Code;
            errorMessageText.text = result.Message;
            this.retryAction = retryAction;

            gameObject.SetActive(true);
        }

        private void Quit()
        {
            Application.Quit();
        }

        private void Retry()
        {
            retryAction?.Invoke();

            gameObject.SetActive(false);
        }
    }
}