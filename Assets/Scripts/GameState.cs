using UnityEngine;
using UnityEngine.Events;

public enum State
{
    PAUSED,
    TETRIS,
    WALK
}

public class GameState : MonoBehaviour, IService
{

    public State State { get; private set; }
    public int CurrentStage { get; private set; } = 0;

    public int Money { get; private set; } = 0;
    public int MoneyOnTray { get; private set; } = 0;

    [SerializeField, Range(2, 5)] private int _stages = 5;
    [SerializeField] private UnityEvent _onStageChange;
    [SerializeField] private UnityEvent _onMoneyChange;

    [ContextMenu("Add stage")]
    public void AddStage()
    {
        CurrentStage++;
        _onStageChange.Invoke();
    }

    public void SetState(State state)
    {
        State = state;
    }

    /// Money - общие деньги, MoneyOnTray - стоимость еды на подносе
    /// Пока несем, меняем MoneyOnTray, донесли - зафиксировали в Money
    public void AddMoney(int money)
    {
        Money += money;
        Money = Money < 0 ? 0 : Money;
        _onMoneyChange.Invoke();
    }
}
