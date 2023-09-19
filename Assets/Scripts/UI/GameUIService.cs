using UnityEngine;
using UnityEngine.UI;

public class GameUIService : MonoBehaviour
{
    [SerializeField] private GameObject _waiterUI;
    [SerializeField] private GameObject _upgradesUI;
    [SerializeField] private GameObject _darkPanel;
    [SerializeField] private GameObject _orderFrame;


    [SerializeField] private Slider _stageSlider;

    private void Start()
    {
        _darkPanel.SetActive(false);
        _stageSlider.value = 0;
        SetManSpritePosition();
    }

    public void SetManSpritePosition()
    {
        int stage = ServiceLocator.Current.Get<GameState>().CurrentStage;
        _stageSlider.value = stage % 4;
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
}
