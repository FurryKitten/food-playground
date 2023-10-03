using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest", order = 1)]
public class QuestSO : ScriptableObject
{
    [SerializeField] private int _guestId;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;
    [SerializeField] public int TooltipId; //костыль для массива тултипов

    public int GuestId => _guestId;
    public Sprite Icon => _icon;
    public string Description => _description;
}
