using TMPro;
using UnityEngine;
using static MagicVars;

public class TooltipCountersUI : MonoBehaviour
{
    [SerializeField] private TooltipScript _tooltip;
    [SerializeField] private TextMeshProUGUI _textCupCount;
    [SerializeField] private TextMeshProUGUI _textTeapotCount;
    [SerializeField] private TextMeshProUGUI _textFishLCount;
    [SerializeField] private TextMeshProUGUI _textFishLongCount;
    [SerializeField] private TextMeshProUGUI _textOctopusCount;
    [SerializeField] private TextMeshProUGUI _textSushiCount;
    [SerializeField] private TextMeshProUGUI _textShrimpCount;
    [SerializeField] private TextMeshProUGUI _textCrabCount;
    [SerializeField] private TextMeshProUGUI _textSpoilerCount;
    [SerializeField] private TextMeshProUGUI _textSpoiledFoodCount;
    [SerializeField] private TextMeshProUGUI _textBlackKillCount;
    [SerializeField] private TextMeshProUGUI _textTimer;

    private GameObject _seafoodBlock;
    private GameObject _teaPartyBlock;
    private GameObject _spoilersBlock;
    private GameObject _spoiledFoodBlock;
    private GameObject _killedSpoilersBlock;
    private GameObject _expressBlock;

    public void FillTooltips()
    {
        SetParents();
        QuestsService _questsService = ServiceLocator.Current.Get<QuestsService>();
        DisableAllBlocks();
        switch (_questsService.ActiveQuest.Id)
        {
            case QUEST_TEA_PARTY_ID:
                _teaPartyBlock.SetActive(true);
                _tooltip.SetToolTip(_teaPartyBlock);
                _textCupCount.text = $"{_questsService.QuestData.CupCount}"; 
                _textTeapotCount.text = $"{_questsService.QuestData.TeapotCount}";
                break;
            case QUEST_SEAFOOD_ID:
                _seafoodBlock.SetActive(true);
                _tooltip.SetToolTip(_seafoodBlock);
                _textFishLCount.text = $"{_questsService.QuestData.FishLCount}";
                _textFishLongCount.text = $"{_questsService.QuestData.FishLongCount}";
                _textOctopusCount.text = $"{_questsService.QuestData.OctopusCount}";
                _textSushiCount.text = $"{_questsService.QuestData.SushiCount}";
                _textShrimpCount.text = $"{_questsService.QuestData.ShrimpCount}";
                _textCrabCount.text = $"{_questsService.QuestData.CrabCount}";
                break;
            case QUEST_SPOILERS_ID:
                _spoilersBlock.SetActive(true);
                _tooltip.SetToolTip(_spoilersBlock);
                _textSpoilerCount.text = $"{_questsService.QuestData.BlackCount}";
                break;
            case QUEST_SPOILED_FOOD_ID:
                _spoiledFoodBlock.SetActive(true);
                _tooltip.SetToolTip(_spoiledFoodBlock);
                _textSpoiledFoodCount.text = $"{_questsService.QuestData.SpoiledFoodCount}";
                break;
            case QUEST_KILL_SPOILERS_ID:
                _killedSpoilersBlock.SetActive(true);
                _tooltip.SetToolTip(_killedSpoilersBlock);
                _textBlackKillCount.text = $"{_questsService.QuestData.BlackKillCount}";
                break;
            case QUEST_EXPRESS_ID:
                _expressBlock.SetActive(true);
                _tooltip.SetToolTip(_expressBlock);
                _textTimer.text = $"12:00"; // TODO: добавить время
                break;
        }
    }

    private void DisableAllBlocks()
    {
        _seafoodBlock.SetActive(false);
        _teaPartyBlock.SetActive(false);
        _spoilersBlock.SetActive(false);
        _spoiledFoodBlock.SetActive(false);
        _killedSpoilersBlock.SetActive(false);
        _expressBlock.SetActive(false);
    }

    private void SetParents()
    {
        _seafoodBlock = _textFishLCount.transform.parent.gameObject;
        _teaPartyBlock = _textCupCount.transform.parent.gameObject;
        _spoilersBlock = _textSpoilerCount.transform.parent.gameObject;
        _spoiledFoodBlock = _textSpoiledFoodCount.transform.parent.gameObject;
        _killedSpoilersBlock = _textBlackKillCount.transform.parent.gameObject;
        _expressBlock = _textTimer.transform.parent.gameObject;
    }
}
