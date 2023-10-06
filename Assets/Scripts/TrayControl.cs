using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayControl : MonoBehaviour, IService
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
    [SerializeField] private Sprite[] _trayEndSprites;
    [SerializeField] private Sprite[] _trayStikySprites;
    [SerializeField] private GameObject _trayLeftEndEffects;
    [SerializeField] private GameObject _trayRightEndEffects;
    [SerializeField] private GameObject _trayCenterEffects;
    [SerializeField] private GameObject _trayBodyEffects;
    [SerializeField] private List<GameObject> _trayBodyLeftEffects;
    [SerializeField] private List<GameObject> _trayBodyRightEffects;
    [SerializeField] private Sprite _emptySprite;

    private int _trayLVL = 0;
    private int _trayWidth = 10;
    public int _traySkin { get; set; } = 0;

    public void SetTrayWidth(int width) //TO DO: use Unity Events
    {
        Vector3 difference = new Vector3 ((width - _trayWidth)/2, 0, 0);

        _trayLeftEnd.transform.localPosition += -difference;
        _trayRightEnd.transform.localPosition += difference;

        if(_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.transform.localPosition += -difference;
            }

        if(_trayBodyLeftEffects.Count > 0)
            foreach(var part in _trayBodyLeftEffects)
            {
                part.transform.localPosition += -difference;
            }

        var trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayLeftParts.Add(trayPart);

        var trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyLeftEffects.Add(trayPartEffect);
        /*_trayLeftParts.Add(Instantiate(_trayBody, 
            new Vector3(_trayCenter.transform.position.x - 1.5f, 
            _trayCenter.transform.localPosition.y, _trayCenter.transform.position.z),
            transform.rotation, transform));*/


        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.transform.localPosition += difference;
            }

        if (_trayBodyRightEffects.Count > 0)
            foreach (var part in _trayBodyRightEffects)
            {
                part.transform.localPosition += -difference;
            }

        trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayRightParts.Add(trayPart);

        trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyRightEffects.Add(trayPartEffect);
        /*_trayRightParts.Add(Instantiate(_trayBody, 
            new Vector3(_trayCenter.transform.position.x + 1.5f, 
            _trayCenter.transform.localPosition.y, _trayCenter.transform.position.z), 
            transform.rotation, transform));*/

        _trayWidth = width;
    }

    public void IncreaseTrayWidth() 
    {
        Vector3 difference = Vector3.right;

        _trayLeftEnd.transform.localPosition += -difference;
        _trayRightEnd.transform.localPosition += difference;

        if (_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.transform.localPosition += -difference;
            }

        if (_trayBodyLeftEffects.Count > 0)
            foreach (var part in _trayBodyLeftEffects)
            {
                part.transform.localPosition += -difference;
            }

        var trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayLeftParts.Add(trayPart);

        var trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyLeftEffects.Add(trayPartEffect);


        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.transform.localPosition += difference;
            }

        if (_trayBodyRightEffects.Count > 0)
            foreach (var part in _trayBodyRightEffects)
            {
                part.transform.localPosition += -difference;
            }

        trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayRightParts.Add(trayPart);

        trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyRightEffects.Add(trayPartEffect);

        _trayWidth += 2;
        ServiceLocator.Current.Get<HandPlacer>().SetGridWidth(_trayWidth);
    }

    public void ResetTrayWidth()
    {
        Vector3 difference = new Vector3((_trayWidth - 10) / 2, 0, 0);

        _trayLeftEnd.transform.localPosition -= -difference;
        _trayRightEnd.transform.localPosition -= difference;

        if (_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                Destroy(part.gameObject);
            }

        if (_trayBodyLeftEffects.Count > 0)
            foreach (var part in _trayBodyLeftEffects)
            {
                Destroy(part.gameObject);
            }
        _trayLeftParts.Clear();
        _trayBodyLeftEffects.Clear();

        var trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayLeftParts.Add(trayPart);

        var trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x - 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyLeftEffects.Add(trayPartEffect);


        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                Destroy(part.gameObject);
            }

        if (_trayBodyRightEffects.Count > 0)
            foreach (var part in _trayBodyRightEffects)
            {
                Destroy(part.gameObject);
            }

        _trayRightParts.Clear();
        _trayBodyRightEffects.Clear();

        trayPart = Instantiate(_trayBody, transform);
        trayPart.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPart.transform.rotation = transform.rotation;
        _trayRightParts.Add(trayPart);

        trayPartEffect = Instantiate(_trayBodyEffects, transform);
        trayPartEffect.transform.localPosition = new Vector3(_trayCenter.transform.localPosition.x + 1.5f,
            _trayCenter.transform.localPosition.y, _trayCenter.transform.localPosition.z);
        trayPartEffect.transform.rotation = transform.rotation;
        _trayBodyRightEffects.Add(trayPartEffect);

        _trayWidth = 10;
        ServiceLocator.Current.Get<HandPlacer>().SetGridWidth(_trayWidth);
        SetSkin(0);
    }

    public void SetTrayBorders()
    {
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_traySkin];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_traySkin];
    }

    public void ResetTrayBorders()
    {
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayEndSprites[_traySkin];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayEndSprites[_traySkin];
    }

    public void UpTrayLVL()
    {
        _trayLVL++;

        _trayCenter.GetComponent<SpriteRenderer>().sprite = _trayCenterSprites[_traySkin];
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_traySkin];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayBordersSprites[_traySkin];

        if (_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_traySkin];
            }

        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_traySkin];
            }

    }

    private void ResetStickyTray()
    {
        _trayCenterEffects.GetComponent<SpriteRenderer>().sprite = _emptySprite;
        _trayLeftEndEffects.GetComponent<SpriteRenderer>().sprite = _emptySprite;
        _trayRightEndEffects.GetComponent<SpriteRenderer>().sprite = _emptySprite;

        foreach(var part in _trayBodyLeftEffects)
            part.GetComponent<SpriteRenderer>().sprite = _emptySprite;
        foreach (var part in _trayBodyRightEffects)
            part.GetComponent<SpriteRenderer>().sprite = _emptySprite;
    }

    private void SetStickyTray()
    {
        _trayCenterEffects.GetComponent<SpriteRenderer>().sprite = _trayStikySprites[1];
        _trayLeftEndEffects.GetComponent<SpriteRenderer>().sprite = _trayStikySprites[2];
        _trayRightEndEffects.GetComponent<SpriteRenderer>().sprite = _trayStikySprites[3];

        foreach (var part in _trayBodyLeftEffects)
            part.GetComponent<SpriteRenderer>().sprite = _trayStikySprites[0];
        foreach (var part in _trayBodyRightEffects)
            part.GetComponent<SpriteRenderer>().sprite = _trayStikySprites[0];
    }

    public void SetGift(int giftNum)
    {
        ResetTrayBorders();
        ResetStickyTray();

        if (giftNum == 10)
            SetStickyTray();
        else
            if (giftNum == 4)
                SetTrayBorders();
    }

    public void SetSkin(int skin)
    {
        if (_traySkin == skin)
            return;

        _traySkin = skin;
        _trayCenter.GetComponent<SpriteRenderer>().sprite = _trayCenterSprites[_traySkin];
        _trayLeftEnd.GetComponent<SpriteRenderer>().sprite = _trayEndSprites[_traySkin];
        _trayRightEnd.GetComponent<SpriteRenderer>().sprite = _trayEndSprites[_traySkin];

        if (_trayLeftParts.Count > 0)
            foreach (var part in _trayLeftParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_traySkin];
            }

        if (_trayRightParts.Count > 0)
            foreach (var part in _trayRightParts)
            {
                part.GetComponent<SpriteRenderer>().sprite = _trayBodySprites[_traySkin];
            }
    }
}
