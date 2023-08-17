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

}
