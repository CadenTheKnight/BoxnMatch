using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Types;
using Assets.Scripts.Game.Managers;
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
        [SerializeField] private MapSelectionData mapSelectionData;

        private bool isEditing = false;

        private void OnEnable()
        {
            SetPanelState(false);
            UpdateSelections();
            UpdateEditUpdateButton(isEditing);

            editUpdateButton.onClick.AddListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyRefreshed += OnLobbyUpdated;
            // LobbyEvents.OnLobbyDataUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            editUpdateButton.onClick.RemoveListener(OnEditUpdateClicked);

            LobbyEvents.OnLobbyRefreshed -= OnLobbyUpdated;
            // LobbyEvents.OnLobbyDataUpdated -= OnLobbyUpdated;
        }

        private void OnEditUpdateClicked()
        {
            UpdateEditUpdateButton(isEditing);
            if (AuthenticationManager.Instance.LocalPlayer.Id == LobbyManager.Instance.Lobby.HostId)
                SetPanelState(!isEditing);
        }

        private async void SetPanelState(bool isEditing)
        {
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

                editUpdateButton.interactable = false;
                editUpdateText.text = "Updating...";

                await Task.Delay(1500);
                GameLobbyManager.Instance.UpdateLobbyData(mapChanger.Value, roundCountIncrementer.Value, roundTimeIncrementer.Value, (GameMode)gameModeSelector.GetSelectedIndices()[0]);

                editUpdateButton.interactable = true;
                editUpdateText.text = "EDIT";
            }

            this.isEditing = isEditing;
        }

        private void UpdateEditUpdateButton(bool isEditing)
        {
            editUpdateButton.gameObject.SetActive(LobbyManager.Instance.Lobby.HostId == AuthenticationManager.Instance.LocalPlayer.Id);
            editUpdateText.text = isEditing ? "UPDATE" : "EDIT";

            ColorBlock colors = editUpdateButton.colors;
            colors.normalColor = isEditing ? UIColors.greenDefaultColor : UIColors.secondaryDefaultColor;
            colors.highlightedColor = isEditing ? UIColors.greenHoverColor : UIColors.secondaryHoverColor;
            colors.pressedColor = isEditing ? UIColors.greenDefaultColor : UIColors.secondaryPressedColor;
            colors.selectedColor = isEditing ? UIColors.greenHoverColor : UIColors.secondaryHoverColor;
            colors.disabledColor = UIColors.secondaryDisabledColor;

            editUpdateButton.colors = colors;
        }

        private void OnLobbyUpdated()
        {
            if (!isEditing) UpdateSelections();
            UpdateEditUpdateButton(isEditing);
        }

        public void UpdateSelections()
        {
            mapChanger.SetSelection(int.Parse(LobbyManager.Instance.Lobby.Data["MapIndex"].Value));
            roundCountIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundCount"].Value));
            roundTimeIncrementer.SetValue(int.Parse(LobbyManager.Instance.Lobby.Data["RoundTime"].Value));
            gameModeSelector.SetSelection(int.Parse(LobbyManager.Instance.Lobby.Data["GameMode"].Value), true);
        }
    }
}