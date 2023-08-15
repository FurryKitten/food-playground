using UnityEngine;

public class ServiceLoaderGame : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private ServiceLocator _services = ServiceLocator.Current;

    private void Awake()
    {
        ServiceLocator.Initialize();
        Debug.Log(_services);
        _services.Register<GameState>(_gameState);
    }
}
