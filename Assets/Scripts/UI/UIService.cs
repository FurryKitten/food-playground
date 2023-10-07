using UnityEngine;

public class UIService : MonoBehaviour, IService
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questsMenu;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _shopMenu;

    [SerializeField] private GameObject _gameMenuWaiterFrame;
    [SerializeField] private GameObject _gameMenuOrderFrame;

    [SerializeField] private GameObject _gameSpace;

    private PauseUIService _pauseUI; // TO DO: 
    private DeathScreenMenu _deathScreenMenu;

    private GameState _gameState;
    private AudioService _audioService;
    private QuestsUIService _questsUIService;

    private void Awake()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
    }

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();
        _audioService = ServiceLocator.Current.Get<AudioService>();

        _questsUIService = GetComponent<QuestsUIService>();
        _pauseUI = GetComponent<PauseUIService>();
        _deathScreenMenu = GetComponent<DeathScreenMenu>();

        _gameState._onFinish.AddListener(() => { 
            _audioService.PlayTetrisJingle();
            _audioService.StopMusic(); });
    }

    /** MAIN MENU
     */
    #region MENU BUTTONS
    public void ShowMainMenu()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
        _audioService.PlayMenuMusic();
    }

    public void OnPressPlay()
    {
        OnOpenQuests();
        _audioService.PlayButtonPress();
    }

    public void OnPressNewPlay()
    {
        _gameState.RestarForNewGame();
        OnOpenQuests();
        _audioService.PlayButtonPress();
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
        _gameState.SetState(isPaused ? _gameState.PreviousState : State.PAUSED);
        // _gameState.SetState(isPaused ? State.TETRIS : State.PAUSED); // UNCOMMENT FOR FAST TESTING - PAUSE-UNPAUSE IN WALK TO GO TETRIS MODE
        _audioService.PlayButtonPress();
        _pauseUI.SetActiveGiftAndQuest();
        _pauseUI.HideWarningAndTooltips();
    }

    public void OnPressReturnToMenu()
    {
        _gameState.ChangeOrderNumber(-1);
        DisableAllMenu();
        ShowMainMenu();
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

        _gameState.ChangeOrderNumber(1);
        _questsUIService.FillQuests();
        ServiceLocator.Current.Get<ReplicUIService>().SetNewQuestReplic();
        ServiceLocator.Current.Get<ReplicUIService>().SetNewGiftReplic();
        _pauseUI.SetActiveGiftAndQuest();
        _audioService.PlayMenuMusic();
    }

    public void OnQuestAccept()
    {
        DisableAllMenu();
        _gameSpace.SetActive(true);
        _gameMenuWaiterFrame.SetActive(true);
        _gameMenuOrderFrame.SetActive(true);
        
        _audioService.PlayButtonPress();

        _gameState.RestartRun();
        _gameState.SetState(State.WALK);
        _audioService.PlayMusic();
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

            _deathScreenMenu.FillResultText();

            _gameState.FinishRun();
            _audioService.StopMusic();
            _audioService.PlayFail();
        }
    }

    public void OnDeathReturnToMenu()
    {
        DisableAllMenu();
        ShowMainMenu();
    }

    public void OnDeathReturnToShop()
    {
        ShowShop();
        _audioService.PlayMenuMusic();
    }
    #endregion

    /** Shop
     */
    #region SHOP
    public void ShowShop()
    {
        DisableAllMenu();
        _shopMenu.SetActive(true);
    }

    public void OnShopBackButton()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
    }
    #endregion

    private void DisableAllMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _upgradesMenu.SetActive(false);
        _questsMenu.SetActive(false);
        _deathMenu.SetActive(false);
        _shopMenu.SetActive(false);

        _gameMenuOrderFrame.SetActive(false);
        _gameMenuWaiterFrame.SetActive(false);
    }
}
