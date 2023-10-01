using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIService : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _playButton.onClick.AddListener(_menuService.OnPressPlay);
        _shopButton.onClick.AddListener(_menuService.ShowShop);
    }
}
