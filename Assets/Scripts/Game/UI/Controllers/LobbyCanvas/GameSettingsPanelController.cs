using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Framework.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Utilities;

namespace Assets.Scripts.Game.UI.Controllers.LobbyCanvas
{
    public class GameSettingsPanelController : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private MapChanger mapChanger;
        [SerializeField] private Incrementer roundCountIncrementer;
        [SerializeField] private Incrementer roundTimeIncrementer;
        [SerializeField] private Selector gameModeSelector;


        [Header("Settings Management")]
        [SerializeField] private Button editUpdateButton;
        [SerializeField] private TextMeshProUGUI editUpdateText;
        [SerializeField] private MapSelectionData mapSelectionData;

        private bool isEditing = false;

        private void OnEnable()
        {
            // mapChanger.onSelectionChanged += OnMapChanged;
            // roundCountIncrementer.onValueChanged += OnRoundCountChanged;
            // roundTimeIncrementer.onValueChanged += OnRoundTimeChanged;
            // gameModeSelector.onSelectionChanged += OnGameModeChanged;

            SetPanelState(false);

            UpdateEditUpdateButton(isEditing);
            editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyDataUpdated += OnLobbyDataUpdated;
        }

        private void OnDisable()
        {
            // mapChanger.onSelectionChanged -= OnMapChanged;
            // roundCountIncrementer.onValueChanged -= OnRoundCountChanged;
            // roundTimeIncrementer.onValueChanged -= OnRoundTimeChanged;
            // gameModeSelector.onSelectionChanged -= OnGameModeChanged;

            editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyDataUpdated -= OnLobbyDataUpdated;
        }

        // private void OnMapChanged(string mapName)
        // {
        //     // do something
        //     Debug.Log($"Map changed to: {mapName}");
        // }

        // private void OnRoundCountChanged(int newValue)
        // {
        //     // do something
        //     Debug.Log($"Round count changed to: {newValue}");
        // }

        // private void OnRoundTimeChanged(int newValue)
        // {
        //     // do something
        //     Debug.Log($"Round time changed to: {newValue}");
        // }

        // private void OnGameModeChanged(List<int> newIndices)
        // {
        //     // do something
        //     Debug.Log($"Game mode changed to: {newIndices[0]}");
        // }

        private void OnEditUpdateClicked()
        {
            isEditing = !isEditing;
            SetPanelState(isEditing);
        }

        private void SetPanelState(bool isEditing)
        {
            UpdateEditUpdateButton(isEditing);

            if (isEditing)
            {
                mapChanger.EnableInteraction();
                roundCountIncrementer.EnableInteraction();
                roundTimeIncrementer.EnableInteraction();
                gameModeSelector.EnableInteraction();
            }
            else
            {
                mapChanger.DisableInteraction();
                roundCountIncrementer.DisableInteraction();
                roundTimeIncrementer.DisableInteraction();
                gameModeSelector.DisableInteraction();

                UpdateLobbyData();
            }
        }

        private void UpdateEditUpdateButton(bool isEditing)
        {
            editUpdateText.text = isEditing ? "UPDATE" : "EDIT";

            ColorBlock colors = editUpdateButton.colors;
            colors.normalColor = isEditing ? UIColors.greenDefaultColor : UIColors.secondaryDefaultColor;
            colors.highlightedColor = isEditing ? UIColors.greenHoverColor : UIColors.secondaryHoverColor;
            colors.pressedColor = isEditing ? UIColors.greenDefaultColor : UIColors.secondaryPressedColor;
            colors.selectedColor = isEditing ? UIColors.greenHoverColor : UIColors.secondaryHoverColor;
            colors.disabledColor = UIColors.secondaryDisabledColor;

            editUpdateButton.colors = colors;
        }

        private async void UpdateLobbyData()
        {
            editUpdateButton.interactable = false;
            editUpdateText.text = "Updating...";

            await Task.Delay(1500);
            GameLobbyManager.Instance.UpdateLobbyData(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, (GameMode)gameModeSelector.GetSelectedIndices()[0]);

            editUpdateButton.interactable = true;
            editUpdateText.text = "EDIT";
        }

        private void OnLobbyDataUpdated(OperationResult result)
        {
            mapChanger.SetSelection(int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection(int.Parse(LobbyManager.Instance.Lobby.Data["GameMode"].Value), true);
        }
    }
}