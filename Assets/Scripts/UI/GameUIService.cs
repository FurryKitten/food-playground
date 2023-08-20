using UnityEngine;

public class GameUIService : MonoBehaviour
{
    [SerializeField] private GameObject _stagesUI;
    [SerializeField] private GameObject _waiterUI;
    [SerializeField] private GameObject _upgradesUI;
    [SerializeField] private GameObject _darkPanel;

    [SerializeField] private RectTransform _manSprite;
    [SerializeField] private RectTransform[] _stageTransforms;

    private void Start()
    {
        _darkPanel.SetActive(false);
        SetManSpritePosition();
    }

    public void SetManSpritePosition()
    {
        int stage = ServiceLocator.Current.Get<GameState>().CurrentStage;
        Vector2 manPos = _manSprite.anchoredPosition;
        manPos.y = _stageTransforms[stage].anchoredPosition.y;
        _manSprite.anchoredPosition = manPos;
    }

    public void OnUpgradesContinueButton()
    {
        _stagesUI.SetActive(true);
        _waiterUI.SetActive(true);
        _upgradesUI.SetActive(false);
        _darkPanel.SetActive(false);
    }

    public void OnFinishStage()
    {
        _stagesUI.SetActive(false);
        _waiterUI.SetActive(true);
        _upgradesUI.SetActive(true);
        _darkPanel.SetActive(true);
    }
}
