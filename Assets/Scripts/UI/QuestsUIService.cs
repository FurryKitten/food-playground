using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestsUIService : MonoBehaviour
{
    //[SerializeField] private QuestSO[] _quests;
    [SerializeField] private GameObject[] _toolTips;
    [SerializeField] private Button _acceptQuestButton;
    [SerializeField] private Image _iconQuest1;
    [SerializeField] private Image _iconQuest2;
    [SerializeField] private Image _guestImage;
    [SerializeField] private TextMeshProUGUI _textQuest1;
    [SerializeField] private TextMeshProUGUI _textQuest2;
    [SerializeField] private RadioButtonGroupBehaviour _radioButtons;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest1;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest2;
    [SerializeField] private GameObject _quest2;
    [SerializeField] private GameObject _rerollButton;

    private UIService _menuService;
    private QuestsService _questService;
    private GiftsService _giftsService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _questService = ServiceLocator.Current.Get<QuestsService>();
        _giftsService = ServiceLocator.Current.Get<GiftsService>();
        _acceptQuestButton.onClick.AddListener(_menuService.OnQuestAccept);

        _rerollButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            _rerollButton.SetActive(false);

            _questService.GenerateDisplayQuests();
            Quest quest = _questService.DisplayQuests[0];
            ServiceLocator.Current.Get<AudioService>().PlayButtonPress();
            FillQuest(quest);
        });

        _buttonQuest1._onClick.AddListener(num =>
        {
            if (num >= 0 && num < _questService.DisplayQuests.Count)
                _questService.SetActiveQuest(num);
        });

        _buttonQuest2._onClick.AddListener(num =>
        {
            if (num >= 0 && num < _questService.DisplayQuests.Count)
                _questService.SetActiveQuest(num);
        });

        SetActiveQuest2(false);
    }

    public void FillQuests()
    {
        SetActiveQuest2(false);
        _radioButtons.ResetAllButtons();
        _buttonQuest1.SetSelectState(true);
        SetInteractableAcceptQuestButton(true);

        bool isQuestGift = _giftsService.ActiveGift == MagicVars.GIFT_QUEST_CHOICE_ID;

        _questService.ChooseNewGuest();
        _questService.QuestData.ResetData();
        _guestImage.overrideSprite = _questService.GuestsInfo[_questService.CurrentGuest].Icon;
        _guestImage.SetNativeSize();

        _questService.GenerateDisplayQuests();

        if (isQuestGift && _questService.DisplayQuests.Count != 2)
            Debug.LogError("2 quest gift is active, but DisplayQuests.Count != 2");

        FillQuest(_questService.DisplayQuests[0]);

        _questService.SetActiveQuest(0);

        if (isQuestGift)
        {
            FillQuest(_questService.DisplayQuests[1], 1);
            SetActiveQuest2(true);
        }
    }

    public void SetInteractableAcceptQuestButton(bool interactable)
    {
        _acceptQuestButton.interactable = interactable;
    }

    public void SetActiveQuest2(bool active)
    {
        _quest2.SetActive(active);
    }

    
    private void FillQuest(Quest quest, int num = 0)
    {
        if (num == 0)
        {
            _textQuest1.text = quest.Description;
            _iconQuest1.overrideSprite = quest.Icon;
            _buttonQuest1.SetTooltip(_toolTips[quest.Id]);
        }
        else if (num == 1)
        {
            _textQuest2.text = quest.Description;
            _iconQuest2.overrideSprite = quest.Icon;
            _buttonQuest2.SetTooltip(_toolTips[quest.Id]);
        }
    }
}