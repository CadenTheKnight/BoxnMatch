using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.UI.Components
{
    public class LoadingStatus : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private LoadingBar loadingBar;

        public void UpdateStatus(string message)
        {
            statusText.text = message;
        }

        public void StartLoading()
        {
            loadingBar.StartLoading();
        }

        public void StopLoading()
        {
            loadingBar.StopLoading();
        }
    }
}