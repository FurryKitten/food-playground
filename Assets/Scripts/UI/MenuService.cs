using UnityEngine;

public class MenuService : MonoBehaviour, IService
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameMenuWaiterFrame;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questsMenu;
    [SerializeField] private GameObject _gameMenuOrderFrame;

    [SerializeField] private GameObject _gameSpace;

    private GameState _gameState;

    private void Awake()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
        _questsMenu.SetActive(false);
    }

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();
    }

    public void OnPressPlay()
    {
      /* DisableAllMenu();
        _gameMenu.SetActive(true);
        _gameSpace.SetActive(true);
        _gameState.SetState(State.TETRIS); // TODO: Врубать таймер перед тетрисом*/
        OnOpenQuests();
        ServiceLocator.Current.Get<AudioService>().PlayButtonPress();
        ServiceLocator.Current.Get<AudioService>().PlayMusic();
    }

    public void OnPressPause()
    {
        DisableAllMenu();
        _pauseMenu.SetActive(true);
        _gameState.SetState(State.PAUSED);
    }

    public void OnExitPause()
    {
        DisableAllMenu();
        _gameMenuOrderFrame.SetActive(true);
        _gameState.SetState(State.TETRIS);
        ServiceLocator.Current.Get<AudioService>().PlayButtonPress();
    }

    public void OnOpenQuests()
    {
        DisableAllMenu();
        _gameMenuWaiterFrame.SetActive(true);
        _questsMenu.SetActive(true);
        _gameState.SetState(State.PAUSED);
    }

    public void OnQuestAccept()
    {
        DisableAllMenu();
        _gameState.SetState(State.TETRIS);
        _gameSpace.SetActive(true);
        _gameMenuWaiterFrame.SetActive(true);
        _gameMenuOrderFrame.SetActive(true);
        ServiceLocator.Current.Get<AudioService>().PlayButtonPress();
    }

    public void OnGiftAccept()
    {
        OnOpenQuests();
    }




    private void DisableAllMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameMenuWaiterFrame.SetActive(false);
        _upgradesMenu.SetActive(false);
        _questsMenu.SetActive(false);
        _gameMenuOrderFrame.SetActive(false);
    }
}
