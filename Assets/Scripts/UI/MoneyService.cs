using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class MoneyService : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] TextMeshProUGUI _healthText;

    private void Awake()
    {
        _moneyText.text = "¥0";
        _healthText.text = "100";
    }

    public void SetTrayMoneyText()
    {
        int money = ServiceLocator.Current.Get<GameState>().Money;
        int trayMoney = ServiceLocator.Current.Get<GameState>().MoneyOnTray;

        _moneyText.text = $"¥{trayMoney + money}";
    }

    public void SetHeath()
    {
        int hp = ServiceLocator.Current.Get<GameState>().Health;
        _healthText.text = $"{hp}";
    }
}
