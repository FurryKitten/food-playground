using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthService : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] _healthText;
    [SerializeField] Slider _healthBar;
    [SerializeField] Slider _whiteHealthBar;
    [SerializeField] int _maxHealth = 100;

    private float _whiteValue;
    private bool _animationCheck = false;

    private void Awake()
    {
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _maxHealth;
        _whiteHealthBar.maxValue = _maxHealth;
        _whiteHealthBar.value = _maxHealth;
        _whiteValue = _maxHealth;
        foreach (var health in _healthText)
            health.text = $"{_maxHealth}";
    }

    public void SetHeath()
    {
        int hp = ServiceLocator.Current.Get<GameState>().Health;
        foreach (var health in _healthText)
            health.text = $"{hp}";
        _healthBar.value = hp;

        if (!_animationCheck)
        {
            _animationCheck = true;
            StartCoroutine(SliderAnimation());
        }
    }

    private IEnumerator SliderAnimation()
    {
        float t = 0;
        const float animationSpeed = 0.4f;
        while (t < 1)
        {
            _whiteValue = Mathf.Lerp(_whiteValue, _healthBar.value, t * t * t);
            t += Time.deltaTime * animationSpeed;
            _whiteHealthBar.value = _whiteValue;
            yield return null;
        }
        _animationCheck = false;
    }
}
