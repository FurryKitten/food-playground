using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
    [SerializeField] private int _queueSize;
    [SerializeField] private HandControls _handControls;
    [SerializeField] private float _trayAngle;



    private Queue<int> _figureSOIdQueue;
    private Figure _flyingFigure = null;
    private Grid _gameSpace;
    private float _movementTimer = 0;
    private PlayerController _playerController;
    private List<Figure> _figureList = new List<Figure>();
    private float _dashTime;
    private float _figureListTimer = 0;
    private bool _dashMode = false;

    private GameState _gameState;

    private float _balaceValue = 0;
    private BalaceState _balaceState;

    private void Awake()
    {
        _playerController = new PlayerController();
        _playerController.Tetris.Move.started += HorizontalMove;
        _playerController.Enable();

        _playerController.Tetris.Dash.started += DashMode;
    }

    private void Start()
    {
        _figureSOIdQueue = new Queue<int>();
        //_figureSOIdQueue.Enqueue(0); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(1); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(0);
        GenerateQueue();
        _gameSpace.width = 16;
        _gameSpace.height = 15;
        _gameSpace.cellsStatus = new bool[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.cellsStatus[i, j] = false;
        }


        _dashTime = 0.1f * _movementTime;

        _gameState = ServiceLocator.Current.Get<GameState>();
    }
    private void Update()
    {
        if (_gameState.State != State.TETRIS)
            return;

        _balaceValue = _handControls.CheckBalance();

        int rotationDir = 0;

        if (_balaceValue != 0)
        {
            if (_balaceValue > 0)
            {
                rotationDir = 1;
                _balaceState = BalaceState.Left;
            }
            else
            {
                rotationDir = -1;
                _balaceState = BalaceState.Right;
            }
        }
        else
        {
            _balaceState = BalaceState.OK;
            if (transform.parent.rotation.eulerAngles.z > 0.1)
            {
                rotationDir = -1;
            }
            else if (transform.parent.rotation.eulerAngles.z > 340 && transform.parent.rotation.eulerAngles.z < 360f)
            {
                rotationDir = 1;
            }
        }

        transform.parent.Rotate(0, 0, rotationDir * _trayAngle * Time.deltaTime);
        
        transform.parent.rotation = Quaternion.Euler(0, 0, RotationClamp(transform.parent.rotation.eulerAngles.z));
        

        if (_flyingFigure == null)
        {
            if (_figureSOIdQueue.TryPeek(out int figureSOId))
            {
                if (_gameSpace.cellsStatus[_figureStartPos.x, _figureStartPos.y])
                {
                    //TO DO: tetris lose
                }
                Vector2 pos = transform.parent.position;
                _flyingFigure = Instantiate(_defaultFigure, transform.parent);
                _flyingFigure.Init(_figureSOPrefabs[figureSOId]);
                _figureSOIdQueue.Dequeue();

                _flyingFigure.SetPosition(_figureStartPos.x, _figureStartPos.y);
                _flyingFigure.SetWorldPosition(_figureStartPos - pos);


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
            if (!_dashMode)
            {

                if (_movementTimer >= _movementTime)
                {
                    MoveFlyingFigure();
                    _movementTimer = 0;

                }
            }
            else
            {
                if (_movementTimer >= _dashTime)
                {
                    MoveFlyingFigure();
                    _movementTimer = 0;
                }
            }
        }

        
        _figureListTimer += Time.deltaTime;
        if (_figureListTimer > _movementTime)
        {
            _figureListTimer = 0;
            if (_figureList.Count != 0)
            {
                VerticalMoveFigureList();
                if (transform.parent.rotation.eulerAngles.z > 10 && transform.parent.rotation.eulerAngles.z < 21)
                {
                    HorizontalMoveFigureList(-1);
                }
                else if(transform.parent.rotation.eulerAngles.z < 350 && transform.parent.rotation.eulerAngles.z > 339)
                    HorizontalMoveFigureList(1);
            }
        }
    
    }

    private void MoveFlyingFigure()
    {
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = _flyingFigurePos.x;
        int gridY = _flyingFigurePos.y;

        bool checkGridSpace = false;
        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridY - pos.y - 1) >= 0)
            {
                if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y - 1])
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
                _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = true;

            _figureList.Add(_flyingFigure);
            _handControls.AddFigures(_figureList);

            _flyingFigure = null;
            _dashMode = false;
        }
    }

    private float RotationClamp(float angle)
    {
        if(angle > 20 && angle < 30)
        {
            return Mathf.Clamp(angle, 0, 20);
        }
        else
        {
            if(angle > 20)
            {
                return Mathf.Clamp(angle, 340, 360);
            }
        }

        return angle;
    }
    private bool checkHorizontalMove(int dir)
    {
        Vector2 _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = Mathf.RoundToInt(_flyingFigurePos.x) + dir;
        int gridY = Mathf.RoundToInt(_flyingFigurePos.y);

        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridX + pos.x ) >= 0 && (gridX + pos.x ) < _gameSpace.width && gridY - pos.y >= 0)
            {
                if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y])
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

    private void HorizontalMoveFigureList(int dir)
    {
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.cellsStatus[i, j] = false;
        }

        int gridXFlying = -100;
        int gridYFlying = -100;
        if (_flyingFigure != null)
        {
            Vector2 _flyingFigurePos = _flyingFigure.GetPosition();
            gridXFlying = Mathf.RoundToInt(_flyingFigurePos.x);
            gridYFlying = Mathf.RoundToInt(_flyingFigurePos.y);
        }

        List<int> lostFigureIndexes = new List<int>();
        for(int i = 0; i < _figureList.Count; i++)
        {
            Vector2 _fPos = _figureList[i].GetPosition();
            int gridX = Mathf.RoundToInt(_fPos.x) + dir;
            int gridY = Mathf.RoundToInt(_fPos.y);

            bool boundaryCheck = false;

            foreach (Vector2Int pos in _figureList[i].GetForm())
            {
                if ((gridX + pos.x) < 0 || (gridX + pos.x) == _gameSpace.width)
                {
                    boundaryCheck = true;
                    break;
                }
                else
                {
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = true;
                }

                if((gridX + pos.x) == gridXFlying && (gridY - pos.y) == gridYFlying)
                {
                    boundaryCheck = true;
                    break;
                }
            }

            _figureList[i].HorizontalMove(dir);


            if(boundaryCheck)
            {
                _figureList[i].FlyAway(dir);
                lostFigureIndexes.Add(i);
            }

        }

        if (lostFigureIndexes.Count > 0)
        {
            int offset = 0;
            foreach (int i in lostFigureIndexes)
            {
                _figureList.RemoveAt(i - offset);
                offset++;
            }

            _handControls.AddFigures(_figureList);
            
        }
    }

    private void VerticalMoveFigureList()
    {
        foreach (Figure f in _figureList)
        {
            Vector2 _fPos = f.GetPosition();
            int gridX = Mathf.RoundToInt(_fPos.x);
            int gridY = Mathf.RoundToInt(_fPos.y);

            bool checkGridSpace = false;

            foreach (Vector2Int pos in f.GetForm())
            {
                _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
            }

            foreach (Vector2Int pos in f.GetForm())
                if ((gridY - pos.y - 1) >= 0)
                {
                    if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y - 1])
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
                foreach (Vector2Int pos in f.GetForm())
                {
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
                }
            }
            else
            {
                foreach (Vector2Int pos in f.GetForm())
                {
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = true;
                }
            }

        }

        foreach (Figure f in _figureList)
        {
            Vector2 _fPos = f.GetPosition();
            int gridX = Mathf.RoundToInt(_fPos.x);
            int gridY = Mathf.RoundToInt(_fPos.y);

            bool checkGridSpace = false;

            foreach (Vector2Int pos in f.GetForm())
                if ((gridY - pos.y - 1) >= 0)
                {
                    if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y - 1])
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

            if(!checkGridSpace)
            {
                f.Fall();
                foreach (Vector2Int pos in f.GetForm())
                {
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y - 1] = true;
                }
            }

        }
    }

    private void DashMode(InputAction.CallbackContext context)
    {
        _dashMode = true;
    }

    private void GenerateQueue()
    {
        for(int i = 0; i < _queueSize; i++)
            _figureSOIdQueue.Enqueue(Random.Range(0, _figureSOPrefabs.Length));
    }

    #region DEBUG

    private void OnDrawGizmos()
    {
        Vector2 pos = this.transform.localPosition;
        for (int i = 0; i <= _gameSpace.width; ++i)
        {
            Vector2 offset = new Vector2(i, 0);
            Gizmos.DrawLine(pos + offset, pos + offset + new Vector2(0, _gameSpace.height));
        }
        for (int j = 0; j <= _gameSpace.height; ++j)
        {
            Vector2 offset = new Vector2(0, j);
            Gizmos.DrawLine(pos + offset + new Vector2(_gameSpace.width, 0), pos + offset );
        }

        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
            {
                Gizmos.color = Color.gray;
                if (_gameSpace.cellsStatus[i, j])
                    Gizmos.color = Color.yellow;
                Gizmos.DrawCube(pos + new Vector2(i + 0.5f, j + 0.5f), Vector2.one * 0.3f);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pos + new Vector2(_figureStartPos.x + 0.5f, _figureStartPos.y + 0.5f), Vector2.one * 0.3f);
    }

    #endregion
}
