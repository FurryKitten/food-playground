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
    public int MaxStage => _stages;

    public int Money { get; private set; } = 0;
    public int MoneyOnTray { get; private set; } = 0;

    [SerializeField, Range(2, 5)] private int _stages = 5;
    [SerializeField] private UnityEvent _onStageChange;
    [SerializeField] private UnityEvent _onMoneyChange;
    [SerializeField] private UnityEvent _onTrayMoneyChange;
    [SerializeField] public UnityEvent _onFinish;

    [SerializeField] private Animator _animatorTrayMoneyCounter;
    [SerializeField] private Animator _animatorAllMoneyCounter;

    [ContextMenu("Add stage")]
    public void AddStage()
    {
        CurrentStage++;
        _onStageChange?.Invoke();

        /// последний этап, начисляем деньги, показываем апгрейды
        if (CurrentStage == _stages - 1) 
        {
            //State = State.PAUSED;
            // переходим в Walk.cs, там идем, потом включаем апгрейды
            AddMoney(MoneyOnTray);
            MoneyOnTray = 0;

            //_onFinish?.Invoke();
        }
    }

    public void SetState(State state)
    {
        State = state;
    }

    /// Money - общие деньги, MoneyOnTray - стоимость еды на подносе
    /// Пока несем, меняем MoneyOnTray, донесли - зафиксировали в Money
    public void AddMoney(int money)
    {
        if (money < 0)
            _animatorAllMoneyCounter.SetTrigger("LoseMoney");
        else
            _animatorAllMoneyCounter.SetTrigger("GetMoney");
        Money += money;
        Money = Money < 0 ? 0 : Money;
        Debug.Log($"Money: {Money}");
        MoneyOnTray = 0;
        _onMoneyChange?.Invoke();
        _onTrayMoneyChange?.Invoke();
    }

    public void AddTrayMoney(int money)
    {
        if (money < 0)
            _animatorTrayMoneyCounter.SetTrigger("LoseMoney");
        else
            _animatorTrayMoneyCounter.SetTrigger("GetMoney");
        MoneyOnTray += money;
        MoneyOnTray = MoneyOnTray < 0 ? 0 : MoneyOnTray;
        Debug.Log($"MoneyOnTray: {MoneyOnTray}");
        _onTrayMoneyChange?.Invoke();
    }

    public void RestartRun()
    {
        CurrentStage = 0;
        State = State.TETRIS;
    }
}
