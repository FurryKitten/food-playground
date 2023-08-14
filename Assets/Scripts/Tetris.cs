using System.Collections.Generic;
using UnityEngine;


public struct Grid
{
    public int width, height;

    
}

public class Tetris : MonoBehaviour
{
    [SerializeField] private FigureSO[] _figurePrefabs;


    private Queue<Figure> figureQueue;
    private Figure currentFigure;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
