using UnityEngine;
using UnityEngine.UI;

public class UpgradesUIService : MonoBehaviour
{
    [SerializeField] private Sprite[] _icons;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questInfoBlock;
    [SerializeField] private GameObject _giftsInfoBlock;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _acceptGiftButton;
    [SerializeField] private RadioButtonGroupBehaviour _giftsRadioGroup;
    [SerializeField] ButtonWithTooltipScript[] _giftsButtons;
    [SerializeField] private Image[] _giftsImages;
    [SerializeField] private GameObject[] _giftsTooltips;
    [SerializeField] private Sprite _failedQuestIcon;
    [SerializeField] private GameObject _failedQuestTooltip;
    [SerializeField] private GameObject _questReroll;
    [SerializeField] private GameObject _secondQuest;

    private UIService _menuService;
    private bool _questDone = true; // TO DO: use Locator
    private Vector3Int _gifts = new Vector3Int(-1, -1, -1);

    private void Awake()
    {
        _upgradesMenu.SetActive(false);
        _questInfoBlock.SetActive(false);
        _giftsInfoBlock.SetActive(false);
    }

    private void Start()
    {
        ServiceLocator.Current.Get<GiftsService>().ResetGiftPool();
        _menuService = ServiceLocator.Current.Get<UIService>();
        _continueButton.onClick.AddListener(() => { 
            _questInfoBlock.SetActive(false);
            _giftsInfoBlock.SetActive(true);
            _giftsRadioGroup.ResetAllButtons();

            if(ServiceLocator.Current.Get<GameState>().LastHealthGift)
            {
                ServiceLocator.Current.Get<GameState>().LastHealthGift = false;
                ServiceLocator.Current.Get<GameState>().ChangeHealth(-1);
            }

            FillGiftsButtons();

        });

        _acceptGiftButton.onClick.AddListener(() =>
        {
            _acceptGiftButton.interactable = false;

            if (SetChosenGift())
            {
                _questInfoBlock.SetActive(true);
                _giftsInfoBlock.SetActive(false);
                _menuService.OnGiftAccept();
            }
        });


        //_acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
        
    }

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
        _questInfoBlock.SetActive(true);
    }

    public void SetEnableAcceptGiftButton(bool enabled)
    {
        _acceptGiftButton.interactable = enabled;
    }

    private void FillGiftsButtons()
    {
        _gifts = ServiceLocator.Current.Get<GiftsService>().GenerateGifts();

        if (_questDone)
        {
            _giftsImages[0].overrideSprite = _icons[_gifts.x];
            _giftsImages[0].SetNativeSize();
            _giftsButtons[0].SetTooltip(_giftsTooltips[_gifts.x]);
            _giftsButtons[0].SetEnable(true);
        }
        else
        {
            _giftsImages[0].overrideSprite = _failedQuestIcon;
            _giftsImages[0].SetNativeSize();
            _giftsButtons[0].SetTooltip(_failedQuestTooltip);
            _giftsButtons[0].SetEnable(false);
        }

        _giftsImages[1].overrideSprite = _icons[_gifts.y];
        _giftsButtons[1].SetTooltip(_giftsTooltips[_gifts.y]);

        _giftsImages[2].overrideSprite = _icons[_gifts.z];
        _giftsButtons[2].SetTooltip(_giftsTooltips[_gifts.z]);
    }

    private bool SetChosenGift()
    {
        int _giftNum = _giftsRadioGroup.SelectedButton == 2 ? _gifts.z
                : (_giftsRadioGroup.SelectedButton == 1 ? _gifts.y : _gifts.x);

        Debug.Log(_giftNum);

        if (_giftNum == 0)
            ServiceLocator.Current.Get<GameState>().ChangeHealth(5);
        else
        {
            if (_giftNum == 12)
                ServiceLocator.Current.Get<GameState>().LastHealthGift = true;
            else
            {
                if (_giftNum == 11)
                {
                    _giftsRadioGroup.ResetAllButtons();
                    FillGiftsButtons();
                    return false;
                }
                else
                {
                    if(_giftNum == 9)
                    {
                        _secondQuest.SetActive(true);
                    }
                    else
                    {
                        if(_giftNum == 6)
                        {
                            _questReroll.SetActive(true);
                        }
                    }
                }
            }
        }

        ServiceLocator.Current.Get<TrayControl>().SetGift(_giftNum);
        ServiceLocator.Current.Get<Tetris>().SetGift(_giftNum);

        return true;
    }
}
