using UnityEngine;
using UnityEngine.UI;

public class UpgradesUIService : MonoBehaviour
{
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questInfoBlock;
    [SerializeField] private GameObject _giftsInfoBlock;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _acceptGiftButton;

    private MenuService _menuService;

    private void Awake()
    {
        _upgradesMenu.SetActive(false);
        _questInfoBlock.SetActive(false);
        _giftsInfoBlock.SetActive(false);
    }

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<MenuService>();
        _continueButton.onClick.AddListener(() => { 
            _questInfoBlock.SetActive(false);
            _giftsInfoBlock.SetActive(true);
        });
        _acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
    }

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
        _questInfoBlock.SetActive(true);
    }
}
