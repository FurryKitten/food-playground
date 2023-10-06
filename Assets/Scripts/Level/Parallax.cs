using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private BackgroundPart _backgroundObject;
    [SerializeField] private float _speed;
    [SerializeField] private int _count;
    [SerializeField] private float _offsetBetween = 0f;

    private List<BackgroundPart> _backgrounds;
    private float _backgroundSize;
    private int _leftBgIndex;

    private bool InKitchen = false;
    private bool InRestaurant = true;

    public float DefaultPos { get; private set; }
    public float RightObjPos { get; private set; }

    private void Start()
    {
        _backgrounds = new List<BackgroundPart>();
        _backgroundSize = _backgroundObject.gameObject.GetComponent<Renderer>().bounds.size.x;
        for (int i = 0; i < _count; i++)
        {
            Vector3 bgPos = transform.position;
            float backgroundSize = _backgroundObject.gameObject.GetComponent<Renderer>().bounds.size.x;
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

                if(_backgrounds[i].IsDoor())
                {
                    _backgrounds[i].ResetCorridor();
                    InKitchen = !InKitchen;
                    InRestaurant = !InRestaurant;
                }
                continue;
            }

            if(pos.x - _backgroundSize > cameraBottomLeftPos.x)
            {
                if (ServiceLocator.Current.Get<GameState>().CurrentStage == 0)
                {
                    _backgrounds[i].SetKitchen();

                    if (!InKitchen)
                    {
                        _backgrounds[i].SetCorridor1();
                        InKitchen = true;
                    }
                }
                else if (ServiceLocator.Current.Get<GameState>().CurrentStage == ServiceLocator.Current.Get<GameState>().MaxStage - 1)
                {
                    _backgrounds[i].SetRestaurant();
                    if (!InRestaurant)
                    {
                        _backgrounds[i].SetCorridor2();
                        InRestaurant = true;
                    }
                }
            }
        }
    }

    public void ChangeLeftBgIndex()
    {
        float rightPosX = _backgrounds[0].transform.position.x;
        _leftBgIndex = 0;
        for (int i = 0; i < _backgrounds.Count; i++)
        {
            Vector3 pos = _backgrounds[i].transform.position;
            pos.x = Mathf.Round(_backgrounds[i].transform.position.x);
            _backgrounds[i].transform.position = pos;
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
