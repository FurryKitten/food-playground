using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] float _mass = 1.0f;
    [SerializeField] Vector2 _centerMass;

    public void Init(FigureSO figure)
    {
        // задаем коллайдер из данных фигуры
    }
}
