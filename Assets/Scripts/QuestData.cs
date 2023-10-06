using System.Collections.Generic;
using static MagicVars;

public class QuestData
{
    /// ������ ���������
    public int CupCount { get; private set; }
    public int TeapotCount { get; private set; }

    /// ������ ���������
    public int FishLCount { get; private set; }
    public int FishLongCount { get; private set; }
    public int OctopusCount { get; private set; }
    public int SushiCount { get; private set; }
    public int ShrimpCount { get; private set; }
    public int CrabCount { get; private set; }

    /// ����������
    public int SpoilerCount { get; private set; }

    /// ���������� ����
    public int SpoiledFoodCount { get; private set; }

    /// ������ ����
    public int SpoilerKillCount { get; private set; }

    /// �������
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
        SpoilerCount = 0;
        SpoiledFoodCount = 0;
        SpoilerKillCount = 0;
        Timer = 0;
    }

    public void ProcessFigure(Figure figure, bool remove = false)
    {
        int delta = remove ? -1 : 1;
        switch (figure.Index)
        {
            case FIGURE_SHRIMP_ID: ShrimpCount += delta; break;
            case FIGURE_FISH_L_ID: FishLCount += delta; break;
            case FIGURE_OCTOPUS_ID: OctopusCount += delta; break;
            case FIGURE_SUSHI_ID: SushiCount += delta; break;
            case FIGURE_CRAB_ID: CrabCount += delta; break;
            case FIGURE_FISH_LONG_ID: FishLongCount += delta; break;
            case FIGURE_CUP_ID: CupCount += delta; break;
            case FIGURE_TEAPOT_ID: TeapotCount += delta; break;
            case FIGURE_CHERNUSHKA_ID: SpoilerCount += delta; break;
        }
        if (remove && figure.IsSpoiled)
        {
            SpoiledFoodCount--;
        }
    }

    public void ProcessBlackKill(bool remove = false)
    {
        SpoilerKillCount += remove ? -1 : 1;
    }

// todo: убрать
    public void ProcessSpoiledFood(bool spoiled)
    {
        SpoiledFoodCount += spoiled ? 0 : 0;
    }

    public void CountSpoiledFood()
    {
        int spoiledCount = 0;
        List<Figure> figureList = ServiceLocator.Current.Get<Tetris>().FigureList;
        figureList.ForEach(figure =>
        {
            spoiledCount += figure.IsSpoiled ? 1 : 0;
        });
        SpoiledFoodCount = spoiledCount;
    }
}
