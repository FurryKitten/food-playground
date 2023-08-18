using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax2 : MonoBehaviour
{
    [SerializeField] private GameObject[] _bgPrefabs;
    [SerializeField] private float[] _speeds;

    private List<GameObject[]> _backgroundObjs = new List<GameObject[]>(); //List of 2 gameobjs

    private void Start()
    {
        InitBackgrounds();
    }

    private void Update()
    {
        if (ServiceLocator.Current.Get<GameState>().State != State.WALK)
            return;

        StaticChildrenScrolling();
        ApplyBackgroundLooping();
    }

    private void StaticChildrenScrolling()
    {
        for (int i = 0; i < _backgroundObjs.Count; i++)
        {
            for (int j = 0; j < _backgroundObjs[i].Length; j++)
            {
                Vector3 childPos = _backgroundObjs[i][j].transform.position;
                childPos.x -= _speeds[i] * Time.deltaTime;
                _backgroundObjs[i][j].transform.position = childPos;
            }
        }
    }

    private void ApplyBackgroundLooping()
    {
        Vector3 cameraBottomLeftPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        foreach (var objs in _backgroundObjs)
        {
            float backgroundSize = objs[0].GetComponent<Renderer>().bounds.size.x;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].transform.position.x + backgroundSize / 2f < cameraBottomLeftPos.x)
                {
                    Vector3 newPos = objs[i].transform.position;
                    newPos = objs[(i + 1) % 2].transform.position;
                    newPos.x += backgroundSize;
                    objs[i].transform.position = newPos;
                }
            }
        }
    }

    private void InitBackgrounds()
    {
        // Background Images (Parallax)
        foreach (var prefabContainer in _bgPrefabs)
        {
            for (int i = 0; i < prefabContainer.transform.childCount; i++)
            {
                var prefab = prefabContainer.transform.GetChild(i).gameObject;
                GameObject[] backgrounds = new GameObject[2];

                Vector3 bgPos = prefab.transform.position;
                float backgroundSize = prefab.GetComponent<Renderer>().bounds.size.x;
                backgrounds[0] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

                bgPos += new Vector3(backgroundSize, 0, 0);
                backgrounds[1] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

                _backgroundObjs.Add(backgrounds);
            }
        }
    }


    /*private void ApplyBackgroundLooping()
    {
        Vector3 cameraBottomLeftPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        foreach (var objs in _backgroundObjs)
        {
            float backgroundSize = objs[0].GetComponent<Renderer>().bounds.size.x;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].transform.position.x + backgroundSize / 2f < cameraBottomLeftPos.x)
                {
                    Vector3 newPos = objs[i].transform.position;
                    newPos = objs[(i + 1) % 2].transform.position;
                    newPos.x += backgroundSize;
                    objs[i].transform.position = newPos;
                }
            }
        }
    }

    private void InitBackgrounds()
    {
        // Background Images (Parallax)
        for (int i = 0; i < _bgPrefabs.Length; i++)
        {
            var prefab = _bgPrefabs[i];
            GameObject[] backgrounds = new GameObject[2];

            Vector3 bgPos = transform.position;
            float backgroundSize = prefab.GetComponent<Renderer>().bounds.size.x;
            backgrounds[0] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

            bgPos += new Vector3(backgroundSize, 0, 0);
            backgrounds[1] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

            _backgroundObjs.Add(backgrounds);
        }
    }*/

    /*private void InitBackgrounds()
    {
        // Background Images (Parallax)
        for (int i = 0; i < _spritePrefabs.Length; i++)
        {
            var prefab = _spritePrefabs[i];
            GameObject[] backgrounds = new GameObject[2];

            Vector3 bgPos = prefab.transform.position;
            float backgroundSize = prefab.GetComponent<Renderer>().bounds.size.x;
            backgrounds[0] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

            bgPos += new Vector3(backgroundSize, 0, 0);
            backgrounds[1] = Instantiate(prefab, bgPos, Quaternion.identity, transform);

            _backgroundObjs.Add(backgrounds);
        }
    }*/
}