using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithTooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _tooltip;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _disableSprite;
    [SerializeField] public UnityEvent<int> _onClick;
    [SerializeField] private int _number = 0;

    private bool _enableStatus = true;
    private bool _selectStatus = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_enableStatus)
        {
            SetSelectState(!_selectStatus);
            _onClick?.Invoke(_selectStatus ? _number : -1);
        }
    }

    public void SetSelectState(bool selectState)
    {
        _selectStatus = selectState;
        if (_selectStatus)
            gameObject.GetComponent<Image>().overrideSprite = _selectedSprite;
        else
            gameObject.GetComponent<Image>().overrideSprite = _defaultSprite;
    }

    public void SetEnable(bool enableState)
    {
        _enableStatus = enableState;
        if (_enableStatus)
            gameObject.GetComponent<Image>().overrideSprite = _defaultSprite;
        else
            gameObject.GetComponent<Image>().overrideSprite = _disableSprite;
    }

    public void SetTooltip(GameObject toopltip)
    {
        _tooltip = toopltip;
    }

}
