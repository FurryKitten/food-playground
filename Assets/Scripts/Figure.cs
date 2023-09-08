﻿using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    [SerializeField] Material _figureMaterial;
    
    private Transform _transform;
    private Vector2Int _centerPos; // TODO: pivot to top left corner
    private Vector2Int[] _form;
    private int _cost;
    private bool _doubleCost = false;
    private int _index = 0;
    private Material _material;
    private bool _spoiled = false;
    private float _height = 0.2f; // 0.2f - размер одной клетки, такой формат необходим для шейдера
    private bool _shaderAnimation = false;
    private float _step = 0.7f;
    private float _targetStep = 0f;
    private bool _shaderStateGold2Spoiled = false;

    public void Init(FigureSO figure)
    {
        _transform = GetComponent<Transform>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.sprite;
        _material = new Material(_figureMaterial);
        _material.SetVector("_Tilling", new Vector2(figure.widthTex, figure.heightTex));
        _index = figure.indexTex;
        _material.SetFloat("_Index", figure.indexTex);
        gameObject.GetComponent<SpriteRenderer>().material = _material;
        _form = figure.form;
        _cost = figure.cost;
        _height = figure.heightTex;
    }
    public void SetPosition(int x, int y)
    {
        _centerPos = new Vector2Int(x, y);
    }

    public void SetWorldPosition(Vector2 pos)
    {
        _transform.localPosition = pos;
    }

    public void Fall()
    {
        _centerPos.y -= 1;
        Vector3 pos = _transform.localPosition;
        pos.y -= 1;
        _transform.localPosition = pos;
    }

    public Vector2Int GetPosition()
    {
        return _centerPos;
    }

    public Vector3 GetWorldPosition()
    {
        return _transform.localPosition;
    }
    public Vector2Int[] GetForm()
    {
        return _form;
    }

    public void HorizontalMove(int dir)
    {
        _centerPos.x += dir;
        Vector3 pos = _transform.localPosition;
        pos.x += dir;
        _transform.localPosition = pos;
    }

    public void FlyAway(int dir)
    {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(dir * 200, 2));

        ServiceLocator.Current.Get<GameState>().AddTrayMoney(GetFine());
    }

    public void FlyAway()
    {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 200);
    }

    public int GetProfit() 
    {
        return (_spoiled) ? 0 :((_doubleCost) ? 2 * _cost : _cost);
    }

    public int GetFine() 
    {
        return (_doubleCost) ? -2 * _cost : -_cost;
    }

    public void SetDoubleCost()
    {
        _doubleCost = true;
        _material.SetInt("_Gold2Normal", 1);
        _targetStep = 0;
        if (!_shaderAnimation)
            StartCoroutine(ShaderAnimation());
    }

    public void ChangeSpoiledStatus(bool status)
    {
        if (_spoiled != status)
        {
            _spoiled = status;

            if(_doubleCost && !_shaderStateGold2Spoiled)
            {
                _shaderStateGold2Spoiled = true;
                _material.SetInt("_Gold2Spoiled", 1);
                _step = 0.7f;
                _material.SetFloat("_StepTimer", _step);
            }

            if (_spoiled)
            {
                //анимация порченья
                _targetStep = 0;
                if (!_shaderAnimation)
                    StartCoroutine(ShaderAnimation());
            }
            else
            {
                //анимация восстановления
                _targetStep = 0.7f;
                if (!_shaderAnimation)
                    StartCoroutine(ShaderAnimation());
            }
        }
    }

    private IEnumerator ShaderAnimation()
    {
        float t = 0;
        const float animationSpeed = 0.8f;
        _shaderAnimation = true;
        while (t < 1)
        {
            _step = Mathf.Lerp(_step, _targetStep, t*t*t);
            t += Time.deltaTime * animationSpeed;
            _material.SetFloat("_StepTimer", _step);
            yield return null; 
        }
        _shaderAnimation = false;
       // Debug.Log("Animation end");
    }

    private IEnumerator NextAnimation()
    {
        while (_shaderAnimation)
        {
            yield return null;
        }
        StartCoroutine(ShaderAnimation());
    }
    public void DestroySpoiler()
    {
        // TO DO: Start animation of destroy spoiler
    }

    public int Index
    { get { return _index; } }

    public bool IsSpoiled
    { get { return _spoiled; } }

    public float Height
    { get { return _height; } }
}
