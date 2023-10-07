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
    [SerializeField] private Sprite[] _boughtIcons;
    [SerializeField] private Sprite[] _disabledIcons;
    [SerializeField] private Button[] _shopButtons;
    [SerializeField] private TextMeshProUGUI[] _costsText;
    [SerializeField] private TextMeshProUGUI _playerMoney;
    [SerializeField] private GameObject[] _blackTrays;
    [SerializeField] private ShopFramePart[] _shopFrame;
    [SerializeField] private Image[] _shopIcons;
    [SerializeField] private Sprite _outLineForButtons;
    [SerializeField] private Sprite _outLineForButtonsDefault;

    private UIService _menuService;
    private GameState _gameState;
    private TrayControl _tray;
    private GiftsService _giftsService;
    private HandControls _handControls;

    private int _selectedButtonNumber = -1;
    private bool[] _avaivableUpgrades = {true, true, true, false, false, false, false, false,
                                                false, false, false, false, false, false, false };
    private bool[] _avaivableTier = { true, false, false, false, false };
    private int _currentTier = 0;
    private static int[] _costs = { 50, 50, 50, 50, 50, 50, 50,
                                    50, 50, 50, 50, 50, 50, 50, 50};
    private static int[] _giftNumbers = { 0, 4, 5, 1, 6, 7, 0, 8, 
                                          9, 2, 10, 11, 0, 12,
                                          13, -1, -1, -1, -1, -1 };
    

    // DESCRIPTIONS CONTENT
    #region DESCRIPTIONS CONTENT
    private static string[] _descriptions =
    {
        "Навсегда увеличивает размер подноса.",
        "Новый дар!\nУ подноса появляются бортики. Упершись в бортик, еда не упадет на пол.",
        "Новый дар!\nКаждое блюдо имеет шанс появиться в золотой форме. Золотые блюда приносят вдвое больше чаевых.",
        "Загадочное улучшение вашей руки.",
        "Новый дар!\nПозволяет заменить особое пожелание следующего гостя на случайное.",
        "Новый дар!\nТри одинаковых блюда, находящихся рядом, объединяются в золотую версию! Золотые блюда приносят вдвое больше чаевых.",
        "Навсегда увеличивает размер подноса.",
        "Новый дар!\nЗначительно уменьшает скорость вращения подноса!",
        "Новый дар!\nПозволяет выбрать особое пожелание гостя из двух случайных.",
        "Загадочное улучшение вашей руки.",
        "Новый дар!\nПокрывает поднос липкой жижей. Касающиеся подноса, блюда не скользят по нему.",
        "Новый дар!\nЗаменяет все предлагаемые дары на случайные.",
        "Навсегда увеличивает размер подноса.",
        "Новый дар!\nДо конца заказа здоровье не может опуститься ниже 1.",
        "Новый дар!\nТри комка сажи, находящиеся рядом, объединяются в золотую версию!",
        "Сделан из спинки стула. Будет лучше, если гости об этом не узнают...",
        "Пластиковый поднос? Интересно, откуда он в мире духов?",
        "Ммм-металл. Бессмертная классика.",
        "Выглядит роскошно!",
        "Рецепт крафта: расположить на верстаке три алмаза в ряд.",
        "Легендарный Нефритовый Поднос!\nО таком мечтает каждый официант."
    };
    #endregion

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();
        _menuService = ServiceLocator.Current.Get<UIService>();
        _tray = ServiceLocator.Current.Get<TrayControl>();
        _giftsService = ServiceLocator.Current.Get<GiftsService>();
        _handControls = ServiceLocator.Current.Get<HandControls>();

        //_buyButton.onClick.AddListener(_menuService.OnDeathReturnToShop);
        _backButton.onClick.AddListener(_menuService.ShowMainMenu);


        for (int i = 0; i < _shopButtons.Length; i++)
        {
            _shopButtons[i].GetComponent<ButtonValue>().SetButtonParametrs(this, i);
            _shopButtons[i].onClick.AddListener(_shopButtons[i].GetComponent<ButtonValue>().SetDescription);

            if (i < 15)
            {
                _costsText[i].text = $"¥{_costs[i]}";
                if(i < 3)
                    _shopIcons[i].overrideSprite = _descriptionIcons[i];
                else
                    _shopIcons[i].overrideSprite = _disabledIcons[i];
                _shopIcons[i].SetNativeSize();
            }
        }

        _buyButton.GetComponent<Button>().onClick.AddListener(TryBuy);
        _buyButton.GetComponent<Button>().interactable = false;

        _switchSkinButton.GetComponent<Button>().onClick.AddListener(TrySwitchSkin);
        _switchSkinButton.SetActive(false);

        UpdatePlayerMoneyCounter();
    }

    public void SetDescription(int index)
    {
        ServiceLocator.Current.Get<AudioService>().PlaySelect();

        _descriptionText.text = _descriptions[index];

        _selectedButtonNumber = index;

        if (index < 15)
        {
            _buyButton.SetActive(true);
            _switchSkinButton.SetActive(false);


            if (_avaivableTier[index/3] && _avaivableUpgrades[index] 
                && _gameState.Money >= _costs[_selectedButtonNumber])
            {
                _buyButton.GetComponent<Button>().interactable = true;
                _descriptionIcon.overrideSprite = _descriptionIcons[index];
                _priceText.text = $"¥{_costs[index]}";
            }
            else
            {
                _buyButton.GetComponent<Button>().interactable = false;
                if(!_avaivableTier[index / 3])
                {
                    _descriptionIcon.overrideSprite = _disabledIcons[index];
                    _priceText.text = $"¥{_costs[index]}";
                }
                else 
                {
                    if(_avaivableTier[index / 3] && !_avaivableUpgrades[index])
                    {
                        _descriptionIcon.overrideSprite = _boughtIcons[index];
                        _priceText.text = $"";
                    }
                }
            }
        }
        else
        {
            _descriptionIcon.overrideSprite = _descriptionIcons[index];
            _priceText.text = $"";
            _buyButton.SetActive(false);
            _switchSkinButton.SetActive(true);

            if (_currentTier >= index - 15)
            {
                _switchSkinButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                _switchSkinButton.GetComponent<Button>().interactable = false;
            }
        }

        _descriptionIcon.SetNativeSize();
    }

    public void UpdatePlayerMoneyCounter()
    {
        if (_gameState.InRun)
        {
            _playerMoney.text = $"Смена не окончена!";
            _buyButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            _playerMoney.text = $"Мои сбережения: ¥{_gameState.Money}";
            _buyButton.GetComponent<Button>().interactable = true;
        }
    }

    private void TryBuy()
    {
        ServiceLocator.Current.Get<AudioService>().PlayBuyPress();
        _gameState.AddMoney(-_costs[_selectedButtonNumber]);
        _avaivableUpgrades[_selectedButtonNumber] = false;
        UpdatePlayerMoneyCounter();

        // buy stuff
        if (_giftNumbers[_selectedButtonNumber] > 3)
            _giftsService.AddGiftInPool(_giftNumbers[_selectedButtonNumber]);
        else
        {
            if (_giftNumbers[_selectedButtonNumber] == 0)
            {
                _tray.IncreaseTrayWidth();
                ServiceLocator.Current.Get<Tetris>().IncreaseGridWidth();
            }
            else
            {
                if (_giftNumbers[_selectedButtonNumber] == 1)
                    _handControls.SetTripleTentacle();
                else if (_giftNumbers[_selectedButtonNumber] == 2)
                    _handControls.SetTentacle();
            }
        }

        _shopIcons[_selectedButtonNumber].overrideSprite = _boughtIcons[_selectedButtonNumber];
        _shopIcons[_selectedButtonNumber].SetNativeSize();
        _costsText[_selectedButtonNumber].text = $"";
        _shopButtons[_selectedButtonNumber].GetComponent<Image>().overrideSprite = _outLineForButtons;
        _shopButtons[_selectedButtonNumber].GetComponent<Image>().SetNativeSize();

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

                _shopIcons[_currentTier * 3].overrideSprite = _descriptionIcons[_currentTier * 3];
                _shopIcons[_currentTier * 3].SetNativeSize();
                _shopIcons[_currentTier * 3 + 1].overrideSprite = _descriptionIcons[_currentTier * 3 + 1];
                _shopIcons[_currentTier * 3 + 1].SetNativeSize();
                _shopIcons[_currentTier * 3 + 2].overrideSprite = _descriptionIcons[_currentTier * 3 + 2];
                _shopIcons[_currentTier * 3 + 2].SetNativeSize();

                _avaivableTier[_currentTier] = true;
                _shopFrame[_currentTier].SetGray();
            }
            else
            {
                _currentTier++;
            }
        }

        SetDescription(_selectedButtonNumber);
    }

    private void TrySwitchSkin()
    {
        _tray.SetSkin(_selectedButtonNumber - 15);
    }

    public void ResetShop()
    {
        _currentTier = 0;
        _switchSkinButton.SetActive(false);
        _buyButton.SetActive(true);
        _buyButton.GetComponent<Button>().interactable = false;

        _tray.ResetTrayWidth();
        _handControls.ResetHand();
        _giftsService.ResetGiftPool();

        for (int i = 0; i < _shopButtons.Length; i++)
        {
            if (i < 15)
            {
                _costsText[i].text = $"¥{_costs[i]}";
                if (i < 3)
                {
                    _shopIcons[i].overrideSprite = _descriptionIcons[i];
                    _avaivableUpgrades[i] = true;
                }
                else
                {
                    _shopIcons[i].overrideSprite = _disabledIcons[i];
                    _avaivableUpgrades[i] = false;
                }
                _shopIcons[i].SetNativeSize();
                _shopButtons[i].GetComponent<Image>().overrideSprite = _outLineForButtonsDefault;
                _shopButtons[i].GetComponent<Image>().SetNativeSize();
            }
            else
            {
                if (i < 20)
                {
                    _shopFrame[i - 15].ResetAll();
                    _avaivableTier[i - 15] = false;

                    if (i == 15)
                    {
                        _shopFrame[i - 15].SetGray();
                        _avaivableTier[i - 15] = true;
                    }

                    _blackTrays[i - 15].SetActive(true);
                }
            }
        }
    }
}
