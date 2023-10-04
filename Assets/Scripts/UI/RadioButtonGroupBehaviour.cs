using UnityEngine;
using UnityEngine.Events;

public class RadioButtonGroupBehaviour : MonoBehaviour
{
    public int SelectedButton { get; private set; } = -1;

    [SerializeField] public UnityEvent<bool> _onSelected;
    [SerializeField] ButtonWithTooltipScript[] _buttons;

    public void SetSelectedButton(int number)
    {
        SelectedButton = number;
        if(number >= 0)
        {
            for (int i = 0; i < _buttons.Length; ++i)
                if(number != i)
                    _buttons[i].SetSelectState(false);
            _onSelected?.Invoke(true);
        }
        else
        {
            _onSelected?.Invoke(false);
        }
    }

    public void ResetAllButtons()
    {
        for (int i = 0; i < _buttons.Length; ++i)
            _buttons[i].SetSelectState(false);
        SelectedButton = -1;
    }

}
