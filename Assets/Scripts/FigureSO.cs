using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation
{
    UP, DOWN, LEFT, RIGHT
}

[CreateAssetMenu(fileName = "Figure", menuName = "ScriptableObjects/Figures", order = 1)]
public class FigureSO : ScriptableObject
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] Vector2Int[] _form;
    [SerializeField] float _mass;
    [SerializeField] Sprite _sprite;

    
    public int width
    { get { return _width; } }

    public int height
    { get { return _height; } }

    public Vector2Int[] form
        { get { return _form; } }

    public float mass
    { get { return _mass; } }
    public Sprite sprite
    { get { return _sprite; } }

    public Rotation rotation
    { get { return _rotation; } }
}
