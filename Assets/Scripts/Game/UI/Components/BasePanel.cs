using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] protected Button closeButton;
        [SerializeField] protected Button backgroundButton;

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
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                HidePanel();
            }
        }

        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        public virtual void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
