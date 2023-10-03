using UnityEngine;

public class ShopFramePart : MonoBehaviour
{
    [SerializeField] GameObject _grayFrame;
    [SerializeField] GameObject _goldFrame;

    public void SetGray()
    {
        _grayFrame.SetActive(true);
    }

    public void SetGold()
    {
        _goldFrame.SetActive(true);
    }

    public void ResetAll()
    {
        _grayFrame.SetActive(false);
        _grayFrame.SetActive(false);
    }
}
