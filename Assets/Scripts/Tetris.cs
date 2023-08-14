using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


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
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = new PlayerController();
        _playerController.Tetris.Move.started += HorizontalMove;
    }

    private void Start()
    {
        _figureSOIdQueue = new Queue<int>();
        _figureSOIdQueue.Enqueue(0); // TODO: generate or premade queue SO
        _figureSOIdQueue.Enqueue(1); // TODO: generate or premade queue SO
        _gameSpace.width = 15;
        _gameSpace.height = 10;
        _gameSpace.cellsStatus = new bool[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.cellsStatus[i, j] = false;
        }

        ControlsOnEnable();
    }

    private void Update()
    {

        if (_flyingFigure == null)
        {
            if (_figureSOIdQueue.TryPeek(out int figureSOId))
            {
                _flyingFigure = Instantiate(_defaultFigure);
                _flyingFigure.Init(_figureSOPrefabs[figureSOId]);
                _figureSOIdQueue.Dequeue();

                _flyingFigure.SetPosition(_figureStartPos.x, _figureStartPos.y);

            }
            else
            {
                //stop tetris
                //_playerController.Tetris.Move.started -= HorizontalMove;
            }
        }
        else
        {
            _movementTimer += Time.deltaTime;
            if (_movementTimer >= _movementTime)
            {
                MoveFlyingFigure();
                _movementTimer = 0;
            }
        }
    }

    private void ControlsOnEnable()
    {
        _playerController.Enable();
    }

    private void ControlsOnDisable()
    {
        _playerController.Disable();
    }
    private void MoveFlyingFigure()
    {
        Vector2 _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = Mathf.RoundToInt(_flyingFigurePos.x);
        int gridY = Mathf.RoundToInt(_flyingFigurePos.y);

        bool checkGridSpace = false;
        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridY + pos.y - 1) > 0)
            {
                if (_gameSpace.cellsStatus[gridX + pos.x, gridY + pos.y - 1])
                {
                    checkGridSpace = true;
                    break;
                }
            }
            else
            {
                checkGridSpace = true;
                break;
            }
       
        if (!checkGridSpace)
        { 
            _flyingFigure.Fall(); 
        }
        else
        {
            foreach (Vector2Int pos in _flyingFigure.GetForm())
                _gameSpace.cellsStatus[gridX + pos.x, gridY + pos.y] = true;

            _flyingFigure = null;
        }
    }

    private bool checkHorizontalMove(int dir)
    {
        Vector2 _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = Mathf.RoundToInt(_flyingFigurePos.x) + dir;
        int gridY = Mathf.RoundToInt(_flyingFigurePos.y);

        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridX + pos.x ) > 0 && (gridX + pos.x ) < _gameSpace.width)
            {
                if (_gameSpace.cellsStatus[gridX + pos.x, gridY + pos.y])
                    return true;
            }
            else
                return true;

        return false;
    }

    private void HorizontalMove(InputAction.CallbackContext context)
    {
        if (_flyingFigure == null)
            return;

        float inpDir = _playerController.Tetris.Move.ReadValue<float>();

        int dir = 0;

        if (inpDir > 0)
            dir = 1;
        else if (inpDir < 0)
            dir = -1;

        if (!checkHorizontalMove(dir))
        {
            _flyingFigure.HorizontalMove(dir);
        }
        
    }


    #region DEBUG

    private void OnDrawGizmos()
    {
        
    }

    #endregion
}
