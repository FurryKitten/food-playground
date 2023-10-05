using UnityEngine;
using UnityEngine.UI;

public class PauseUIService : MonoBehaviour
{
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _absolutelyGiveUpButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _warningFrame;
    [SerializeField] private Image _currentGiftIcon;
    [SerializeField] private Image _currentQuestIcon;
    [SerializeField] private TooltipScript _currentGiftTooltip;
    [SerializeField] private TooltipScript _currentQuestTooltip;
    [SerializeField] private GameObject[] _giftTooltips;
    [SerializeField] private GameObject[] _questTooltips;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _continueGameButton.onClick.AddListener(_menuService.OnPressPause);
        _giveUpButton.onClick.AddListener(() => { _warningFrame.SetActive(true); });
        _backButton.onClick.AddListener(() => { _warningFrame.SetActive(false); });
        _absolutelyGiveUpButton.onClick.AddListener(_menuService.OnPressReturnToMenu);
    }

    public void SetActiveGiftAndQuest()
    {
        if (ServiceLocator.Current.Get<GiftsService>().ActiveGift.Id >= 0)
        {
            _currentGiftIcon.overrideSprite = ServiceLocator.Current.Get<GiftsService>().ActiveGift.Icon;
            _currentGiftTooltip.SetToolTip(_giftTooltips[ServiceLocator.Current.Get<GiftsService>().ActiveGift.Id]);
        }
        else
        {
            _currentGiftIcon.overrideSprite = null;
            _currentGiftTooltip.ResetToolTip();
        }

        _currentQuestIcon.overrideSprite = ServiceLocator.Current.Get<QuestsService>().ActiveQuest.Icon;
        _currentQuestTooltip.SetToolTip(_questTooltips[ServiceLocator.Current.Get<QuestsService>().ActiveQuest.Id]);
    }
}
