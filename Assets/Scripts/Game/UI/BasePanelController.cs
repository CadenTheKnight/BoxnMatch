using UnityEngine;
using UnityEngine.UI;

public abstract class BasePanelController : MonoBehaviour
{
    [SerializeField] protected GameObject panel;
    [SerializeField] protected Button backgroundButton;
    [SerializeField] protected Button closeButton;

    protected virtual void OnEnable()
    {
        closeButton.onClick.AddListener(HidePanel);
        backgroundButton.onClick.AddListener(HidePanel);
    }

    protected virtual void OnDisable()
    {
        closeButton.onClick.RemoveListener(HidePanel);
        backgroundButton.onClick.RemoveListener(HidePanel);
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
        }
    }

    public virtual void ShowPanel()
    {
        panel.SetActive(true);
    }

    public virtual void HidePanel()
    {
        panel.SetActive(false);
    }
}
