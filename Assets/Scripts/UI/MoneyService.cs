using TMPro;
using UnityEngine;

public class MoneyService : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyText;

    private void Awake()
    {
        _moneyText.text = "0";
    }

    public void SetTrayMoneyText()
    {
        int money = ServiceLocator.Current.Get<GameState>().Money;
        int trayMoney = ServiceLocator.Current.Get<GameState>().MoneyOnTray;

        _moneyText.text = $"{trayMoney + money}";
    }

}
