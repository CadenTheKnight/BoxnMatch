using UnityEngine;
using Assets.Scripts.Game.UI.Components.Options.Selector;

namespace Assets.Scripts.Game.UI.Controllers.MainCanvas.LobbyPanel.CharacterPanel
{
    public class CharacterSettingsPanelController : MonoBehaviour
    {
        [SerializeField] private Selector colorAreaSelector;
        [SerializeField] private Selector colorSelector;
        [SerializeField] private Selector faceSelector;
        [SerializeField] private Selector boxSelector;

        public Selector ColorAreaSelector => colorAreaSelector;
        public Selector ColorSelector => colorSelector;
        public Selector FaceSelector => faceSelector;
        public Selector BoxSelector => boxSelector;

        private void Start()
        {
            colorAreaSelector.SetSelection(0, true);
            colorSelector.SetSelection(0, true);
            faceSelector.SetSelection(0, true);
            boxSelector.SetSelection(0, true);
        }

        private void OnEnable()
        {
            colorAreaSelector.OnSelectionChanged += SetColorArea;
            colorSelector.OnSelectionChanged += SetColor;
            faceSelector.OnSelectionChanged += SetFace;
            boxSelector.OnSelectionChanged += SetBox;
        }

        private void OnDisable()
        {
            colorAreaSelector.OnSelectionChanged -= SetColorArea;
            colorSelector.OnSelectionChanged -= SetColor;
            faceSelector.OnSelectionChanged -= SetFace;
            boxSelector.OnSelectionChanged -= SetBox;
        }

        private void SetColorArea(int selection)
        {
            Debug.Log($"Color area changed to: {selection}");
        }

        private void SetColor(int selection)
        {
            Debug.Log($"Color changed to: {selection}");
        }

        private void SetFace(int selection)
        {
            Debug.Log($"Face changed to: {selection}");
        }

        private void SetBox(int selection)
        {
            Debug.Log($"Box changed to: {selection}");
        }
    }
}
