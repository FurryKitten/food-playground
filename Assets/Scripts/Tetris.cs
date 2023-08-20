using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public struct Grid
{
    public int width, height;
    public bool[,] cellsStatus;
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
    private float _spawnTimer = 0;

    private GameState _gameState;

    private float _balaceValue = 0;
    private BalaceState _balaceState;

    private bool _doubleCost = false;
    private bool _trayBorders = false;
    private int _leftGridConstrain = 4;
    private int _rightGridConstrain = 14;
    private int _gridXOffsetFromWorld = 0;
    private Figure[,] _figureGrid;

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
        _gameSpace.cellsStatus = new bool[_gameSpace.width, _gameSpace.height];
        _figureGrid = new Figure[_gameSpace.width, _gameSpace.height];
        for (int i = 0; i < _gameSpace.width; ++i)
        {
            for (int j = 0; j < _gameSpace.height; ++j)
            {
                _gameSpace.cellsStatus[i, j] = false;
                _figureGrid[i, j] = null;
            }
        }

        _dashTime = 0.1f * _movementTime;

        _gameState = ServiceLocator.Current.Get<GameState>();
    }
    private void Update()
    {
        if (_gameState.State == State.PAUSED)
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
            if (transform.parent.rotation.eulerAngles.z >= 340 && transform.parent.rotation.eulerAngles.z < 360f)
            {
                rotationDir = 1;
            }
            else if (transform.parent.rotation.eulerAngles.z > 0.1)
            {
                rotationDir = -1;
            }
        }

        transform.parent.Rotate(0, 0, rotationDir * _trayAngle * Time.deltaTime);
        
        transform.parent.rotation = Quaternion.Euler(0, 0, RotationClamp(transform.parent.rotation.eulerAngles.z));
                
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
                else if(transform.parent.rotation.eulerAngles.z < 350 && transform.parent.rotation.eulerAngles.z >= 330)
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
                    if (_figureSOIdQueue.TryPeek(out int figureSOId)) // Фигура заспавнилась
                    {

                        Vector2 pos = transform.parent.position;
                        _flyingFigure = Instantiate(_defaultFigure, transform.parent);
                        _flyingFigure.Init(_figureSOPrefabs[figureSOId]);
                        _flyingFigure.name = "FlyingFigure";
                        _figureSOIdQueue.Dequeue();

                        _flyingFigure.SetPosition(_figureStartPos.x, _figureStartPos.y);
                        _flyingFigure.SetWorldPosition(_figureStartPos - pos);

                        if (_doubleCost)
                        {
                            if ((Random.Range(0f, 1f) < 0.3f))
                                _flyingFigure.SetDoubleCost();
                        }

                        ServiceLocator.Current.Get<AudioService>().PlayTetrisSpawn();

                        // Если нет места для спавна - удаляем фигуру
                        foreach (Vector2Int p in _flyingFigure.GetForm())
                        {
                            if (_gameSpace.cellsStatus[_figureStartPos.x + _gridXOffsetFromWorld + p.x, _figureStartPos.y - p.y])
                            {
                                _flyingFigure.FlyAway();
                                _flyingFigure = null;
                                _spawnTimer = 0;
                                break;
                            }
                        }
                    }
                    else
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
        }

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
        else // Ставим на доску
        {
            foreach (Vector2Int pos in _flyingFigure.GetForm())
            {
                _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = true;
                _figureGrid[gridX + pos.x, gridY - pos.y] = _flyingFigure;
            }

            _figureList.Add(_flyingFigure);
            _handControls.AddFigures(_figureList);

            _gameState.AddTrayMoney(_flyingFigure.GetProfit());

            _flyingFigure = null;
            _dashMode = false;
            _spawnTimer = _movementTime;
            
            ServiceLocator.Current.Get<AudioService>().PlayTetrisLanding();
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
        Vector2Int _flyingFigurePos = _flyingFigure.GetPosition();
        int gridX = (_flyingFigurePos.x) + dir + _gridXOffsetFromWorld;
        int gridY = (_flyingFigurePos.y);

        foreach (Vector2Int pos in _flyingFigure.GetForm())
            if ((gridX + pos.x ) >= _leftGridConstrain && (gridX + pos.x ) < _rightGridConstrain && gridY - pos.y >= 0)
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
                    if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y])
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
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
                    _figureGrid[gridX + pos.x, gridY - pos.y] = null;
                }
                _figureList[i].FlyAway(dir);
                _onFigureFall?.Invoke();
                lostFigureIndexes.Add(i);
            }

            if(bordured)
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.cellsStatus[gridX + pos.x - dir, gridY - pos.y] = true;
                    _figureGrid[gridX + pos.x - dir, gridY - pos.y] = _figureList[i];
                }
            }

            if(!boundaryCheck && !bordured)
            {
                foreach (Vector2Int pos in _figureList[i].GetForm())
                {
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = true;
                    _figureGrid[gridX + pos.x, gridY - pos.y] = _figureList[i];
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
            // Debug.Log("> 0");
            List<Figure> checkedFigures = new List<Figure>();
            for (int i = _gameSpace.width - 1; i > -1; i--)
                for (int j = 0; j < _gameSpace.height; j++)
                {
                    if (!checkedFigures.Contains(_figureGrid[i, j]) && _figureGrid[i, j] != null)
                    {
                        checkedFigures.Add(_figureGrid[i, j]);
                        Figure lastFigure = _figureGrid[i, j];

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
                                if(gridX + pos.x + dir < _gameSpace.width)
                                {
                                    moveCheck = false;
                                    break;
                                }

                                if (_figureGrid[gridX + pos.x + dir, gridY - pos.y] != lastFigure &&
                                        _figureGrid[gridX + pos.x + dir, gridY - pos.y] != null)
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
                                _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
                                _figureGrid[gridX + pos.x, gridY - pos.y] = null;
                            }
                        }
                    }
                }

        }
        else
        {
            // Debug.Log("<0");
            List<Figure> checkedFigures = new List<Figure>();
            for (int i = 0; i < _gameSpace.width; i++)
                for (int j = 0; j < _gameSpace.height; j++)
                {
                    if (!checkedFigures.Contains(_figureGrid[i, j]) && _figureGrid[i, j] != null)
                    {
                        checkedFigures.Add(_figureGrid[i, j]);
                        Figure lastFigure = _figureGrid[i, j];

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

                                if (gridX + pos.x + dir < _gameSpace.width)
                                {
                                    moveCheck = false;
                                    break;
                                }

                                if (_figureGrid[gridX + pos.x + dir, gridY - pos.y] != lastFigure &&
                                    _figureGrid[gridX + pos.x + dir, gridY - pos.y] != null)
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
                                _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
                                _figureGrid[gridX + pos.x, gridY - pos.y] = null;
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
                if (!checkedFigures.Contains(_figureGrid[i, j]) && _figureGrid[i, j] != null)
                {
                    checkedFigures.Add(_figureGrid[i, j]);
                    Figure lastFigure = _figureGrid[i, j];

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
                        if (_figureGrid[gridX + pos.x, gridY - pos.y - 1] != lastFigure &&
                            _figureGrid[gridX + pos.x, gridY - pos.y - 1] != null)
                        {
                            moveCheck = false;
                            break;
                        }
                    }
                   

                    if (moveCheck)
                    {
                        foreach (Vector2Int pos in lastFigure.GetForm())
                        {
                            _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y] = false;
                            _figureGrid[gridX + pos.x, gridY - pos.y] = null;
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
                if (_gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y])
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
                    _gameSpace.cellsStatus[gridX + pos.x, gridY - pos.y - 1] = true;
                    _figureGrid[gridX + pos.x, gridY - pos.y - 1] = f;
                }
            }

        }

        /*
        foreach (Figure f in _figureList)
        {
            Vector2Int _fPos = f.GetPosition();
            int gridX = (_fPos.x) + _gridXOffsetFromWorld;
            int gridY =  (_fPos.y);

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
                    _figureGrid[gridX + pos.x, gridY - pos.y - 1] = f;
                }
            }

        }
        */
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
        /*int offset = Mathf.RoundToInt((_gameSpace.width - trayWidth) * 0.5f);
        _leftGridConstrain -= offset;
        _rightGridConstrain += offset;*/
        //int offset = Mathf.RoundToInt((_gameSpace.width - trayWidth) * 0.5f);
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
                _gameSpace.cellsStatus[i, j] = false;

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
