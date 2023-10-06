using TMPro;
using UnityEngine;
using static MagicVars;
using static GameConfig;

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
                _tooltip.SetToolTip(_teaPartyBlock);
                _textCupCount.text = $"{_questsService.QuestData.CupCount} / {COND_CUP_COUNT}"; 
                _textTeapotCount.text = $"{_questsService.QuestData.TeapotCount} / {COND_TEAPOT_COUNT}";
                break;
            case QUEST_SEAFOOD_ID:
                _tooltip.SetToolTip(_seafoodBlock);
                _textFishLCount.text = $"{_questsService.QuestData.FishLCount} / {COND_FISH_L_COUNT}"; 
                _textFishLongCount.text = $"{_questsService.QuestData.FishLongCount} / {COND_FISH_LONG_COUNT}"; 
                _textOctopusCount.text = $"{_questsService.QuestData.OctopusCount} / {COND_OCTOPUS_COUNT}"; 
                _textSushiCount.text = $"{_questsService.QuestData.SushiCount} / {COND_SUSHI_COUNT}"; 
                _textShrimpCount.text = $"{_questsService.QuestData.ShrimpCount} / {COND_SHRIMP_COUNT}"; 
                _textCrabCount.text = $"{_questsService.QuestData.CrabCount} / {COND_CRAB_COUNT}";
                break;
            case QUEST_SPOILERS_ID:
                _tooltip.SetToolTip(_spoilersBlock);
                _textSpoilerCount.text = $"{_questsService.QuestData.SpoilerCount} / {COND_SPOILERS_COUNT}";
                break;
            case QUEST_SPOILED_FOOD_ID:
                _tooltip.SetToolTip(_spoiledFoodBlock);
                _questsService.QuestData.CountSpoiledFood();
                _textSpoiledFoodCount.text = $"{_questsService.QuestData.SpoiledFoodCount} / {COND_SPOILED_FOOD_COUNT}";
                break;
            case QUEST_KILL_SPOILERS_ID:
                _tooltip.SetToolTip(_killedSpoilersBlock);
                _textBlackKillCount.text = $"{_questsService.QuestData.SpoilerKillCount} / {COND_SPOILER_KILL_COUNT}";
                break;
            case QUEST_EXPRESS_ID:
                _tooltip.SetToolTip(_expressBlock);
                _textTimer.text = $"12:00"; // TODO: �������� �����
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
