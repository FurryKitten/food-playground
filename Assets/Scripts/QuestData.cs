using static MagicVars;

public class QuestData
{
    /// Чайная церемония
    public int CupCount { get; private set; }
    public int TeapotCount { get; private set; }

    /// Рыбная вечеринка
    public int FishLCount { get; private set; }
    public int FishLongCount { get; private set; }
    public int OctopusCount { get; private set; }
    public int SushiCount { get; private set; }
    public int ShrimpCount { get; private set; }
    public int CrabCount { get; private set; }

    /// Сусуватари
    public int BlackCount { get; private set; }

    /// Пожиратель сажи
    public int SpoiledFoodCount { get; private set; }

    /// Подсос лисы
    public int BlackKillCount { get; private set; }

    /// Спидран
    public float Timer { get; private set; }

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
        SpoiledFoodCount = 0;
        BlackKillCount = 0;
        Timer = 0;
    }

    public void ProcessFigure(Figure figure, bool remove = false)
    {
        int delta = remove ? -1 : 1;
        switch(figure.Index)
        {
            case FIGURE_SHRIMP_ID: ShrimpCount += delta; break;
            case FIGURE_FISH_L_ID: FishLCount += delta; break;
            case FIGURE_OCTOPUS_ID: OctopusCount += delta; break;
            case FIGURE_SUSHI_ID: SushiCount += delta; break;
            case FIGURE_CRAB_ID: CrabCount += delta; break;
            case FIGURE_FISH_LONG_ID: FishLongCount += delta; break;
            case FIGURE_CUP_ID: CupCount += delta; break;
            case FIGURE_TEAPOT_ID: TeapotCount += delta; break;
            case FIGURE_CHERNUSHKA_ID: BlackCount += delta; break;
        }
        if (remove && figure.IsSpoiled)
        {
            SpoiledFoodCount--;
        }
    }

    public void ProcessBlackKill(bool remove = false)
    {
        BlackKillCount += remove ? -1 : 1;
    }

    public void ProcessSpoiledFood(bool remove = false)
    {
        SpoiledFoodCount += remove ? -1 : 1;
    }
}
