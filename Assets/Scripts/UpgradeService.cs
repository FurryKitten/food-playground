using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UpgradeService : MonoBehaviour
{
    [SerializeField] private UpgradeInfoSO _upgradeInfo;

    [SerializeField] private GameObject[] _tierUpgrades;
    [SerializeField] private GameObject[] _tierSpecialUpgrade;
    [SerializeField] private GameObject _hiddenUpgrade;

    [SerializeField] private Button[] _upgradeButtons;

    private TextMeshProUGUI[] _buttonTexts;

    private int _maxTier = 2;
    private int _currentTier = 0; // 1-3 tier
    private bool[] isUpgradeBought = new bool[3] { false, false, false };
    private GameState _gameState;

    private void Start()
    {
        _buttonTexts = new TextMeshProUGUI[_upgradeButtons.Length];
        for (int i = 0; i < _buttonTexts.Length; i++)
        {
            _buttonTexts[i] = _upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            Assert.IsNotNull(_buttonTexts[i]);
            _buttonTexts[i].text = $"¥{_upgradeInfo.Tier1Costs[i]}";
            _upgradeButtons[i].interactable = true;
        }
        _upgradeButtons[_maxTier].interactable = false; // special tier

        for (int i = 0; i < _tierUpgrades.Length; i++) 
        {
            _tierUpgrades[i].SetActive(false);
            _tierSpecialUpgrade[i].SetActive(false) ;
        }
        _tierUpgrades[0].SetActive(true);
        _tierSpecialUpgrade[0].SetActive(true);

        _hiddenUpgrade.SetActive(true);

        _gameState = ServiceLocator.Current.Get<GameState>();
    }

    public void BuyUpgrade(int col)
    {
        if (_gameState.Money < _upgradeInfo.getTierCosts()[_currentTier][col])
        {
            //no money
            return;
        }

        _gameState.AddMoney(-_upgradeInfo.getTierCosts()[_currentTier][col]);

        applyUpgrade(col);
        isUpgradeBought[col] = true;
        _upgradeButtons[col].interactable = false;

        if (isSpecialReady())
        {
            //open third
            _hiddenUpgrade.SetActive(false);
            _tierSpecialUpgrade[_currentTier].SetActive(true);
            _upgradeButtons[2].interactable = true;
        }

        if (isAllBought()) // to next tier
        {
            if (_currentTier == _maxTier)
            {
                for (int i = 0; i < _upgradeButtons.Length; i++)
                {
                    _upgradeButtons[i].interactable = false;
                }
                return;
            }
            _tierUpgrades[_currentTier].SetActive(false);
            _currentTier++;
            isUpgradeBought = new bool[3] { false, false, false };
            _tierUpgrades[_currentTier].SetActive(true);
            _hiddenUpgrade.SetActive(true);
            updatePrices();
        }
        UpdateButtons();
    }

    /// Обновить кнопки по ивенту получения денег (нет денег - кнопка заблокирована)
    public void UpdateButtons()
    {
        int[][] prices = _upgradeInfo.getTierCosts();
        for (int col = 0; col < _upgradeButtons.Length; col++)
        {
            bool isInteractable = !isUpgradeBought[col] && _gameState.Money >= prices[_currentTier][col];
            _upgradeButtons[col].interactable = isInteractable;
        }
        
        if (!isSpecialReady())
        {
            _upgradeButtons[_maxTier].interactable = false;
        }
    }

    private bool isAllBought()
    {
        return isUpgradeBought[0] && isUpgradeBought[1] && isUpgradeBought[2];
    }

    private bool isSpecialReady()
    {
        return isUpgradeBought[0] && isUpgradeBought[1] && !isUpgradeBought[2];
    }

    private void updatePrices()
    {
        int[][] prices = _upgradeInfo.getTierCosts();
        for (int col = 0; col < _buttonTexts.Length; col++)
        {
            _buttonTexts[col].text = $"¥{prices[_currentTier][col]}";
        }
    }

    private void applyUpgrade(int col)
    {
        switch(_currentTier)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
        }

        switch(col)
        {
            case 0: // podnos length
                int width = 12 + 2 * _currentTier;
                ServiceLocator.Current.Get<Tetris>().SetGridWidth(width);
                ServiceLocator.Current.Get<TrayControl>().SetTrayWidth(width);
                ServiceLocator.Current.Get<HandPlacer>().SetGridWidth(width);
                break;
            case 1: // бортики, тентакля, цена
                switch(_currentTier)
                {
                    case 0:
                        ServiceLocator.Current.Get<Tetris>().SetTrayBorders();
                        ServiceLocator.Current.Get<TrayControl>().SetTrayBorders();
                        break;
                    case 1:
                        ServiceLocator.Current.Get<HandControls>().SetTentacle();
                        break;
                    case 2:
                        ServiceLocator.Current.Get<Tetris>().SetDoubleCost();
                        break;
                }
                break;
            case 2:
                ServiceLocator.Current.Get<TrayControl>().UpTrayLVL();
                break;
        }
    }
}
