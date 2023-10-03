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
    [SerializeField] private TextMeshProUGUI _textQuest1;
    [SerializeField] private TextMeshProUGUI _textQuest2;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest1;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest2;
    [SerializeField] private GameObject _quest2;
    [SerializeField] private GameObject _rerollButton;


    private UIService _menuService;
    private QuestsService _questService;
    private GiftsService _giftsService;
    private int _numberQuest1;
    private int _numberQuest2;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _questService = ServiceLocator.Current.Get<QuestsService>();
        _giftsService = ServiceLocator.Current.Get<GiftsService>();
        _acceptQuestButton.onClick.AddListener(_menuService.OnQuestAccept);
        /*_acceptQuestButton.onClick.AddListener(() =>
        {
            _acceptQuestButton.interactable = false;

            Quest quest1 = _questService.AddActiveQuest();

            _textQuest1.text = quest1.Description;
            _iconQuest1.overrideSprite = quest1.Icon;
            _buttonQuest1.SetTooltip(_toolTips[quest1.TooltipId]);

            //_buttonQuest1.SetTooltip(_toolTips[quest1.TooltipId]);

            Quest quest2 = _questService.AddActiveQuest();

            _textQuest2.text = quest2.Description;
            _iconQuest2.overrideSprite = quest2.Icon;
            _buttonQuest2.SetTooltip(_toolTips[quest2.TooltipId]);

            _rerollButton.SetActive(false);
            _quest2.SetActive(false);
        });*/

        _rerollButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            _rerollButton.SetActive(false);

            _questService.GenerateDisplayQuests();
            Quest quest = _questService.DisplayQuests[0];

            FillQuest(quest);
        });

        /*_questService.GenerateDisplayQuests();
        Quest quest = _questService.AddActiveQuest();

        _textQuest1.text = quest.Description;
        _iconQuest1.overrideSprite = quest.Icon;
        _buttonQuest1.SetTooltip(_toolTips[quest.GuestId]);
        _quest2.SetActive(false);*/
    }

    public void FillQuests()
    {
        bool isQuestGift = _giftsService.ActiveGift == MagicVars.GIFT_QUEST_CHOICE_ID;

        _questService.ChooseNewGuest();
        _questService.GenerateDisplayQuests();

        if (isQuestGift && _questService.DisplayQuests.Count != 2)
            Debug.LogError("2 quest gift is active, but DisplayQuests.Count != 2");
        Debug.Log($"_questService.DisplayQuests[0]={_questService.DisplayQuests[0].Description}");

        FillQuest(_questService.DisplayQuests[0]);

        if (isQuestGift)
        {
            Debug.Log($"_questService.DisplayQuests[1]={_questService.DisplayQuests[1].Description}");
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
            _buttonQuest1.SetTooltip(_toolTips[quest.GuestId]);
        }
        else if (num == 1)
        {
            _textQuest2.text = quest.Description;
            _iconQuest2.overrideSprite = quest.Icon;
            _buttonQuest2.SetTooltip(_toolTips[quest.TooltipId]);
        }
    }
}