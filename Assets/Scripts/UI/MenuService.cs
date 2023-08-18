using UnityEngine;

public class MenuService : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private GameObject _upgradesMenu;

    [SerializeField] private GameObject _gameSpace;

    private GameState _gameState;

    private void Awake()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
        _gameSpace.SetActive(false);
    }

    private void Start()
    {
        _gameState = ServiceLocator.Current.Get<GameState>();
    }

    public void OnPressPlay()
    {
        DisableAllMenu();
        _gameMenu.SetActive(true);
        _gameSpace.SetActive(true);
        _gameState.SetState(State.TETRIS); // TODO: Врубать таймер перед тетрисом
    }

    public void OnPressPause()
    {
        DisableAllMenu();
        _pauseMenu.SetActive(true);
    }

    public void OnExitPause()
    {
        DisableAllMenu();
        _gameMenu.SetActive(true);
    }

/*    public void OnShowUpgrades()
    {
        DisableAllMenu();
        _gameSpace.SetActive(false);

        _gameMenu.SetActive(true);
        SetActiveInChildren(_gameMenu.transform, false);
        _upgradesMenu.SetActive(true);
    }

    public void OnContinueUpgrades()
    {
        DisableAllMenu();
        _gameSpace.SetActive(true);

        _gameMenu.SetActive(true);
        SetActiveInChildren(_gameMenu.transform, true);
        _upgradesMenu.SetActive(false);
    }

    private void SetActiveInChildren(Transform transform, bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }*/

    private void DisableAllMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameMenu.SetActive(false);
        _upgradesMenu.SetActive(false);
    }
}
