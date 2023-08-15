using UnityEngine;

public class ServiceLoaderGame : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private void Awake()
    {
        ServiceLocator.Initialize();
        ServiceLocator.Current.Register<GameState>(_gameState);
    }
}
