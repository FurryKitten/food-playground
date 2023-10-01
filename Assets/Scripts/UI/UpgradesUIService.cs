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

    private MenuService _menuService;
    private bool _questDone = false; // TO DO: use Locator
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
        _menuService = ServiceLocator.Current.Get<MenuService>();
        _continueButton.onClick.AddListener(() => { 
            _questInfoBlock.SetActive(false);
            _giftsInfoBlock.SetActive(true);
            _giftsRadioGroup.ResetAllButtons();

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
        });

        _acceptGiftButton.onClick.AddListener(() =>
        {
            _questInfoBlock.SetActive(true);
            _giftsInfoBlock.SetActive(false);
            _acceptGiftButton.interactable = false;
            ServiceLocator.Current.Get<Tetris>().SetGift(_giftsRadioGroup.SelectedButton == 2 ? _gifts.z 
                : (_giftsRadioGroup.SelectedButton == 1 ? _gifts.y : _gifts.x));
        });


        _acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
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

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
        _questInfoBlock.SetActive(true);
    }

    public void SetEnableAcceptGiftButton(bool enabled)
    {
        _acceptGiftButton.interactable = enabled;
    }
}
