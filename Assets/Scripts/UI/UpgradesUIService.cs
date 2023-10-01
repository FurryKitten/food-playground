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

    private UIService _menuService;
    private bool _questDone = false; // TO DO: use Locator

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

            Vector3Int gifts = ServiceLocator.Current.Get<GiftsService>().GenerateGifts();


            if (_questDone)
            {
                _giftsImages[0].overrideSprite = _icons[gifts.x];
                _giftsButtons[0].SetTooltip(_giftsTooltips[gifts.x]);
                _giftsButtons[0].SetEnable(true);
            }
            else
            {
                _giftsImages[0].overrideSprite = _failedQuestIcon;
                _giftsButtons[0].SetTooltip(_failedQuestTooltip);
                _giftsButtons[0].SetEnable(false);
            }

            _giftsImages[1].overrideSprite = _icons[gifts.y]; 
            _giftsButtons[1].SetTooltip(_giftsTooltips[gifts.y]);

            _giftsImages[2].overrideSprite = _icons[gifts.z];
            _giftsButtons[2].SetTooltip(_giftsTooltips[gifts.z]);
        });
        _acceptGiftButton.onClick.AddListener(() =>
        {
            _questInfoBlock.SetActive(true);
            _giftsInfoBlock.SetActive(false);
            _acceptGiftButton.interactable = false;
        });
        _acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
        Vector3Int gifts = ServiceLocator.Current.Get<GiftsService>().GenerateGifts();

        if (_questDone)
        {
            _giftsImages[0].overrideSprite = _icons[gifts.x];
            _giftsButtons[0].SetTooltip(_giftsTooltips[gifts.x]);
            _giftsButtons[0].SetEnable(true);
        }
        else
        {
            _giftsImages[0].overrideSprite = _failedQuestIcon;
            _giftsButtons[0].SetTooltip(_failedQuestTooltip);
            _giftsButtons[0].SetEnable(false);
        }

        _giftsImages[1].overrideSprite = _icons[gifts.y];
        _giftsButtons[1].SetTooltip(_giftsTooltips[gifts.y]);

        _giftsImages[2].overrideSprite = _icons[gifts.z];
        _giftsButtons[2].SetTooltip(_giftsTooltips[gifts.z]);
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
