﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    
    private Transform _transform;
    private Vector2 _centerPos;
    public void Init(FigureSO figure)
    {
        // задаем коллайдер из данных фигуры
        _transform = GetComponent<Transform>();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.sprite;
    }

    public void SetPosition(int x, int y)
    {
        _centerPos = new Vector2Int(x, y);
        _transform.position = _centerPos;
    }

    public void Fall()
    {
        _centerPos.y -= 1;
        _transform.position = _centerPos;
    }

    public Vector2 GetPosition()
    {
        return _centerPos;
    }
}
