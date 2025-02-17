using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ErrorPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorTitleText;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button backgroundButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button retryButton;

    private void Start()
    {
        backgroundButton.onClick.AddListener(Retry);
        quitButton.onClick.AddListener(QuitGame);
        retryButton.onClick.AddListener(Retry);
    }

    public void ShowError(int httpCode, string message)
    {
        errorTitleText.text = $"Error - {httpCode}";
        errorMessageText.text = message;
        gameObject.SetActive(true);
    }

    private void QuitGame()
    {
        backgroundButton.onClick.RemoveListener(Retry);
        quitButton.onClick.RemoveListener(QuitGame);
        retryButton.onClick.RemoveListener(Retry);
        gameObject.SetActive(false);
        Application.Quit();
    }

    private void Retry()
    {
        backgroundButton.onClick.RemoveListener(Retry);
        quitButton.onClick.RemoveListener(QuitGame);
        retryButton.onClick.RemoveListener(Retry);
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
