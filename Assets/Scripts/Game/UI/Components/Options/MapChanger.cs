using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;

namespace Assets.Scripts.Game.UI.Components.Options
{
    public class MapChanger : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private Image mapThumbnailImage;

        [Header("Selection Info")]
        [SerializeField] private MapSelectionData mapSelectionData;

        public Action<string> OnMapChanged;

        public int Value { get; private set; }

        private void OnEnable()
        {
            leftButton.onClick.AddListener(OnLeftClicked);
            rightButton.onClick.AddListener(OnRightClicked);
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(OnLeftClicked);
            rightButton.onClick.RemoveListener(OnRightClicked);
        }

        public void SetValue(int index)
        {
            if (index < 0 || index >= mapSelectionData.Maps.Count) return;
            Value = index;
            UpdateMap();
        }

        private void UpdateMap()
        {
            mapNameText.text = mapSelectionData.GetMap(Value).Name;
            mapThumbnailImage.sprite = mapSelectionData.GetMap(Value).Thumbnail;
        }

        private void OnLeftClicked()
        {
            if (Value > 0) Value--;
            else Value = mapSelectionData.Maps.Count - 1;

            UpdateMap();
            OnMapChanged?.Invoke(mapSelectionData.GetMap(Value).Name);
        }

        private void OnRightClicked()
        {
            if (Value < mapSelectionData.Maps.Count - 1) Value++;
            else Value = 0;

            UpdateMap();
            OnMapChanged?.Invoke(mapSelectionData.GetMap(Value).Name);
        }

        public void UpdateInteractable(bool isInteractable)
        {
            leftButton.interactable = isInteractable;
            rightButton.interactable = isInteractable;
        }
    }
}