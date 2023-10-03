using System.Collections.Generic;
using UnityEngine;

public class QuestsService : MonoBehaviour, IService
{
    [SerializeField] private QuestSO[] _questsSO;
    [SerializeField, Range(1,5)] private int _maxGuests = 3;

    public int CurrentGuest { get; private set; } = 0;

    private Dictionary<int, List<Quest>> _activeQuests;

    private void Start()
    {
        _activeQuests = new Dictionary<int, List<Quest>>();
        foreach (var questSO in _questsSO)
        {
            if (_activeQuests.TryGetValue(questSO.GuestId, out var questList))
            {
                questList.Add(new Quest(questSO));
            }
            else
            {
                questList = new List<Quest>{ new Quest(questSO) };
            }
        }
    }

    public Quest GetRandomQuestByGuest()
    {
        var questList = _activeQuests[CurrentGuest];
        return questList[Random.Range(0, questList.Count - 1)];
    }

    public void ChooseNewGuest()
    {
        int newGuestId = Random.Range(0, _maxGuests - 2);
        CurrentGuest = newGuestId < CurrentGuest ? newGuestId : newGuestId + 1;
    }
}
