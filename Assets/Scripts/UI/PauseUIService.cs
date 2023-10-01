using UnityEngine;
using UnityEngine.UI;

public class PauseUIService : MonoBehaviour
{
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _giveUpButton;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _continueGameButton.onClick.AddListener(_menuService.OnPressPause);
        _giveUpButton.onClick.AddListener(() => _menuService.ShowDeathScreen());
    }
}
