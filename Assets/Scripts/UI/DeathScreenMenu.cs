using UnityEngine;
using UnityEngine.UI;

public class DeathScreenMenu : MonoBehaviour
{
    [SerializeField] private Button _toShopButton;
    [SerializeField] private Button _toMenuButton;

    private MenuService _menuService;

    private void Start()
    {
        _menuService =  ServiceLocator.Current.Get<MenuService>();
        _toShopButton.onClick.AddListener(_menuService.OnDeathReturnToShop);
        _toMenuButton.onClick.AddListener(_menuService.OnDeathReturnToMenu);
    }
}
