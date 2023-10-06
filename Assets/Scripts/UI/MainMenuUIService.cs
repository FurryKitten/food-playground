using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIService : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _playNewGameButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _settingsWindow;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _playButton.onClick.AddListener(_menuService.OnPressPlay);
        _shopButton.onClick.AddListener(_menuService.ShowShop);
        _playNewGameButton.onClick.AddListener(_menuService.OnPressNewPlay);

        _playButton.interactable = false;
        _playNewGameButton.onClick.AddListener(() =>
        {
            _playButton.interactable = true;
        });
        _exitButton.onClick.AddListener(() => { Application.Quit(); });
        _settingsButton.onClick.AddListener(() =>
        {
            _settingsWindow.SetActive(true);
        });
    }
}
