using UnityEngine;
using UnityEngine.UI;

public class UpgradesUIService : MonoBehaviour
{
    [SerializeField] private Sprite[] _icons;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _questInfoBlock;
    [SerializeField] private GameObject _giftsInfoBlock;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _acceptGiftButton;
    [SerializeField] private RadioButtonGroupBehaviour _giftsRadioGroup;
    [SerializeField] ButtonWithTooltipScript[] _giftsButtons;
    [SerializeField] private Image[] _giftsImages;
    [SerializeField] private GameObject[] _giftsTooltips;

    private MenuService _menuService;

    private void Awake()
    {
        _upgradesMenu.SetActive(false);
        _questInfoBlock.SetActive(false);
        _giftsInfoBlock.SetActive(false);
    }

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<MenuService>();
        _continueButton.onClick.AddListener(() => { 
            _questInfoBlock.SetActive(false);
            _giftsInfoBlock.SetActive(true);
            _giftsRadioGroup.ResetAllButtons();

            int gift1 = Random.Range(0, _icons.Length);
            int gift2 = Random.Range(0, _icons.Length);
            int gift3 = Random.Range(0, _icons.Length);
            while(gift1 == gift2)
                gift2 = Random.Range(0, _icons.Length);
            
            while(gift2 == gift3 || gift1 == gift3)
                gift3 = Random.Range(0, _icons.Length);

            _giftsImages[0].overrideSprite = _icons[gift1];
            _giftsButtons[0].SetTooltip(_giftsTooltips[gift1]);

            _giftsImages[1].overrideSprite = _icons[gift2]; 
            _giftsButtons[1].SetTooltip(_giftsTooltips[gift2]);

            _giftsImages[2].overrideSprite = _icons[gift3];
            _giftsButtons[2].SetTooltip(_giftsTooltips[gift3]);
        });
        _acceptGiftButton.onClick.AddListener(() =>
        {
            _questInfoBlock.SetActive(true);
            _giftsInfoBlock.SetActive(false);
            _acceptGiftButton.interactable = false;
        });
        _acceptGiftButton.onClick.AddListener(_menuService.OnGiftAccept);
        int gift1 = Random.Range(0, _icons.Length);
        int gift2 = Random.Range(0, _icons.Length);
        int gift3 = Random.Range(0, _icons.Length);
        while (gift1 == gift2)
            gift2 = Random.Range(0, _icons.Length);

        while (gift2 == gift3 || gift1 == gift3)
            gift3 = Random.Range(0, _icons.Length);

        _giftsImages[0].overrideSprite = _icons[gift1];
        _giftsButtons[0].SetTooltip(_giftsTooltips[gift1]);

        _giftsImages[1].overrideSprite = _icons[gift2];
        _giftsButtons[1].SetTooltip(_giftsTooltips[gift2]);

        _giftsImages[2].overrideSprite = _icons[gift3];
        _giftsButtons[2].SetTooltip(_giftsTooltips[gift3]);
    }

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
        _questInfoBlock.SetActive(true);
    }

    public void SetEnableAcceptGiftButton(bool enabled)
    {
        _acceptGiftButton.interactable = enabled;
    }
}
