using System.Collections.Generic;
using UnityEngine;

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
    /// Сменить гостя
    /// </summary>
    public void ChooseNewGuest()
    {
        int newGuestId = Random.Range(0, _maxGuests - 1);
        CurrentGuest = newGuestId < CurrentGuest ? newGuestId : newGuestId + 1;
    }

    /// <summary>
    /// Генерируем для отображения в UI
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

    // TODO: Вынести magic numbers в конфиг
    public bool IsQuestDone()
    {
        bool result = false;
        switch (ActiveQuest.Id)
        {
            case MagicVars.QUEST_TEA_PARTY_ID:
                result = QuestData.CupCount >= 4 && QuestData.TeapotCount >= 1;
                break;
            case MagicVars.QUEST_SEAFOOD_ID:
                result = QuestData.FishLCount >= 1
                      && QuestData.ShrimpCount >= 1
                      && QuestData.CrabCount >= 1
                      && QuestData.SushiCount >= 1
                      && QuestData.FishLongCount >= 1
                      && QuestData.OctopusCount >= 1;
                break;
            case MagicVars.QUEST_SPOILERS_ID:
                result = QuestData.BlackCount >= 5;
                break;
            case MagicVars.QUEST_SPOILED_FOOD_ID:
                result = QuestData.SpoiledFoodCount >= 15 && QuestData.TeapotCount >= 1;
                break;
            case MagicVars.QUEST_KILL_SPOILERS_ID:
                result = QuestData.BlackKillCount >= 10;
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
