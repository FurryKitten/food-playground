using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenMenu : MonoBehaviour
{
    [SerializeField] private Button _toMenuButton;
    [SerializeField] private TextMeshProUGUI _ResultsText;

    private UIService _menuService;

    private void Start()
    {
        _menuService =  ServiceLocator.Current.Get<UIService>();
        _toMenuButton.onClick.AddListener(_menuService.OnDeathReturnToMenu);
    }

    public void FillResultText()
    {
        _ResultsText.text = $"Получено чаевых: {ServiceLocator.Current.Get<GameState>().MoneyInRun}¥" +
            $"\nПожеланий выполнено: {ServiceLocator.Current.Get<GameState>().QuestDone}" +
            $"\nГостей обслужено: {ServiceLocator.Current.Get<GameState>().ClientsInRun - 1}";
    }
}
