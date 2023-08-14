using System;
using System.Collections.Generic;
using UnityEngine;


public struct Grid
{
    public int width, height;
    public bool[,] cellsStatus;
}

public class Tetris : MonoBehaviour
{
    [SerializeField] private Vector2Int _figureStartPos;
    [SerializeField] private FigureSO[] _figureSOPrefabs;
    [SerializeField] private Figure _defaultFigure;
    [SerializeField] private float _movementTime;


    private Queue<int> _figureSOIdQueue;
    private Figure _flyingFigure = null;
    private Grid _gameSpace;
    private float _movementTimer = 0;

    private void Start()
    {
        _figureSOIdQueue = new Queue<int>();
        _figureSOIdQueue.Enqueue(0);
        _figureSOIdQueue.Enqueue(0);
        _gameSpace.width = 15;
        _gameSpace.height = 10;
        _gameSpace.cellsStatus = new bool[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.cellsStatus[i, j] = false;
        }
    }

    private void Update()
    {
       if(_flyingFigure == null)
        {
            if (_figureSOIdQueue.TryPeek(out int figureSOId))
            {
                _flyingFigure = Instantiate(_defaultFigure);
                print(figureSOId);
                _flyingFigure.Init(_figureSOPrefabs[figureSOId]);
                _figureSOIdQueue.Dequeue();

                _flyingFigure.SetPosition(_figureStartPos.x, _figureStartPos.y);
            }
            else
            {
                //stop tetris
            }
        }
        else
        {
            
            _movementTimer += Time.deltaTime;
            if(_movementTimer >= _movementTime)
            {
                MoveFlyingFigure();
                _movementTimer = 0;
            }
            
        }

    }

    private void MoveFlyingFigure()
    {
        Vector2 _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = Mathf.RoundToInt(_flyingFigurePos.x);
        int gridY = Mathf.RoundToInt(_flyingFigurePos.y);

        if (!_gameSpace.cellsStatus[gridX , gridY - 1] && (gridY - 1) > 0)
        { 
            _flyingFigure.Fall(); 
        }
        else
        {
            _gameSpace.cellsStatus[gridX, gridY] = true;
            _flyingFigure = null;
        }
    }
}
