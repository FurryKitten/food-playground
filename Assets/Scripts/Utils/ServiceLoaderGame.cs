using UnityEngine;

public class ServiceLoaderGame : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    [SerializeField] private Tetris _tetris;
    [SerializeField] private TrayControl _trayControl;
    [SerializeField] private HandPlacer _handPlacer;
    [SerializeField] private HandControls _handControls;

    private void Awake()
    {
        ServiceLocator.Initialize();

        ServiceLocator.Current.Register<GameState>(_gameState);
        ServiceLocator.Current.Register<Tetris>(_tetris);
        ServiceLocator.Current.Register<TrayControl>(_trayControl);
        ServiceLocator.Current.Register<HandPlacer>(_handPlacer);
        ServiceLocator.Current.Register<HandControls>(_handControls);
    }
}
