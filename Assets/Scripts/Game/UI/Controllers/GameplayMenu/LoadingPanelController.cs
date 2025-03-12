using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.Managers;


namespace Assets.Scripts.Game.UI.Controllers.GameplayMenu
{
    public class LoadingPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadingBar loadingBar;
        [SerializeField] private Image mapThumbnailImage;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI mapNameText;

        public void StartLoading(Sprite thumbnail, string name, string status)
        {
            mapThumbnailImage.sprite = thumbnail;
            mapNameText.text = name;
            statusText.text = status;
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
            GameLobbyManager.Instance.LoadGameMap();
        }
    }
}