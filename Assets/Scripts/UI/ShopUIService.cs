using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIService : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Image _descriptionIcon;
    [SerializeField] private Sprite[] _descriptionIcons;
    [SerializeField] private Button[] _shopButtons;

    private UIService _menuService;
    // DESCRIPTIONS CONTENT
    #region DESCRIPTIONS CONTENT
    private static string[] _descriptions =
    {
        "0", 
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "tray 0",
        "tray 1",
        "tray 2",
        "tray 3",
        "tray 4",
        "tray 5"
    };
    #endregion

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        //_buyButton.onClick.AddListener(_menuService.OnDeathReturnToShop);
        _backButton.onClick.AddListener(_menuService.ShowMainMenu);
        
        for(int i = 0; i < _shopButtons.Length; i++)
        {
            _shopButtons[i].GetComponent<ButtonValue>().SetButtonParametrs(this, i);
            _shopButtons[i].onClick.AddListener(_shopButtons[i].GetComponent<ButtonValue>().SetDescription);
        }
    }

    public void SetDescription(int index)
    {
        _descriptionText.text = _descriptions[index];
        _descriptionIcon.overrideSprite = _descriptionIcons[index];
        _descriptionIcon.SetNativeSize();

        if (index < 15)
            _priceText.text = "300";
        else
            _priceText.text = "";
    }

}
