using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;

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

        private bool isEditing = false;

        private void Start()
        {
            UpdateSelections();
            UpdateInteractable(isEditing);
            UpdateEditUpdateButton(isEditing);

            editUpdateButton.interactable = AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId;
        }

        private void OnEnable()
        {
            editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
            LobbyEvents.OnLobbyMapIndexChanged += OnLobbyMapIndexChanged;
            LobbyEvents.OnLobbyRoundCountChanged += OnLobbyRoundCountChanged;
            LobbyEvents.OnLobbyRoundTimeChanged += OnLobbyRoundTimeChanged;
            LobbyEvents.OnLobbyGameModeChanged += OnLobbyGameModeChanged;
            LobbyEvents.OnLobbyDataChanged += OnLobbyDataChanged;
        }

        private void OnDisable()
        {
            editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
            LobbyEvents.OnLobbyMapIndexChanged -= OnLobbyMapIndexChanged;
            LobbyEvents.OnLobbyRoundCountChanged -= OnLobbyRoundCountChanged;
            LobbyEvents.OnLobbyRoundTimeChanged -= OnLobbyRoundTimeChanged;
            LobbyEvents.OnLobbyGameModeChanged -= OnLobbyGameModeChanged;
            LobbyEvents.OnLobbyDataChanged -= OnLobbyDataChanged;
        }

        private async void OnEditUpdateClicked()
        {
            isEditing = !isEditing;
            UpdateInteractable(isEditing);

            if (!isEditing)
            {
                editUpdateText.text = "Updating...";
                editUpdateButton.interactable = false;

                Dictionary<string, DataObject> changedData = new();

                int currentMapIndex = int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value);
                if (mapChanger.Value != currentMapIndex) { changedData["MapIndex"] = new DataObject(DataObject.VisibilityOptions.Public, mapChanger.Value.ToString()); }

                int currentRoundCount = int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value);
                if (roundCountIncrementer.Value != currentRoundCount) { changedData["RoundCount"] = new DataObject(DataObject.VisibilityOptions.Member, roundCountIncrementer.Value.ToString()); }

                int currentRoundTime = int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value);
                if (roundTimeIncrementer.Value != currentRoundTime) { changedData["RoundTime"] = new DataObject(DataObject.VisibilityOptions.Member, roundTimeIncrementer.Value.ToString()); }

                int currentGameMode = (int)Enum.Parse<GameMode>(LobbyManager.Instance.Lobby.Data["GameMode"].Value);
                if (gameModeSelector.Selection != currentGameMode) { changedData["GameMode"] = new DataObject(DataObject.VisibilityOptions.Public, gameModeSelector.Selection.ToString()); }

                if (changedData.Count > 0) await LobbyManager.Instance.UpdateLobbyData(changedData);
            }
            else UpdateEditUpdateButton(isEditing);
        }

        private void OnLobbyHostMigrated(Player player)
        {
            editUpdateButton.interactable = player.Id == AuthenticationManager.Instance.LocalPlayer.Id;
        }

        private void OnLobbyMapIndexChanged(int mapIndex)
        {
            mapChanger.SetValue(mapIndex);
        }

        private void OnLobbyRoundCountChanged(int roundCount)
        {
            roundCountIncrementer.SetValue(roundCount);
        }

        private void OnLobbyRoundTimeChanged(int roundTime)
        {
            roundTimeIncrementer.SetValue(roundTime);
        }

        private void OnLobbyGameModeChanged(GameMode gameMode)
        {
            gameModeSelector.SetSelection((int)gameMode, true);
        }

        private async void OnLobbyDataChanged(OperationResult result)
        {
            await Task.Delay(1000);

            UpdateEditUpdateButton(isEditing);
            editUpdateButton.interactable = AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId;
        }

        private void UpdateSelections()
        {
            mapChanger.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection((int)Enum.Parse<GameMode>(LobbyManager.Instance.Lobby.Data["GameMode"].Value), true);
        }

        private void UpdateInteractable(bool isEditing)
        {
            mapChanger.UpdateInteractable(isEditing);
            roundCountIncrementer.UpdateInteractable(isEditing);
            roundTimeIncrementer.UpdateInteractable(isEditing);
            gameModeSelector.UpdateInteractable(isEditing);
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
    }
}