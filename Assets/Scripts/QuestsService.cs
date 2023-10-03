using System.Collections.Generic;
using UnityEngine;

public class QuestsService : MonoBehaviour, IService
{
    [SerializeField] private QuestSO[] _questsSO;
    [SerializeField, Range(1, 5)] private int _maxGuests = 3;

    public int CurrentGuest { get; private set; } = 0;
    public Quest ActiveQuest {  get; private set; }
    public List<Quest> DisplayQuests { get; private set; }

    private Dictionary<int, List<Quest>> _questByGuest;
    //private Dictionary<int, List<Quest>> _activeQuests;
    //private List<Quest> _activeQuests;

    private GiftsService _giftService;

    private void Start()
    {
        _giftService = ServiceLocator.Current.Get<GiftsService>();

        DisplayQuests = new List<Quest>();
        _questByGuest = new Dictionary<int, List<Quest>>();
        foreach (var questSO in _questsSO)
        {
            if (_questByGuest.TryGetValue(questSO.GuestId, out var questList))
            {
                questList.Add(new Quest(questSO));
            }
            else
            {
                _questByGuest.Add(questSO.GuestId, new List<Quest> { new Quest(questSO) });
            }
        }
    }

    public void SetActiveQuest(int id)
    {
        ActiveQuest = DisplayQuests[id];
    }

    /// <summary>
    /// ������� �����
    /// </summary>
    public void ChooseNewGuest()
    {
        int newGuestId = Random.Range(0, _maxGuests - 2);
        CurrentGuest = newGuestId < CurrentGuest ? newGuestId : newGuestId + 1;
    }

    /// <summary>
    /// ���������� ��� ����������� � UI
    /// </summary>
    public void GenerateDisplayQuests()
    {
        int questCount = _giftService.ActiveGift == MagicVars.GIFT_QUEST_CHOICE_ID ? 2 : 1;
        
        DisplayQuests.Clear();
        while (questCount --> 0)
        {
            DisplayQuests.Add(GetRandomQuestByGuest());
        }
    }

    private Quest GetRandomQuestByGuest()
    {
        var questList = _questByGuest[CurrentGuest];
        Quest quest;
        do
            quest = questList[Random.Range(0, questList.Count - 1)];
        while (DisplayQuests.Contains(quest));
        return quest;
    }
}
