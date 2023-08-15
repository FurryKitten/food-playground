using UnityEngine;

public enum State
{
    TETRIS,
    WALK
}

public class GameState : MonoBehaviour, IService
{
    public State State { get; private set; }

    [SerializeField, Range(2, 5)] private int _stages = 5;

    private int _currentStage = 0;

    public void AddStage()
    {
        _currentStage++;
    }
}
