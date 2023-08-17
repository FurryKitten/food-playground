using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource : MonoBehaviour
{
    private int _trayWidth = 10;
    private bool _defaultHand = true;
    private int _playerCash = 0;
    private bool _trayBorders = false;
    private bool _doubleCost = false;

    public void SetHand(bool status) 
    {
        _defaultHand = status;
    }
    public bool GetHandStatus()
    {
        return _defaultHand;
    }
    public void TrayUp() 
    {
        _trayWidth += 2;
    }
    public int GetTrayWidth()
    {
        return _trayWidth;
    }
    public void SetTrayWidth(int width)
    {
        _trayWidth = width;
    }
    public void SetTrayBorders(bool status)
    {
        _trayBorders = status;
    }

    public bool GetTrayBorders()
    {
        return _trayBorders;
    }

    public void SetDoubleCost(bool status)
    {
        _doubleCost = status;
    }

    public bool GetDoubleCost()
    {
        return _doubleCost;
    }

    public void AddPlayerCash(int cash)
    {
        _playerCash += cash;
    }

    public int GetPlayerCash()
    {
        return _playerCash;
    }


}
