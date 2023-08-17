using UnityEngine;

public class GameUIService : MonoBehaviour
{
    [SerializeField] RectTransform _manSprite;
    [SerializeField] RectTransform[] _stageTransforms;

    private void Start()
    {
        SetManSpritePosition();
    }

    public void SetManSpritePosition()
    {
        int stage = ServiceLocator.Current.Get<GameState>().CurrentStage;
        Vector2 manPos = _manSprite.anchoredPosition;
        manPos.y = _stageTransforms[stage].anchoredPosition.y;
        _manSprite.anchoredPosition = manPos;
    }
}
