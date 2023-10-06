using TMPro;
using UnityEngine;

public class ReplicUIService : MonoBehaviour, IService
{
    [SerializeField] private GuestSO[] _guestsSO;
    [SerializeField] private TextMeshProUGUI _newQuestPhrase;
    [SerializeField] private TextMeshProUGUI _resultQuestPhrase;
    [SerializeField] private TextMeshProUGUI _newGiftPhrase;
    //For Chief
    [SerializeField] private TextMeshProUGUI _pausePhrase;
    [SerializeField] private TextMeshProUGUI _gameOverPhrase;

    private QuestsService _questsService;

    private void Start()
    {
        _questsService = ServiceLocator.Current.Get<QuestsService>();
       // ServiceLocator.Current.Get<GameState>()._onFinish
    }

     public void SetNewQuestReplic()
     {
        _newQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].NewQuestPhrase;
     }

    public void SetNewGiftReplic()
    {
        _newGiftPhrase.text = _guestsSO[_questsService.CurrentGuest].NewGiftPhrase;
    }

    public void SetQuestResult()
    {
        if (_questsService.IsQuestDone())
            _resultQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].DoneQuestPhrase;
        else
            _resultQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].FailedQuestPhrase;
    }


}
