using UnityEngine;
using UnityEngine.UI;

public class ShopUIService : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _backButton;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        //_buyButton.onClick.AddListener(_menuService.OnDeathReturnToShop);
        _backButton.onClick.AddListener(_menuService.ShowMainMenu);
    }
}
