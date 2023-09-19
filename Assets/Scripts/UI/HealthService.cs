using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthService : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] Slider _healthBar;
    [SerializeField] int _maxHealth = 100;

    private void Awake()
    {
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _maxHealth;
        _healthText.text = $"{_maxHealth}";
    }

    public void SetHeath()
    {
        int hp = ServiceLocator.Current.Get<GameState>().Health;
        _healthText.text = $"{hp}";
        _healthBar.value = hp;
    }
}
