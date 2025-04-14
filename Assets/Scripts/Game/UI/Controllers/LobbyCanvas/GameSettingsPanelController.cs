using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Assets.Scripts.Game.Events;

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

        private void Start()
        {
            LobbyEvents.OnPlayerConnected += OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected += OnPlayerDisconnect;

            if (GameLobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

                LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
                GameLobbyEvents.OnGameSettingsChanged += UpdateSelections;

                UpdateSelections();
                UpdateOptionsInteractable(isEditing);
                UpdateEditUpdateButtonState(isEditing);
                editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

                LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
                GameLobbyEvents.OnGameSettingsChanged -= UpdateSelections;
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.OnPlayerConnected -= OnPlayerConnect;
            LobbyEvents.OnPlayerDisconnected -= OnPlayerDisconnect;

            editUpdateLoadingBar.StopLoading();
        }

        private async void OnEditUpdateClicked()
        {
            isEditing = !isEditing;
            if (!isEditing)
            {
                try
                {
                    editUpdateButton.interactable = false;
                    editUpdateText.text = "Updating...";
                    editUpdateLoadingBar.StartLoading();
                    UpdateOptionsInteractable(!isEditing);

                    Task<OperationResult> updateGameSettingsTask = GameLobbyManager.Instance.UpdateGameSettings(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, gameModeSelector.Selection);
                    await Task.WhenAny(updateGameSettingsTask, Task.Delay(5000));

                    if (updateGameSettingsTask.IsCompletedSuccessfully && updateGameSettingsTask.Result.Status == ResultStatus.Success)
                    {
                        UpdateSelections();

                        GameLobbyEvents.InvokeGameSettingsChanged();
                    }
                }
                finally
                {
                    editUpdateLoadingBar.StopLoading();

                    UpdateEditUpdateButtonState(isEditing);
                    UpdateOptionsInteractable(isEditing);

                    await Task.Delay(1000);

                    editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;
                }
            }
            else
            {
                UpdateOptionsInteractable(isEditing);
                UpdateEditUpdateButtonState(isEditing);
            }
        }

        private void OnLobbyHostMigrated(string playerId)
        {
            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == playerId;
        }

        private void UpdateSelections()
        {
            mapChanger.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection(int.Parse(GameLobbyManager.Instance.Lobby.Data["GameMode"].Value));
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