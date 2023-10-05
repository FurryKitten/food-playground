using UnityEngine;
using UnityEngine.UI;

public class PauseUIService : MonoBehaviour
{
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _absolutelyGiveUpButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _warningFrame;

    private UIService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<UIService>();
        _continueGameButton.onClick.AddListener(_menuService.OnPressPause);
        _giveUpButton.onClick.AddListener(() => { _warningFrame.SetActive(true); });
        _backButton.onClick.AddListener(() => { _warningFrame.SetActive(false); });
        _absolutelyGiveUpButton.onClick.AddListener(_menuService.OnPressReturnToMenu);
    }
}
