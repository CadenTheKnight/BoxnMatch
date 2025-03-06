using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.Game.UI.Components
{
    public class Countdown : MonoBehaviour
    {
        [SerializeField] private Button countdownButton;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private float countdownDuration = 5f;

        private Coroutine countdownCoroutine;
        private bool isCountdownActive = false;

        public event Action OnCountdownComplete;

        private void Start()
        {
            ShowNotReadyMessage();
        }

        private void OnDisable()
        {
            CancelCountdown();
        }

        public void ShowNotReadyMessage()
        {
            if (countdownText != null)
                countdownText.text = "LOBBY NOT READY";

            countdownButton.interactable = false;
        }

        public void CancelCountdown()
        {
            if (countdownCoroutine != null && isCountdownActive)
                Debug.Log("Cancelling active countdown");

            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            isCountdownActive = false;
            countdownButton.interactable = false;
        }

        public void StartCountdown()
        {
            if (isCountdownActive)
            {
                Debug.Log("Countdown already running, not starting again");
                return;
            }

            Debug.Log("Starting countdown");
            CancelCountdown();
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountdownActive = true;
            countdownButton.interactable = true;

            float remainingTime = countdownDuration;

            while (remainingTime > 0)
            {
                if (countdownText != null)
                    countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

                yield return null;
                remainingTime -= Time.deltaTime;
            }

            isCountdownActive = false;
            countdownButton.interactable = false;

            OnCountdownComplete?.Invoke();
        }

        public bool IsCountdownActive => isCountdownActive;
    }
}