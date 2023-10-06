using UnityEngine;

public class BackgroundPart : MonoBehaviour
{
    [SerializeField] private Sprite _kitchenSprite;
    [SerializeField] private Sprite _restaurantSprite;
    [SerializeField] private Sprite _corridorSprite;
    [SerializeField] private Sprite _corridorSprite2;
    [SerializeField] private GameObject _doorObject;

    public void SetKitchen()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _kitchenSprite;

    }

    public void SetRestaurant()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _restaurantSprite;
 
    }

    public void SetCorridor1()
    {
        _doorObject.SetActive(true);
        _doorObject.GetComponent<SpriteRenderer>().sprite = _corridorSprite;
    }

    public void SetCorridor2()
    {
        _doorObject.SetActive(true);
        _doorObject.GetComponent<SpriteRenderer>().sprite = _corridorSprite2;
    }

    public void ResetCorridor()
    {
        _doorObject.SetActive(false);
    }

    public bool IsDoor()
    {
        return _doorObject.activeSelf;
    }
}
