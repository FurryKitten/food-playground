using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIService : MonoBehaviour
{
    [SerializeField] private GameObject _waiterUI;
    [SerializeField] private GameObject _upgradesUI;
    [SerializeField] private GameObject _darkPanel;
    [SerializeField] private GameObject _orderFrame;


    [SerializeField] private Slider _stageSlider;

    private int _currentStage = 0;
    private bool _animationCheck = false;

    private void Start()
    {
        _darkPanel.SetActive(false);
        _stageSlider.value = 0;
        SetManSpritePosition();
    }

    public void SetManSpritePosition()
    {
        _currentStage = ServiceLocator.Current.Get<GameState>().CurrentStage % 4;
        if (!_animationCheck)
            StartCoroutine(SliderMove());
    }

    public void OnUpgradesContinueButton()
    {
        _orderFrame.SetActive(true);
        _waiterUI.SetActive(true);
        _upgradesUI.SetActive(false);
        _darkPanel.SetActive(false);

        ServiceLocator.Current.Get<AudioService>().PlayButtonPress();
    }

    public void OnFinishStage()
    {
        _orderFrame.SetActive(false);
        _waiterUI.SetActive(true);
        _upgradesUI.SetActive(true);
        _darkPanel.SetActive(true);
    }

    private IEnumerator SliderMove()
    {
        float t = 0;
        const float animationSpeed = 0.2f;
        while (t < 1)
        {
            _stageSlider.value = Mathf.Lerp(_stageSlider.value, _currentStage, t * t * t);
            t += Time.deltaTime * animationSpeed;
            yield return null;
        }
        _animationCheck = false;
    }
}
