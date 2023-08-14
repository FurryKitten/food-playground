using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Rotation
{
    UP, DOWN, LEFT, RIGHT
}

[CreateAssetMenu(fileName = "Figure", menuName = "ScriptableObjects/Figures", order = 1)]
public class FigureSO : ScriptableObject
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] Vector2Int[] _figure;
    [SerializeField] float _mass;
    [SerializeField] Sprite _sprite;
    [SerializeField] Rotation _rotation;
}
