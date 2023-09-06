using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundObject;
    [SerializeField] private float _speed;
    [SerializeField] private int _count;
    [SerializeField] private float _offsetBetween = 0f;

    private List<GameObject> _backgrounds;
    private float _backgroundSize;
    private int _leftBgIndex;

    public float DefaultPos { get; private set; }
    public float RightObjPos { get; private set; }

    private void Start()
    {
        _backgrounds = new List<GameObject>();
        _backgroundSize = _backgroundObject.GetComponent<Renderer>().bounds.size.x;
        for (int i = 0; i < _count; i++)
        {
            Vector3 bgPos = transform.position;
            float backgroundSize = _backgroundObject.GetComponent<Renderer>().bounds.size.x;
            bgPos.x += (backgroundSize + _offsetBetween) * i;

            var newBg = Instantiate(_backgroundObject, bgPos, Quaternion.identity, transform);
            _backgrounds.Add(newBg);
        }

        _leftBgIndex = _count - 1;
        DefaultPos = _backgrounds[0].transform.position.x;
        RightObjPos = _backgrounds[_leftBgIndex].transform.position.x;
    }

    private void Update()
    {
        if (ServiceLocator.Current.Get<GameState>().State != State.WALK)
            return;

        Scroll();
        Loop();
    }

    private void Scroll()
    {
        for (int i = 0; i < _backgrounds.Count; i++)
        {
            Vector3 pos = _backgrounds[i].transform.position;
            pos.x -= _speed * Time.deltaTime;
            _backgrounds[i].transform.position = pos;
        }
        RightObjPos = _backgrounds[_leftBgIndex].transform.position.x;
    }

    private void Loop()
    {
        Vector3 cameraBottomLeftPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));

        for (int i = 0; i < _backgrounds.Count; i++)
        {
            Vector3 pos = _backgrounds[i].transform.position;
            if (pos.x + _backgroundSize / 2f < cameraBottomLeftPos.x)
            {
                Vector3 newPos = pos;
                newPos = _backgrounds[(i + _count - 1) % _count].transform.position;
                newPos.x += _backgroundSize + _offsetBetween;
                _backgrounds[i].transform.position = newPos;
            }
        }
    }

    public void ChangeLeftBgIndex()
    {
        float rightPosX = _backgrounds[0].transform.position.x;
        _leftBgIndex = 0;
        for (int i = 0; i < _backgrounds.Count; i++)
        {
            if (_backgrounds[i].transform.position.x > rightPosX)
            {
                rightPosX = _backgrounds[i].transform.position.x;
                _leftBgIndex = i;
            }
        }
        Debug.Log($"left index {_leftBgIndex} {rightPosX} {_backgrounds[_leftBgIndex].transform.position.x}");
        RightObjPos = rightPosX;
    }
}
