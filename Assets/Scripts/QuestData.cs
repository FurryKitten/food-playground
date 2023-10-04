public class QuestData
{
    /// ������ ���������
    public int CupCount { get; set; }
    public int TeapotCount { get; set; }

    /// ������ ���������
    public int FishLCount { get; set; }
    public int FishLongCount { get; set; }
    public int OctopusCount { get; set; }
    public int SushiCount { get; set; }
    public int ShrimpCount { get; set; }
    public int CrabCount { get; set; }

    /// ����������
    public int BlackCount { get; set; }

    /// ������ ����
    public int BlackKillCount { get; set; }

    /// �������
    public float Timer { get; set; }

    public QuestData()
    {
        ResetData();
    }

    public void ResetData()
    {
        CupCount = 0;
        TeapotCount = 0;
        FishLCount = 0;
        FishLongCount = 0;
        OctopusCount = 0;
        SushiCount = 0;
        ShrimpCount = 0;
        CrabCount = 0;
        BlackCount = 0;
        BlackKillCount = 0;
        Timer = 0;
    }
}
