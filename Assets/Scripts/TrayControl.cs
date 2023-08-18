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
    [SerializeField] private Sprite[] _trayBordersSprites;
    [SerializeField] private Sprite[] _trayBodySprites;
    [SerializeField] private Sprite[] _trayCenterSprites;

    private int _trayLVL = 0;
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

        _trayLeftParts.Add(Instantiate(_trayBody, 
            new Vector3(_trayCenter.transform.position.x - 1.5f, 
            _trayCenter.transform.position.y, _trayCenter.transform.position.z),
            transform.rotation, transform));
        

        if(_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.transform.localPosition += difference;
            }

        _trayRightParts.Add(Instantiate(_trayBody, 
            new Vector3(_trayCenter.transform.position.x + 1.5f, 
            _trayCenter.transform.position.y, _trayCenter.transform.position.z), 
            transform.rotation, transform));
    }

    public void SetTrayBorders()
    {
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_trayLVL];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_trayLVL];
    }

    public void UpTrayLVL()
    {
        _trayLVL++;

        _trayCenter.GetComponent<SpriteRenderer>().sprite = _trayCenterSprites[_trayLVL];
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_trayLVL];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_trayLVL];

        if (_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_trayLVL];
            }

        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_trayLVL];
            }

    }
}
