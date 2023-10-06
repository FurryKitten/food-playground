using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

public class QuestsService : MonoBehaviour, IService
{
    [SerializeField] private QuestSO[] _questsSO;
    [SerializeField] private GuestSO[] _guestsSO;
    [SerializeField, Range(1, 5)] private int _maxGuests = 3;

    public QuestData QuestData { get; set; } = new QuestData();

    public GuestSO[] GuestsInfo => _guestsSO;
    public int CurrentGuest { get; private set; } = 0;
    public Quest ActiveQuest { get; private set; }
    public List<Quest> DisplayQuests { get; private set; }

    private Dictionary<int, List<Quest>> _questByGuest;

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
        int newGuestId = Random.Range(0, _maxGuests - 1);
        CurrentGuest = newGuestId < CurrentGuest ? newGuestId : newGuestId + 1;
    }

    /// <summary>
    /// ���������� ��� ����������� � UI
    /// </summary>
    public void GenerateDisplayQuests()
    {
        int questCount = _giftService.ActiveGift.Id == MagicVars.GIFT_QUEST_CHOICE_ID ? 2 : 1;

        DisplayQuests.Clear();
        while (questCount --> 0)
        {
            DisplayQuests.Add(GetRandomQuestByGuest());
        }
    }

    // TODO: ������� magic numbers � ������
    public bool IsQuestDone()
    {
        bool result = false;
        switch (ActiveQuest.Id)
        {
            case MagicVars.QUEST_TEA_PARTY_ID:
                result = QuestData.CupCount >= COND_CUP_COUNT 
                      && QuestData.TeapotCount >= COND_TEAPOT_COUNT;
                break;
            case MagicVars.QUEST_SEAFOOD_ID:
                result = QuestData.FishLCount >= COND_FISH_L_COUNT
                      && QuestData.ShrimpCount >= COND_SHRIMP_COUNT
                      && QuestData.CrabCount >= COND_CRAB_COUNT
                      && QuestData.SushiCount >= COND_SUSHI_COUNT
                      && QuestData.FishLongCount >= COND_FISH_LONG_COUNT
                      && QuestData.OctopusCount >= COND_OCTOPUS_COUNT;
                break;
            case MagicVars.QUEST_SPOILERS_ID:
                result = QuestData.SpoilerCount >= COND_SPOILERS_COUNT;
                break;
            case MagicVars.QUEST_SPOILED_FOOD_ID:
                result = QuestData.SpoiledFoodCount >= COND_SPOILED_FOOD_COUNT;
                break;
            case MagicVars.QUEST_KILL_SPOILERS_ID:
                result = QuestData.SpoilerKillCount >= COND_SPOILER_KILL_COUNT;
                break;
            case MagicVars.QUEST_EXPRESS_ID:
                result = QuestData.Timer <= 0;
                break;
        }
        return result;
    }

    private Quest GetRandomQuestByGuest()
    {
        var questList = _questByGuest[CurrentGuest];
        Quest quest;
        do
            quest = questList[Random.Range(0, questList.Count)];
        while (DisplayQuests.Contains(quest));
        return quest;
    }
}
