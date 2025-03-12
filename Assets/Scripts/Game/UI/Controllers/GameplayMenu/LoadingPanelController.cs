using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;

namespace Assets.Scripts.Game.UI.Controllers.GameplayMenu
{
    public class LoadingPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private Image mapThumbnailImage;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI mapNameText;

        public void StartLoading(string mapName, Sprite mapThumbnail, string loadingStatus)
        {
            mapThumbnailImage.sprite = mapThumbnail;
            mapNameText.text = mapName;
            statusText.text = loadingStatus;
            loadingBar.StartLoading();
        }

        public void SetStatus(string status)
        {
            statusText.text = status;
        }

        public void StopLoading()
        {
            mapThumbnailImage.sprite = null;
            mapNameText.text = "";
            statusText.text = "";
            loadingBar.StopLoading();
        }
    }
}