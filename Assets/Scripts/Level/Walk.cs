using UnityEngine;

public class Walk : MonoBehaviour
{
    [SerializeField] private float _walkTime = 2.0f;

    private GameState _gameState;

    private float _timer;

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();   
    }

    private void Update()
    {
        if (_gameState.State != State.WALK)
        {
            _timer = _walkTime;
            return;
        }

        if (_timer <= 0)
        {
            _timer = _walkTime;
            _gameState.SetState(State.TETRIS);
            return;
        }

        _timer -= Time.deltaTime;
    }
}
