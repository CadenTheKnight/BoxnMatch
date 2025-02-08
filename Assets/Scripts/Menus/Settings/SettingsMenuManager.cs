using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Toggle fullscreenToggle;


    private void Start()
    {
        backButton.onClick.AddListener(CloseSettingsMenu);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void OpenSettingsMenu()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsPanel.SetActive(false);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
