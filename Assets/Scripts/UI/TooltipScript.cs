using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _tooltip;
    private GameObject _tooltipDefault;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.SetActive(false);
    }

    public void SetToolTip(GameObject tooltip)
    {
        _tooltipDefault = _tooltip;
        _tooltip = tooltip;
    }

    public void ResetToolTip()
    {
        _tooltip = _tooltipDefault;
    }
}
