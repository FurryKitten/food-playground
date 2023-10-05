using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Gift
{
    public int Id;
    public Sprite Icon;
}
public class GiftsService : MonoBehaviour, IService
{
    private List<int> _giftPool = new List<int>();
    public Gift ActiveGift;

    private void Start()
    {
        ActiveGift.Id = -1;
    }

    public Vector3Int GenerateGifts()
    {
        Vector3Int gifts = new Vector3Int();

        gifts.x = Random.Range(0, _giftPool.Count);
        gifts.y = Random.Range(0, _giftPool.Count);
        gifts.z = Random.Range(0, _giftPool.Count);

        while(gifts.x == gifts.y)
            gifts.y = Random.Range(0, _giftPool.Count);

        while(gifts.x == gifts.z || gifts.y == gifts.z)
            gifts.z = Random.Range(0, _giftPool.Count);

        gifts.x = _giftPool[gifts.x];
        gifts.y = _giftPool[gifts.y];
        gifts.z = _giftPool[gifts.z];

        return gifts;
    }

    public void AddGiftInPool(int giftNumber)
    {
        _giftPool.Add(giftNumber);
    }

    public void ResetGiftPool()
    {
        _giftPool.Clear();

        for (int i = 0; i < 4; ++i)
            _giftPool.Add(i);
    }
}
