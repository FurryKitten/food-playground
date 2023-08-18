using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundObject;
    [SerializeField] private float _speed;
    [SerializeField] private int _count;
    [SerializeField] private float _offsetBetween = 0f;

    private List<GameObject> _backgrounds;
    private float _backgroundSize;
    private float lastObjectPos;

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
}
