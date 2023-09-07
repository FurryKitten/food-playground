using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public struct Grid
{
    public int width, height;
    public Figure[,] figureGrid;
}

public class Tetris : MonoBehaviour, IService
{
    [SerializeField] private Vector2Int _figureStartPos;
    [SerializeField] private FigureSO[] _figureSOPrefabs;
    [SerializeField] private Figure _defaultFigure;
    [SerializeField] private float _movementTime;
    [SerializeField] private int _queueSize;
    [SerializeField] private HandControls _handControls;
    [SerializeField] private float _trayAngle;

    [SerializeField] private UnityEvent _onFigureFall;


    private Queue<int> _figureSOIdQueue;
    private Figure _flyingFigure = null;
    private Grid _gameSpace;
    private float _movementTimer = 0;
    private PlayerController _playerController;
    private List<Figure> _figureList = new List<Figure>();
    private float _dashTime;
    private float _figureListTimer = 0;
    private bool _dashMode = false;
    private bool _dashOffOnSpawning = false;
    private float _spawnTimer = 0;
    private bool _lastMove = false;

    private GameState _gameState;

    private float _balaceValue = 0;

    private bool _doubleCost = false;
    private bool _trayBorders = false;
    private int _leftGridConstrain = 4;
    private int _rightGridConstrain = 14;
    private int _gridXOffsetFromWorld = 0;
    

    private int _stageNumber = 0;
    private int _trayNumber = 0;
    private int[,] _queueSizes = {  { 5, 6, 8, 10 }, 
                                    { 6, 8, 10, 12 }, 
                                    { 7, 9, 11, 14 },
                                    { 8, 10, 12, 15 }};

    private void Awake()
    {
        _playerController = new PlayerController();
        _playerController.Tetris.Move.started += HorizontalMove;
        _playerController.Enable();

        _playerController.Tetris.Dash.started += DashMode;
        _playerController.Tetris.Dash.canceled += DashMode;
    }

    private void Start()
    {
        _figureSOIdQueue = new Queue<int>();
        //_figureSOIdQueue.Enqueue(0); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(1); // TODO: generate or premade queue SO
        //_figureSOIdQueue.Enqueue(0);
        //GenerateQueue();
        GenerateQueue(_queueSizes[0, 0]);
        _gameSpace.width = 18;
        _gridXOffsetFromWorld = (_gameSpace.width - 16) / 2;
        _gameSpace.height = 15;
        _gameSpace.figureGrid = new Figure[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
            {
                _gameSpace.figureGrid[i, j] = null;
            }
        }

        _dashTime = 0.1f * _movementTime;

        _gameState = ServiceLocator.Current.Get<GameState>();
    }
    private void Update()
    {
        TetrisUpdate();
    }

