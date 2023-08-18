using UnityEngine;

public class UpgradesUIService : MonoBehaviour
{
    [SerializeField] private GameObject _upgradesMenu;

    private void Awake()
    {
        _upgradesMenu.SetActive(false);
    }

    public void ShowUpgrades()
    {
        _upgradesMenu.SetActive(true);
    }
}
