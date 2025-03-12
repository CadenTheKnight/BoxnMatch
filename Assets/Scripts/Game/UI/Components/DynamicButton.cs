using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DynamicButton : Button
{
    TextMeshProUGUI text;

    protected override void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        base.Awake();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        text.color = colors.normalColor;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        text.color = colors.highlightedColor;
        base.OnPointerExit(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        text.color = colors.normalColor;
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        text.color = colors.normalColor;
        base.OnPointerUp(eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        text.color = colors.normalColor;
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        text.color = colors.highlightedColor;
        base.OnDeselect(eventData);
    }
}