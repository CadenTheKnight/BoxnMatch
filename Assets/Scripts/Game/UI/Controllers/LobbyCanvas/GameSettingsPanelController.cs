using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
using Unity.Services.Lobbies.Models;
using Assets.Scripts.Game.UI.Colors;
using Assets.Scripts.Framework.Events;
using Assets.Scripts.Framework.Managers;
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

            LobbyEvents.OnLobbyDataChanged += OnLobbyDataChanged;
            LobbyEvents.OnPlayerDataChanged += OnPlayerDataChanged;
        }

        private void OnDisable()
        {
            editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyDataChanged -= OnLobbyDataChanged;
            LobbyEvents.OnPlayerDataChanged -= OnPlayerDataChanged;
        }
        private async void OnEditUpdateClicked()
        {
            if (AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId)
                UpdateInteractable(!isEditing);
            if (isEditing)
            {
                editUpdateText.text = "Updating...";
                editUpdateButton.interactable = false;

                LobbyData lobbyData = new() { MapIndex = mapChanger.Value, RoundCount = roundCountIncrementer.Value, RoundTime = roundTimeIncrementer.Value, GameMode = (GameMode)gameModeSelector.Selection };
                await LobbyManager.Instance.UpdateLobbyData(lobbyData.Serialize());

                await Task.Delay(1000);

                editUpdateButton.interactable = true;
            }

            UpdateEditUpdateButton(!isEditing);
            isEditing = !isEditing;
        }

        private void OnPlayerDataChanged(Player player, string key, string value)
        {
            Debug.Log($"Player data changed (in controller): {key} = {value}");
            editUpdateButton.interactable = player.Id == LobbyManager.Instance.Lobby.HostId;
            if (!isEditing) UpdateSelections();

        }

        private void OnLobbyDataChanged(string key, string value)
        {
            Debug.Log($"Lobby data changed (in controller): {key} = {value}");
            UpdateSelections();
        }


        public void UpdateSelections()
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