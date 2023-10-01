using UnityEngine;
using UnityEngine.UI;

public class DeathScreenMenu : MonoBehaviour
{
    [SerializeField] private Button _toMenuButton;

    private UIService _menuService;

    private void Start()
    {
        _menuService =  ServiceLocator.Current.Get<UIService>();
        _toMenuButton.onClick.AddListener(_menuService.OnDeathReturnToMenu);
    }
}
