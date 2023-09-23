using UnityEngine;

public class BackgroundPart : MonoBehaviour
{
    [SerializeField] private Sprite _kitchenSprite;
    [SerializeField] private Sprite _restaurantSprite;
    [SerializeField] private Sprite _corridorSprite;

    public void SetKitchen()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _kitchenSprite;
    }

    public void SetRestaurant()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _restaurantSprite;
    }

    public void SetCorridor()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _corridorSprite;
    }
}
