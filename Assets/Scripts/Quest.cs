using UnityEngine;

public class Quest
{
    public readonly int GuestId; /// к какому гостю относится
    public readonly Sprite Icon;
    public readonly string Description;
    public readonly int TooltipId; /// костыль для массива тултипов

    public Quest(QuestSO questSO)
    {
        GuestId = questSO.GuestId;
        Icon = questSO.Icon;
        Description = questSO.Description;
        TooltipId = questSO.TooltipId;
    }
}
