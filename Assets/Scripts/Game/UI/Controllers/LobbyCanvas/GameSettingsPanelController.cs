using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Events;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;

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
        [SerializeField] private LoadingBar editUpdateLoadingBar;
        [SerializeField] private TextMeshProUGUI editUpdateText;

        private bool isEditing = false;

        private void OnEnable()
        {
            editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

            LobbyEvents.OnHostMigrated += OnHostMigrated;
            GameLobbyEvents.OnGameSettingsChanged += OnGameSettingsChanged;
            GameLobbyEvents.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;

            UpdateSelections(GameLobbyManager.Instance.Lobby.Data);
            UpdateOptionsInteractable(isEditing);
            UpdateEditUpdateButtonState(isEditing);
            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;
        }

        private void OnDisable()
        {
            editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

            LobbyEvents.OnHostMigrated -= OnHostMigrated;
            GameLobbyEvents.OnGameSettingsChanged -= OnGameSettingsChanged;
            GameLobbyEvents.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;

            editUpdateLoadingBar.StopLoading();
        }

        private async void OnEditUpdateClicked()
        {
            isEditing = !isEditing;
            if (!isEditing)
            {

                editUpdateButton.interactable = false;
                editUpdateText.text = "Updating...";
                editUpdateLoadingBar.StartLoading();
                UpdateOptionsInteractable(!isEditing);

                await GameLobbyManager.Instance.UpdateGameSettings(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, gameModeSelector.Selection);
            }
            else
            {
                UpdateOptionsInteractable(isEditing);
                UpdateEditUpdateButtonState(isEditing);
            }
        }

        private void OnHostMigrated(string playerId)
        {
            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == playerId;
        }

        private async void OnGameSettingsChanged(bool success, Dictionary<string, DataObject> lobbyData)
        {
            editUpdateLoadingBar.StopLoading();
            UpdateEditUpdateButtonState(isEditing);
            UpdateOptionsInteractable(isEditing);

            if (success) UpdateSelections(lobbyData);

            await Task.Delay(1000);

            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId
                && GameLobbyManager.Instance.Lobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId).Data["ReadyStatus"].Value == ((int)ReadyStatus.NotReady).ToString();
        }

        private void OnPlayerReadyStatusChanged(bool success, string playerId, ReadyStatus readyStatus)
        {
            if (success && playerId == AuthenticationService.Instance.PlayerId)
            {
                editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId && readyStatus == ReadyStatus.NotReady;
                if (readyStatus == ReadyStatus.Ready)
                {
                    if (isEditing) UpdateSelections(GameLobbyManager.Instance.Lobby.Data);
                    isEditing = false;
                    UpdateEditUpdateButtonState(isEditing);
                    UpdateOptionsInteractable(isEditing);
                }
            }
        }

        private void UpdateSelections(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.ContainsKey("MapIndex")) mapChanger.SetValue(int.Parse(lobbyData["MapIndex"].Value));
            if (lobbyData.ContainsKey("RoundCount")) roundCountIncrementer.SetValue(int.Parse(lobbyData["RoundCount"].Value));
            if (lobbyData.ContainsKey("RoundTime")) roundTimeIncrementer.SetValue(int.Parse(lobbyData["RoundTime"].Value));
            if (lobbyData.ContainsKey("GameMode")) gameModeSelector.SetSelection(int.Parse(lobbyData["GameMode"].Value));
        }

        private void UpdateOptionsInteractable(bool isEditing)
        {
            mapChanger.UpdateInteractable(isEditing);
            roundCountIncrementer.UpdateInteractable(isEditing);
            roundTimeIncrementer.UpdateInteractable(isEditing);
            gameModeSelector.UpdateInteractable(isEditing);
        }

        private void UpdateEditUpdateButtonState(bool isEditing)
        {
            editUpdateText.text = isEditing ? "Update" : "Edit Game";

            ColorBlock colors = editUpdateButton.colors;
            colors.normalColor = isEditing ? UIColors.Green.One : UIColors.Primary.Eight;
            colors.highlightedColor = isEditing ? UIColors.Green.Two : UIColors.Primary.Six;
            colors.pressedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
            colors.selectedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
            colors.disabledColor = isEditing ? UIColors.Green.Five : UIColors.Primary.Three;

            editUpdateButton.colors = colors;
        }
    }
}