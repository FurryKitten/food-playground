using UnityEngine;
using UnityEngine.UI;

public class QuestsUIService : MonoBehaviour
{
    /// ������� �� � ��������
    //[SerializeField] private QuestSO[] quests;

    [SerializeField] private Button _acceptQuestButton;

    private MenuService _menuService;

    private void Start()
    {
        _menuService = ServiceLocator.Current.Get<MenuService>();
        _acceptQuestButton.onClick.AddListener(_menuService.OnQuestAccept);
    }
}