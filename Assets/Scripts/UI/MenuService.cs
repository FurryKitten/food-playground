using UnityEngine;
using UnityEngine.InputSystem;

public class MenuService : MonoBehaviour, IService
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questsMenu;
    [SerializeField] private GameObject _deathMenu;

    [SerializeField] private GameObject _gameMenuWaiterFrame;
    [SerializeField] private GameObject _gameMenuOrderFrame;

    [SerializeField] private GameObject _gameSpace;

    private GameState _gameState;
    private AudioService _audioService;

    private void Awake()
    {
        ShowMainMenu();
    }

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();
        _audioService = ServiceLocator.Current.Get<AudioService>();
    }

    /** MAIN MENU
     */
    #region MENU BUTTONS
    public void ShowMainMenu()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
    }

    public void OnPressPlay()
    {
        OnOpenQuests();
        _audioService.PlayButtonPress();
        _audioService.PlayMusic();
    }
    #endregion

    /** PAUSE
     */
    #region PAUSE
    public void OnPressPause()
    {
        bool isPaused = _gameState.State == State.PAUSED;

        DisableAllMenu();
        _pauseMenu.SetActive(!isPaused);
        _gameMenuOrderFrame.SetActive(isPaused);
        _gameMenuWaiterFrame.SetActive(isPaused);
        _gameState.SetState(isPaused ? State.TETRIS : State.PAUSED);
        _audioService.PlayButtonPress();
    }

    public void OnPressSettings()
    {

    }

    public void OnPressExit()
    {

    }
    #endregion

    /** QUESTS / UPGRADES
     */
    #region QUESTS / UPGRADES
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
        _gameSpace.SetActive(true);
        _gameMenuWaiterFrame.SetActive(true);
        _gameMenuOrderFrame.SetActive(true);
        
        _audioService.PlayButtonPress();

        _gameState.RestartRun();
        _gameState.SetState(State.TETRIS);
    }

    public void OnGiftAccept()
    {
        OnOpenQuests();
    }
    #endregion

    /** YOU DIED
     */
    #region YOU DIED
    public void ShowDeathScreen(int health = -1)
    {
        if (health <= 0)
        {
            DisableAllMenu();
            _deathMenu.SetActive(true);
            _gameState.SetState(State.PAUSED);
        }
    }

    public void OnDeathReturnToMenu()
    {
        DisableAllMenu();
        ShowMainMenu();
    }

    public void OnDeathReturnToShop()
    {
        DisableAllMenu();
        ShowMainMenu(); //заглущка
    }
    #endregion

    private void DisableAllMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _upgradesMenu.SetActive(false);
        _questsMenu.SetActive(false);
        _deathMenu.SetActive(false);

        _gameMenuOrderFrame.SetActive(false);
        _gameMenuWaiterFrame.SetActive(false);
    }
}
