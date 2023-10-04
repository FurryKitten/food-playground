using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _tooltip;
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
        _tooltip = tooltip;
    }
}
