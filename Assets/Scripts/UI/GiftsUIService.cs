using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftsUIService : MonoBehaviour
{
    [SerializeField] private Sprite[] _icons;
    [SerializeField] private GameObject _upgradesMenu;
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

    /// Quest info block
    [Space]
    [SerializeField] private GameObject _questResultsBlock;
    [SerializeField] private TextMeshProUGUI _questDescription;
    [SerializeField] private Image _questIcon;
    [SerializeField] private Image _guestIcon;
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private Sprite _successIndicator;
    [SerializeField] private Sprite _failIndicator;

    private UIService _uiService;
    private QuestsService _questsService;
    private Vector3Int _gifts = new Vector3Int(-1, -1, -1);

    private void Awake()
    {
        _upgradesMenu.SetActive(false);
        _questResultsBlock.SetActive(false);
        _giftsInfoBlock.SetActive(false);
    }

    private void Start()
    {
        _questsService = ServiceLocator.Current.Get<QuestsService>();

        ServiceLocator.Current.Get<GiftsService>().ResetGiftPool();
        _uiService = ServiceLocator.Current.Get<UIService>();
        _continueButton.onClick.AddListener(() => { 
            _questResultsBlock.SetActive(false);
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
                _questResultsBlock.SetActive(true);
                _giftsInfoBlock.SetActive(false);
                _uiService.OnGiftAccept();
            }
        });


        //_acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
        
    }

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
        _questResultsBlock.SetActive(true);
        FillQuestResults();
    }

    public void SetEnableAcceptGiftButton(bool enabled)
    {
        _acceptGiftButton.interactable = enabled;
    }

    public void FillQuestResults()
    {
        bool isQuestDone = _questsService.IsQuestDone();
        Quest quest = _questsService.ActiveQuest;
        _questDescription.text = quest.Description;
        _questIcon.overrideSprite = quest.Icon;
        _indicatorImage.overrideSprite = isQuestDone ? _successIndicator : _failIndicator;
        _guestIcon.overrideSprite = _questsService.GuestsInfo[quest.GuestId].Icon;
        _guestIcon.SetNativeSize();
    }

    private void FillGiftsButtons()
    {
        bool isQuestDone = _questsService.IsQuestDone();
        _gifts = ServiceLocator.Current.Get<GiftsService>().GenerateGifts();

        if (isQuestDone)
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

    // TODO: Перенести эту логику в GiftService, Здесь только вычислять айди гифта
    private bool SetChosenGift()
    {
        int giftNum = _giftsRadioGroup.SelectedButton == 2 ? _gifts.z
                : (_giftsRadioGroup.SelectedButton == 1 ? _gifts.y : _gifts.x);

        Debug.Log(giftNum);

        if (giftNum == 0)
            ServiceLocator.Current.Get<GameState>().ChangeHealth(5);
        else
        {
            if (giftNum == 12)
                ServiceLocator.Current.Get<GameState>().LastHealthGift = true;
            else
            {
                if (giftNum == 11)
                {
                    _giftsRadioGroup.ResetAllButtons();
                    FillGiftsButtons();
                    return false;
                }
                else
                {
                    if(giftNum == 9)
                    {
                        _secondQuest.SetActive(true);
                    }
                    else
                    {
                        if(giftNum == 6)
                        {
                            _questReroll.SetActive(true);
                        }
                    }
                }
            }
        }

        ServiceLocator.Current.Get<TrayControl>().SetGift(giftNum);
        ServiceLocator.Current.Get<Tetris>().SetGift(giftNum);

        ServiceLocator.Current.Get<GiftsService>().ActiveGift.Id = giftNum;
        ServiceLocator.Current.Get<GiftsService>().ActiveGift.Icon = _icons[giftNum];

        return true;
    }
}
