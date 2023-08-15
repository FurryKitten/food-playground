using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;
    
    private Transform _transform;
    private Vector2Int _centerPos; // TODO: pivot to top left corner
    private Vector2Int[] _form;
    private int _width;
    private int _height;
    public void Init(FigureSO figure)
    {
        // задаем коллайдер из данных фигуры
        _transform = GetComponent<Transform>();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = figure.sprite;
        _form = figure.form;
        _width = figure.width;
        _height = figure.height;
    }

    public void SetPosition(int x, int y)
    {
        _centerPos = new Vector2Int(x, y);
    }

    public void SetWorldPosition(Vector2 pos)
    {
        _transform.position = pos;
    }

    public void Fall()
    {
        _centerPos.y -= 1;
        Vector3 pos = _transform.position;
        pos.y -= 1;
        _transform.position = pos;
    }

    public Vector2Int GetPosition()
    {
        return _centerPos;
    }

    public Vector2Int[] GetForm()
    {
        return _form;
    }

    public void HorizontalMove(int dir)
    {
        _centerPos.x += dir;
        Vector3 pos = _transform.position;
        pos.x += dir;
        _transform.position = pos;
    }
}
