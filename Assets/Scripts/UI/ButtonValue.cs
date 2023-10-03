using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonValue : MonoBehaviour
{
     private int _value = 0;
     private ShopUIService _shopUI;

    public void SetDescription()
    {
        _shopUI.SetDescription(_value);
    }

    public void SetButtonParametrs(ShopUIService shop, int val)
    {
        _shopUI = shop;
        _value = val;
    }
}
