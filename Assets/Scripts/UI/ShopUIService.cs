using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIService : MonoBehaviour
{
    [SerializeField] private GameObject _buyButton;
    [SerializeField] private GameObject _switchSkinButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Image _descriptionIcon;
    [SerializeField] private Sprite[] _descriptionIcons;
    [SerializeField] private Button[] _shopButtons;
    [SerializeField] private TextMeshProUGUI[] _costsText;
    [SerializeField] private TextMeshProUGUI _playerMoney;
    [SerializeField] private GameObject[] _blackTrays;
    [SerializeField] private ShopFramePart[] _shopFrame;

    private UIService _menuService;
    private GameState _gameState;
    private TrayControl _tray;

    private int _selectedButtonNumber = -1;
    private bool[] _avaivableUpgrades = {true, true, true, false, false, false, false, false,
                                                false, false, false, false, false, false, false };
    private bool[] _avaivableTier = { true, false, false, false, false };
    private int _currentTier = 0;
    private static int[] _costs = { 300, 300, 300, 600, 600, 600, 900,
                                    900, 900, 1200, 1200, 1200, 1500, 1500, 1500};
   
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
        _gameState = ServiceLocator.Current.Get<GameState>();
        _menuService = ServiceLocator.Current.Get<UIService>();
        _tray = ServiceLocator.Current.Get<TrayControl>();
        //_buyButton.onClick.AddListener(_menuService.OnDeathReturnToShop);
        _backButton.onClick.AddListener(_menuService.ShowMainMenu);
        
        for(int i = 0; i < _shopButtons.Length; i++)
        {
            _shopButtons[i].GetComponent<ButtonValue>().SetButtonParametrs(this, i);
            _shopButtons[i].onClick.AddListener(_shopButtons[i].GetComponent<ButtonValue>().SetDescription);
            
            if(i < 15)
                _costsText[i].text = $"¥{_costs[i]}";
        }

        _buyButton.GetComponent<Button>().onClick.AddListener(TryBuy);
        _buyButton.GetComponent<Button>().interactable = false;

        _switchSkinButton.GetComponent<Button>().onClick.AddListener(TrySwitchSkin);
        _switchSkinButton.SetActive(false);

        UpdatePlayerMoneyCounter();
    }

    public void SetDescription(int index)
    {
        _descriptionText.text = _descriptions[index];
        _descriptionIcon.overrideSprite = _descriptionIcons[index];
        _descriptionIcon.SetNativeSize();

        _selectedButtonNumber = index;

        if (index < 15)
        {
            _priceText.text = $"¥{_costs[index]}";
            _buyButton.SetActive(true);
            _switchSkinButton.SetActive(false);

            if (_avaivableTier[index/3] && _avaivableUpgrades[index] 
                && _gameState.Money >= _costs[_selectedButtonNumber])
            {
                _buyButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                _buyButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            _priceText.text = "";
            _buyButton.SetActive(false);
            _switchSkinButton.SetActive(true);

            if (_currentTier == index - 15)
            {
                _switchSkinButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                _switchSkinButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void UpdatePlayerMoneyCounter()
    {
        _playerMoney.text = $"Мои сбережения: ¥{_gameState.Money}";
    }

    private void TryBuy()
    {
        _gameState.AddMoney(-_costs[_selectedButtonNumber]);
        _avaivableUpgrades[_selectedButtonNumber] = false;
        UpdatePlayerMoneyCounter();
        // buy stuff

        if (!_avaivableUpgrades[_currentTier * 3] && !_avaivableUpgrades[_currentTier * 3 + 1]
            && !_avaivableUpgrades[_currentTier * 3 + 2])
        {
            //switch shop frame
            _shopFrame[_currentTier].ResetAll();
            _shopFrame[_currentTier].SetGold();
            _blackTrays[_currentTier].SetActive(false);

            if (_currentTier < 4)
            {
                _currentTier++;
                _avaivableUpgrades[_currentTier * 3] = true;
                _avaivableUpgrades[_currentTier * 3 + 1] = true;
                _avaivableUpgrades[_currentTier * 3 + 2] = true;
                _avaivableTier[_currentTier] = true;
                _shopFrame[_currentTier].SetGray();
            }
        }

    }

    private void TrySwitchSkin()
    {
        _tray.SetSkin(_selectedButtonNumber - 15);
    }

}