    public void TetrisUpdate()
    {
        if (_gameState.State == State.PAUSED)
            return;

        RotateTray();

        _figureListTimer += Time.deltaTime;
        if (_figureListTimer > _movementTime)
        {
            _figureListTimer = 0;
            if (_figureList.Count != 0)
            {
                VerticalMoveFigureList();
                if (transform.parent.rotation.eulerAngles.z > 10 && transform.parent.rotation.eulerAngles.z <= 30)
                {
                    HorizontalMoveFigureList(-1);
                }
                else if (transform.parent.rotation.eulerAngles.z < 350 && transform.parent.rotation.eulerAngles.z >= 330)
                    HorizontalMoveFigureList(1);
            }
        }

        if (_gameState.State == State.TETRIS)
        {
            if (_flyingFigure == null)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer > _movementTime) // Спавн
                {
                    _spawnTimer = 0;
                    if (!FigureSpawn())
                        FinishStage();
                }
            }
            else
            {
                _movementTimer += Time.deltaTime;
                if (_movementTimer >= (_dashMode ? _dashTime : _movementTime))
                {
                    MoveFlyingFigure();
                    _movementTimer = 0;
                }
            }
        }
    }

    private void FinishStage()
    {
        //stop tetris, walk, reset queue
        _gameState.SetState(State.WALK);
        _gameState.AddStage();

        _figureSOIdQueue.Clear();
        if (_stageNumber != 3)
            _stageNumber++;
        else
            _stageNumber = 0;

        GenerateQueue(_queueSizes[_trayNumber, _stageNumber]);
    }
    private void RotateTray()
    {
        _balaceValue = _handControls.CheckBalance();

        int rotationDir = 0;

        if (_balaceValue != 0)
        {
            if (_balaceValue > 0)
            {
                rotationDir = 1;
            }
            else
            {
                rotationDir = -1;
            }
        }
        else
        {
            if (transform.parent.rotation.eulerAngles.z >= 340 && transform.parent.rotation.eulerAngles.z < 360f)
            {
                rotationDir = 1;
            }
            else
            {
                if (transform.parent.rotation.eulerAngles.z > 0.1)
                {
                    rotationDir = -1;
                }
                else
                {
                    transform.parent.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        transform.parent.Rotate(0, 0, rotationDir * _trayAngle * Time.deltaTime);

        transform.parent.rotation = Quaternion.Euler(0, 0, RotationClamp(transform.parent.rotation.eulerAngles.z));
    }
    private void MoveFlyingFigure()
    {
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = _flyingFigurePos.x + _gridXOffsetFromWorld;
        int gridY = _flyingFigurePos.y;

        bool checkGridSpace = false;
        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridY - pos.y - 1) >= 0)
            {
                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != null)
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
        else // Ставим на доску
        {
            if (_lastMove)
            {
                foreach (Vector2Int pos in _flyingFigure.GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = _flyingFigure;
                }

                _figureList.Add(_flyingFigure);
                _handControls.AddFigures(_figureList);

                _gameState.AddTrayMoney(_flyingFigure.GetProfit());

                _flyingFigure = null;
                _spawnTimer = _movementTime;

                ServiceLocator.Current.Get<AudioService>().PlayTetrisLanding();
            }
            else
            {
                _lastMove = true;
            }
        }
    }
    private bool FigureSpawn()
    {
        if (_figureSOIdQueue.TryPeek(out int figureSOId)) // Фигура заспавнилась
        {

            Vector2 posParent = transform.parent.position;
            _flyingFigure = Instantiate(_defaultFigure, transform.parent);
            _flyingFigure.Init(_figureSOPrefabs[figureSOId], figureSOId);
            _flyingFigure.name = "FlyingFigure";
            _figureSOIdQueue.Dequeue();

            _flyingFigure.SetPosition(_figureStartPos.x, _figureStartPos.y);
            _flyingFigure.SetWorldPosition(_figureStartPos - posParent);

            ServiceLocator.Current.Get<AudioService>().PlayTetrisSpawn();
            if (_doubleCost)
            {
                if ((Random.Range(0f, 1f) < 0.3f))
                {
                    _flyingFigure.SetDoubleCost();
                    ServiceLocator.Current.Get<AudioService>().PlayTetrisGoldSpawn();
                }
            }


            bool placeCheck = true;
            // Если нет места для спавна - удаляем фигуры
            foreach (Vector2Int p in _flyingFigure.GetForm())
            {
                if (_gameSpace.figureGrid[_figureStartPos.x + _gridXOffsetFromWorld + p.x, _figureStartPos.y - p.y] != null)
                {
                    int index = _figureList.IndexOf(_gameSpace.figureGrid[_figureStartPos.x + _gridXOffsetFromWorld + p.x, _figureStartPos.y - p.y]);
                    Vector2Int _fPos = _figureList[index].GetPosition();
                    int gridX = (_fPos.x) + _gridXOffsetFromWorld;
                    int gridY = (_fPos.y);

                    foreach (Vector2Int pos in _figureList[index].GetForm())
                    {
                        _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                    }
                    _figureList[index].FlyAway(-1);
                    _onFigureFall?.Invoke();
                    _figureList.RemoveAt(index);
                    placeCheck = false;
                }
            }

            if(!placeCheck)
            {
                _flyingFigure.FlyAway();
                _flyingFigure = null;
                _spawnTimer = 0;
                _handControls.AddFigures(_figureList);
                ServiceLocator.Current.Get<AudioService>().PlayTetrisBadSpawn();
            }

            // Отмена _dashModa при спавне
            if (_dashMode)
            {
                _dashMode = false;
                _dashOffOnSpawning = true;
            }
            return true;
        }
        
        return false;
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
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = (_flyingFigurePos.x) + dir + _gridXOffsetFromWorld;
        int gridY = (_flyingFigurePos.y);

        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridX + pos.x ) >= _leftGridConstrain && (gridX + pos.x ) < _rightGridConstrain && gridY - pos.y >= 0)
            {
                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] != null)
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
        ResetGrid4HorizontalMoveFigureList(dir);

        int gridXFlying = -100;
        int gridYFlying = -100;
        if (_flyingFigure != null)
        {
            Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
            gridXFlying = (_flyingFigurePos.x) + _gridXOffsetFromWorld;
            gridYFlying = (_flyingFigurePos.y);
        }

        List<int> lostFigureIndexes = new List<int>();
        for(int i = 0; i < _figureList.Count; i++)
        {
            Vector2Int _fPos = _figureList[i].GetPosition();
            int gridX = (_fPos.x) + dir + _gridXOffsetFromWorld;
            int gridY = (_fPos.y);

            bool boundaryCheck = false;
            bool bordured = false;
            bool migthBordured = false;

            foreach (Vector2Int pos in _figureList[i].GetForm())
            {
                if (((gridX + pos.x) < _leftGridConstrain || (gridX + pos.x) == _rightGridConstrain))
                {
                    if (!_trayBorders)
                    {
                        boundaryCheck = true;
                        break;
                    }

                    if (gridY - pos.y == 0)
                    { 
                        bordured = true;
                    }
                    else
                    {
                        boundaryCheck = true;
                    }
                }
                else
                {
                    if(gridX + pos.x >= 0 && gridX + pos.x < _gameSpace.width)
                        if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] != null)
                        {
                            bordured = true;
                        }
                        else
                        {
                            if (_trayBorders)
                            {
                                if(gridY - pos.y == 0 && 
                                    (gridX + pos.x == _leftGridConstrain || gridX + pos.x == _rightGridConstrain-1))
                                    migthBordured = true;
                            }
                        }
                }

                if((gridX + pos.x) == gridXFlying && (gridY - pos.y) == gridYFlying)
                {
                    boundaryCheck = true;
                    break;
                }
            }

            if (!bordured)
                _figureList[i].HorizontalMove(dir);

            if(migthBordured)
                boundaryCheck = false;

            if(boundaryCheck && !bordured)
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    if ((gridX + pos.x) < _leftGridConstrain || (gridX + pos.x) == _rightGridConstrain ||
                        (gridX + pos.x) == gridXFlying && (gridY - pos.y) == gridYFlying)
                        continue;
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                }
                _figureList[i].FlyAway(dir);
                _onFigureFall?.Invoke();
                lostFigureIndexes.Add(i);
            }

            if(bordured)
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x - dir, gridY - pos.y] = _figureList[i];
                }
            }

            if(!boundaryCheck && !bordured)
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = _figureList[i];
                }
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

    private void ResetGrid4HorizontalMoveFigureList(int dir)
    {
        if (dir > 0)
        {
            List<Figure> checkedFigures = new List<Figure>();
            for (int i = _gameSpace.width - 1; i > -1; i--)
                for (int j = 0; j < _gameSpace.height; j++)
                {
                    if (!checkedFigures.Contains(_gameSpace.figureGrid[i, j]) && _gameSpace.figureGrid[i, j] != null)
                    {
                        checkedFigures.Add(_gameSpace.figureGrid[i, j]);
                        Figure lastFigure = _gameSpace.figureGrid[i, j];

                        Vector2Int _fPos = lastFigure.GetPosition();
                        int gridX = (_fPos.x) + _gridXOffsetFromWorld;
                        int gridY = (_fPos.y);

                        bool moveCheck = true;

                        if (_trayBorders)
                        {
                            foreach (Vector2Int pos in lastFigure.GetForm())
                            {
                                if (gridY - pos.y == 0 &&
                                    (gridX + pos.x + dir == _rightGridConstrain
                                    || gridX + pos.x + dir < _leftGridConstrain))
                                {
                                    moveCheck = false;
                                    break;
                                }
                                if(gridX + pos.x + dir >= _gameSpace.width)
                                {
                                    moveCheck = false;
                                    break;
                                }

                                if (_gameSpace.figureGrid[gridX + pos.x + dir, gridY - pos.y] != lastFigure &&
                                        _gameSpace.figureGrid[gridX + pos.x + dir, gridY - pos.y] != null)
                                {
                                    moveCheck = false;
                                    break;
                                }
                            }
                        }

                        if (moveCheck)
                        {
                            foreach (Vector2Int pos in lastFigure.GetForm())
                            {
                                _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                            }
                        }
                    }
                }

        }
        else
        {
            List<Figure> checkedFigures = new List<Figure>();
            for (int i = 0; i < _gameSpace.width; i++)
                for (int j = 0; j < _gameSpace.height; j++)
                {
                    if (!checkedFigures.Contains(_gameSpace.figureGrid[i, j]) && _gameSpace.figureGrid[i, j] != null)
                    {
                        checkedFigures.Add(_gameSpace.figureGrid[i, j]);
                        Figure lastFigure = _gameSpace.figureGrid[i, j];

                        Vector2Int _fPos = lastFigure.GetPosition();
                        int gridX = (_fPos.x) + _gridXOffsetFromWorld;
                        int gridY = (_fPos.y);

                        bool moveCheck = true;

                        if (_trayBorders)
                        {
                            foreach (Vector2Int pos in lastFigure.GetForm())
                            {
                                if (gridY - pos.y == 0 &&
                                    (gridX + pos.x + dir == _rightGridConstrain
                                    || gridX + pos.x + dir < _leftGridConstrain))
                                {
                                    moveCheck = false;
                                    break;
                                }

                                if (gridX + pos.x + dir < 0)
                                {
                                    moveCheck = false;
                                    break;
                                }

                                if (_gameSpace.figureGrid[gridX + pos.x + dir, gridY - pos.y] != lastFigure &&
                                    _gameSpace.figureGrid[gridX + pos.x + dir, gridY - pos.y] != null)
                                {
                                    moveCheck = false;
                                    break;
                                }
                            }
                        }

                        if (moveCheck)
                        {
                            foreach (Vector2Int pos in lastFigure.GetForm())
                            {
                                _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                            }
                        }
                    }
                }
        }
    }

    private void VerticalMoveFigureList()
    {

        List<Figure> checkedFigures = new List<Figure>();
        for (int i = 0; i < _gameSpace.width; i++)
            for (int j = 1; j < _gameSpace.height; j++)
            {
                if (!checkedFigures.Contains(_gameSpace.figureGrid[i, j]) && _gameSpace.figureGrid[i, j] != null)
                {
                    checkedFigures.Add(_gameSpace.figureGrid[i, j]);
                    Figure lastFigure = _gameSpace.figureGrid[i, j];

                    Vector2Int _fPos = lastFigure.GetPosition();
                    int gridX = (_fPos.x) + _gridXOffsetFromWorld;
                    int gridY = (_fPos.y);

                    bool moveCheck = true;

                    
                    foreach (Vector2Int pos in lastFigure.GetForm())
                    {
                        if (gridY - pos.y == 0)
                        {
                            moveCheck = false;
                            break;
                        }
                        if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != lastFigure &&
                            _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] != null)
                        {
                            moveCheck = false;
                            break;
                        }
                    }
                   

                    if (moveCheck)
                    {
                        foreach (Vector2Int pos in lastFigure.GetForm())
                        {
                            _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] = null;
                        }
                    }
                }
            }

        foreach (Figure f in _figureList)
        {
            Vector2Int _fPos = f.GetPosition();
            int gridX = (_fPos.x) + _gridXOffsetFromWorld;
            int gridY = (_fPos.y);
            
            bool checkGridSpace = false;


            foreach (Vector2Int pos in f.GetForm())
            {
                if (_gameSpace.figureGrid[gridX + pos.x, gridY - pos.y] != null)
                {
                    checkGridSpace = true;
                    break;
                }
            }

            if (!checkGridSpace)
            {
                f.Fall();
                foreach (Vector2Int pos in f.GetForm())
                {
                    _gameSpace.figureGrid[gridX + pos.x, gridY - pos.y - 1] = f;
                }
            }

        }
    }

    private void DashMode(InputAction.CallbackContext context)
    {
        if (_dashOffOnSpawning && !_playerController.Tetris.Dash.triggered)
        {
            _dashMode = false;
            _dashOffOnSpawning = false;
        }
        else
            _dashMode = !_dashMode;
    }



    private void GenerateQueue()
    {
        for(int i = 0; i < _queueSize; i++)
            _figureSOIdQueue.Enqueue(Random.Range(0, _figureSOPrefabs.Length));
    }

    private void GenerateQueue(int queueSize)
    {
        for (int i = 0; i < queueSize; i++)
            _figureSOIdQueue.Enqueue(Random.Range(0, _figureSOPrefabs.Length));
    }

    public void SetDoubleCost()
    {
        _doubleCost = true;
    }

    public void SetTrayBorders()
    {
        _trayBorders = true;
    }

    public void SetGridWidth(int trayWidth) // TO DO: use Unity Event
    {
        _leftGridConstrain -= 1;
        _rightGridConstrain += 1;
        _trayNumber = Mathf.RoundToInt((trayWidth - 10) * 0.5f);
    }

    public void DropAllFigures()
    {
        foreach(Figure figure in _figureList)
        {
            figure.FlyAway(); ;
        }
        _figureList.Clear();

        ServiceLocator.Current.Get<AudioService>().PlayTetrisJingle();
    }
    
    public void ResetTetris()
    {
        for (int i = 0; i < _gameSpace.width; ++i)
            for (int j = 0; j < _gameSpace.height; ++j)
                _gameSpace.figureGrid[i, j] = null;

        foreach (var figure in _figureList)
        {
            Destroy(figure.gameObject);
        }

        //костыль для удаления физичных фигур
        foreach (var figure in FindObjectsOfType<Figure>())
        {
            Destroy(figure.gameObject);
        }

        transform.parent.rotation = Quaternion.identity;

        _figureList.Clear();
        _flyingFigure = null;
        _movementTimer = 0;
        _figureListTimer = 0;
    }

    #region DEBUG

    private void OnDrawGizmos()
    {
        Vector2 pos = this.transform.localPosition;
        pos.x -= _gridXOffsetFromWorld;
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
                if (_gameSpace.figureGrid[i, j] != null)
                    Gizmos.color = Color.yellow;
                Gizmos.DrawCube(pos + new Vector2(i + 0.5f, j + 0.5f), Vector2.one * 0.3f);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(pos + new Vector2(_figureStartPos.x + 0.5f, _figureStartPos.y + 0.5f), Vector2.one * 0.3f);
    }

    #endregion
}
