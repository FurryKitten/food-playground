using UnityEngine;

public class Quest
{
    public readonly int Id;
    public readonly int GuestId; /// к какому гостю относится
    public readonly Sprite Icon;
    public readonly string Description;

    public Quest(QuestSO questSO)
    {
        Id = questSO.Id;
        GuestId = questSO.GuestId;
        Icon = questSO.Icon;
        Description = questSO.Description;
    }
}
