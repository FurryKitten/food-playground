using UnityEngine;

public class Quest
{
    public readonly int GuestId; /// � ������ ����� ���������
    public readonly Sprite Icon;
    public readonly string Description;
    public readonly int TooltipId; /// ������� ��� ������� ��������

    public Quest(QuestSO questSO)
    {
        GuestId = questSO.GuestId;
        Icon = questSO.Icon;
        Description = questSO.Description;
        TooltipId = questSO.TooltipId;
    }
}
