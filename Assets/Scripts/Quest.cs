using UnityEngine;

public class Quest
{
    private int _guestId; /// к какому гостю относится
    private Sprite _icon;
    private string _description;

    public Quest(QuestSO questSO)
    {
        _guestId = questSO.GuestId;
        _icon = questSO.Icon;
        _description = questSO.Description;
    }
}
