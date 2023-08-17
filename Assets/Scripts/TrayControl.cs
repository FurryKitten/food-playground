using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> _trayLeftParts;
    [SerializeField] private List<GameObject> _trayRightParts;
    [SerializeField] private GameObject _trayLeftEnd;
    [SerializeField] private GameObject _trayRightEnd;
    [SerializeField] private GameObject _trayCenter;
    [SerializeField] private GameObject _trayBody;
    [SerializeField] private Sprite _trayBorders;
    
    private int _trayWidth = 10;

    public void SetTrayWidht(int width) //TO DO: use Unity Events
    {
        Vector3 difference = new Vector3 ((width - _trayWidth)/2, 0, 0);

        _trayLeftEnd.transform.localPosition += -difference;
        _trayRightEnd.transform.localPosition += difference;

        if(_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.transform.localPosition += -difference;
            }

        _trayLeftParts.Add(Instantiate(_trayBody, new Vector3(-1.5f, 0, 0), transform.rotation, transform));
        

        if(_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.transform.localPosition += difference;
            }

        _trayRightParts.Add(Instantiate(_trayBody, new Vector3(1.5f, 0, 0), transform.rotation, transform));
    }

    public void SetTrayBorders()
    {
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayBorders;
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayBorders;
    }
}
