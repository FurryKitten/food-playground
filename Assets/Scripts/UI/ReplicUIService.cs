using TMPro;
using UnityEngine;

public class ReplicUIService : MonoBehaviour
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
    }

     private void SetNewQuestReplic()
     {
        _newQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].NewQuestPhrase;
     }

    private void SetNewGiftReplic()
    {
        _newGiftPhrase.text = _guestsSO[_questsService.CurrentGuest].NewGiftPhrase;
    }

    private void SetQuestResult()
    {
        if (_questsService.IsQuestDone())
            _resultQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].DoneQuestPhrase;
        else
            _resultQuestPhrase.text = _guestsSO[_questsService.CurrentGuest].FailedQuestPhrase;
    }
}
