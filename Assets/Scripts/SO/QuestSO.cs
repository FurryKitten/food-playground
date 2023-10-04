using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest", order = 1)]
public class QuestSO : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private int _guestId;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;

    public int Id => _id;
    public int GuestId => _guestId;
    public Sprite Icon => _icon;
    public string Description => _description;
}
