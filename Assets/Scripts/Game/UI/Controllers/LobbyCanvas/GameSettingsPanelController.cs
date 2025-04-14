using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Authentication;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Game.UI.Components;
using Assets.Scripts.Framework.Utilities;
using Assets.Scripts.Game.UI.Components.Options;
using Assets.Scripts.Game.UI.Components.Options.Selector;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using Assets.Scripts.Framework.Types;

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
                LobbyEvents.OnLobbyDataUpdated += OnLobbyDataUpdated;

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
                LobbyEvents.OnLobbyDataUpdated -= OnLobbyDataUpdated;
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
                editUpdateButton.interactable = false;
                editUpdateText.text = "Updating...";
                editUpdateLoadingBar.StartLoading();
                UpdateOptionsInteractable(!isEditing);

                if (mapChanger.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value) ||
                    roundCountIncrementer.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value) ||
                    roundTimeIncrementer.Value != int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value) ||
                    gameModeSelector.Selection != int.Parse(GameLobbyManager.Instance.Lobby.Data["GameMode"].Value))
                { await GameLobbyManager.Instance.UpdateGameSettings(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, gameModeSelector.Selection); }
                else { OnLobbyDataUpdated(OperationResult.SuccessResult("NoChanges", null, "No changes made.")); }
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

        private async void OnLobbyDataUpdated(OperationResult result)
        {
            if (result.Status == ResultStatus.Success && result.Data is Dictionary<string, DataObject> dataDict)
            {
                if (dataDict.ContainsKey("MapIndex")) mapChanger.SetValue(int.Parse(dataDict["MapIndex"].Value));
                if (dataDict.ContainsKey("RoundCount")) roundCountIncrementer.SetValue(int.Parse(dataDict["RoundCount"].Value));
                if (dataDict.ContainsKey("RoundTime")) roundTimeIncrementer.SetValue(int.Parse(dataDict["RoundTime"].Value));
                if (dataDict.ContainsKey("GameMode")) gameModeSelector.SetSelection(int.Parse(dataDict["GameMode"].Value), true);
            }

            editUpdateLoadingBar.StopLoading();
            UpdateEditUpdateButtonState(isEditing);
            UpdateOptionsInteractable(isEditing);

            await Task.Delay(1000);

            editUpdateButton.interactable = AuthenticationService.Instance.PlayerId == GameLobbyManager.Instance.Lobby.HostId;
        }

        private void UpdateSelections()
        {
            mapChanger.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(GameLobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection(int.Parse(GameLobbyManager.Instance.Lobby.Data["GameMode"].Value), true);
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
            editUpdateText.text = isEditing ? "UPDATE" : "EDIT";

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