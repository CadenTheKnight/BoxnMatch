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
}