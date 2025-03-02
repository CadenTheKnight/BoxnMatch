using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI.Components
{
    /// <summary>
    /// Base class for most of the main UI panels.
    /// </summary>
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] protected Button closeButton;
        [SerializeField] protected Button backgroundButton;

        /// <summary>
        /// Adds listeners to the close and background buttons.
        /// </summary>
        protected virtual void OnEnable()
        {
            closeButton.onClick.AddListener(HidePanel);
            backgroundButton.onClick.AddListener(HidePanel);
        }

        /// <summary>
        /// Removes listeners from the close and background buttons.
        /// </summary>
        protected virtual void OnDisable()
        {
            closeButton.onClick.RemoveListener(HidePanel);
            backgroundButton.onClick.RemoveListener(HidePanel);
        }

        /// <summary>
        /// Hides the panel when the escape key is pressed.
        /// </summary>
        protected virtual void Update()
        {
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                HidePanel();
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
