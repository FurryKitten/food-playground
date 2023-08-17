using UnityEngine;

public class MenuService : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameMenu;

    private void Awake()
    {
        DisableAllMenu();
        _mainMenu.SetActive(true);
    }

    public void OnPressPlay()
    {
        DisableAllMenu();
        _gameMenu.SetActive(true);
    }

    public void OnPressPause()
    {
        DisableAllMenu();
        _pauseMenu.SetActive(true);
    }

    public void OnExitPause()
    {
        DisableAllMenu();
        _gameMenu.SetActive(false);
    }

    private void DisableAllMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameMenu.SetActive(false);
    }
}
