using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestsUIService : MonoBehaviour
{
    [SerializeField] private QuestSO[] _quests;
    [SerializeField] private GameObject[] _toolTips;
    [SerializeField] private Button _acceptQuestButton;
    [SerializeField] private Image _iconQuest1;
    [SerializeField] private Image _iconQuest2;
    [SerializeField] private TextMeshProUGUI _textQuest1;
    [SerializeField] private TextMeshProUGUI _textQuest2;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest1;
    [SerializeField] private ButtonWithTooltipScript _buttonQuest2;
    [SerializeField] private GameObject _quest2;


    private MenuService _menuService;
    private bool _questChoice;
    private int _numberQuest1;
    private int _numberQuest2;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<MenuService>();
        _acceptQuestButton.onClick.AddListener(_menuService.OnQuestAccept);
        _acceptQuestButton.onClick.AddListener(() =>
        {
            _acceptQuestButton.interactable = false;

            _numberQuest1 = Random.Range(0, _quests.Length);

            _textQuest1.text = _quests[_numberQuest1].GetString();
            _iconQuest1.overrideSprite = _quests[_numberQuest1].GetIcon();
            _buttonQuest1.SetTooltip(_toolTips[_numberQuest1]);

            _buttonQuest1.SetTooltip(_toolTips[_numberQuest1]);
            
            _numberQuest2 = Random.Range(0, _quests.Length);

            while(_numberQuest2 == _numberQuest1)
                _numberQuest2 = Random.Range(0, _quests.Length);

            _textQuest2.text = _quests[_numberQuest2].GetString();
            _iconQuest2.overrideSprite = _quests[_numberQuest2].GetIcon();
            _buttonQuest2.SetTooltip(_toolTips[_numberQuest2]);
        });

        _numberQuest1 = Random.Range(0, _quests.Length);

        _textQuest1.text = _quests[_numberQuest1].GetString();
        _iconQuest1.overrideSprite = _quests[_numberQuest1].GetIcon();
        _buttonQuest1.SetTooltip(_toolTips[_numberQuest1]);
        _quest2.SetActive(false);
    }

    public void SetInteractableAcceptQuestButton(bool interactable)
    {
        _acceptQuestButton.interactable = interactable;
    }

    public void SetActiveQuest2(bool active)
    {
        _quest2.SetActive(active);
    }
}