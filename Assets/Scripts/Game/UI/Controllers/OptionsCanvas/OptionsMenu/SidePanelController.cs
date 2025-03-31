using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Game.UI.Controllers.OptionsCanvas.OptionsMenu;

public class SidePanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private OptionsPanelController optionsPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        optionsPanel.ExpandPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        optionsPanel.CollapsePanel();
    }
}