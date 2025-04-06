using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Types;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Managers;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Unity.Services.Authentication;

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

            if (LobbyManager.Instance.Lobby != null) OnPlayerConnect(AuthenticationService.Instance.PlayerId);
        }

        private void OnPlayerConnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

                LobbyEvents.OnLobbyHostMigrated += OnLobbyHostMigrated;
                LobbyEvents.OnLobbyMapIndexChanged += OnLobbyMapIndexChanged;
                LobbyEvents.OnLobbyRoundCountChanged += OnLobbyRoundCountChanged;
                LobbyEvents.OnLobbyRoundTimeChanged += OnLobbyRoundTimeChanged;
                LobbyEvents.OnLobbyGameModeChanged += OnLobbyGameModeChanged;

                UpdateSelections();
                UpdateInteractable(isEditing);
                UpdateEditUpdateButtonState(isEditing);
                UpdateEditUpdateButtonInteractable();
            }
        }

        private void OnPlayerDisconnect(string playerId)
        {
            if (playerId == AuthenticationService.Instance.PlayerId)
            {
                editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

                LobbyEvents.OnLobbyHostMigrated -= OnLobbyHostMigrated;
                LobbyEvents.OnLobbyMapIndexChanged -= OnLobbyMapIndexChanged;
                LobbyEvents.OnLobbyRoundCountChanged -= OnLobbyRoundCountChanged;
                LobbyEvents.OnLobbyRoundTimeChanged -= OnLobbyRoundTimeChanged;
                LobbyEvents.OnLobbyGameModeChanged -= OnLobbyGameModeChanged;
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
            if (isEditing)
            {
                editUpdateButton.interactable = false;
                editUpdateText.text = "Updating...";
                editUpdateLoadingBar.StartLoading();

                if (mapChanger.Value != int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value) ||
                    roundCountIncrementer.Value != int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value) ||
                    roundTimeIncrementer.Value != int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value) ||
                    gameModeSelector.Selection != int.Parse(LobbyManager.Instance.Lobby.Data["GameMode"].Value))
                {
                    OperationResult result = await GameLobbyManager.Instance.UpdateGameSettings(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, gameModeSelector.Selection);

                    editUpdateLoadingBar.StopLoading();
                    if (result.Status == ResultStatus.Success) editUpdateText.text = "Updated";
                    else editUpdateText.text = "Error";

                    await Task.Delay(1000);
                }
                else
                {
                    editUpdateLoadingBar.StopLoading();
                    editUpdateText.text = "No Changes";
                }

                editUpdateButton.interactable = true;
            }

            isEditing = !isEditing;
            UpdateEditUpdateButtonState(isEditing);
            UpdateInteractable(isEditing);
        }

        private void OnLobbyHostMigrated(string playerId)
        {
            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == playerId;
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

        private void UpdateSelections()
        {
            mapChanger.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection(int.Parse(LobbyManager.Instance.Lobby.Data["GameMode"].Value), true);
        }

        private void UpdateInteractable(bool isEditing)
        {
            mapChanger.UpdateInteractable(isEditing);
            roundCountIncrementer.UpdateInteractable(isEditing);
            roundTimeIncrementer.UpdateInteractable(isEditing);
            gameModeSelector.UpdateInteractable(isEditing);
        }

        private void UpdateEditUpdateButtonInteractable()
        {
            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == LobbyManager.Instance.Lobby.HostId;

        }

        private void UpdateEditUpdateButtonState(bool isEditing)
        {
            editUpdateText.text = isEditing ? "UPDATE" : "EDIT";

            ColorBlock colors = editUpdateButton.colors;
            colors.normalColor = isEditing ? UIColors.Green.One : UIColors.Primary.Eight;
            colors.highlightedColor = isEditing ? UIColors.Green.Two : UIColors.Primary.Six;
            colors.pressedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
            colors.selectedColor = isEditing ? UIColors.Green.Three : UIColors.Primary.Four;
            colors.disabledColor = UIColors.Primary.Three;

            editUpdateButton.colors = colors;
        }
    }
}