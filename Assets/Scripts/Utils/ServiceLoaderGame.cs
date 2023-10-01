using UnityEngine;

public class ServiceLoaderGame : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    [SerializeField] private Tetris _tetris;
    [SerializeField] private TrayControl _trayControl;
    [SerializeField] private HandPlacer _handPlacer;
    [SerializeField] private HandControls _handControls;
    [SerializeField] private AudioService _audioService;
    [SerializeField] private MenuService _menuService;
    [SerializeField] private GiftsService _giftsService;

    private void Awake()
    {
        ServiceLocator.Initialize();

        ServiceLocator.Current.Register<GameState>(_gameState);
        ServiceLocator.Current.Register<Tetris>(_tetris);
        ServiceLocator.Current.Register<TrayControl>(_trayControl);
        ServiceLocator.Current.Register<HandPlacer>(_handPlacer);
        ServiceLocator.Current.Register<HandControls>(_handControls);
        ServiceLocator.Current.Register<AudioService>(_audioService);
        ServiceLocator.Current.Register<MenuService>(_menuService);
        ServiceLocator.Current.Register<GiftsService>(_giftsService);
    }
}
