using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guest", menuName = "ScriptableObjects/Guest", order = 1)]
public class GuestSO : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<string> _newQuestPhrases; 
    [SerializeField] private List<string> _questDonePhrases;
    [SerializeField] private List<string> _questFailedPhrases;
    [SerializeField] private List<string> _newGiftPhrases;
    public int Id => _id;
    public Sprite Icon => _icon;

    public string NewQuestPhrase
    {
        get { return _newQuestPhrases[Random.Range(0, _newQuestPhrases.Count)]; }
    }

    public string NewGiftPhrase
    {
        get { return _newGiftPhrases[Random.Range(0, _newGiftPhrases.Count)]; }
    }

    public string DoneQuestPhrase
    {
        get { return _questDonePhrases[Random.Range(0, _questDonePhrases.Count)]; }
    }

    public string FailedQuestPhrase
    {
        get { return _questFailedPhrases[Random.Range(0, _questFailedPhrases.Count)]; }
    }
}
